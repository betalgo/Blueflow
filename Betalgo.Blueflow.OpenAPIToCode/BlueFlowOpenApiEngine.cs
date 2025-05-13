using Betalgo.Blueflow.OpenApiExtensions;
using Betalgo.Blueflow.OpenAPIToCode.Generators;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Scriban;
using System.Diagnostics;
using System.IO;

namespace Betalgo.Blueflow.OpenAPIToCode;

public class BlueFlowOpenApiEngine
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly BlueFlowOpenApiEngineConfiguration _configuration;
    private readonly OpenApiDocument _openApiDocument;

    public BlueFlowOpenApiEngine(ICodeGenerator codeGenerator, BlueFlowOpenApiEngineConfiguration configuration)
    {
        _codeGenerator = codeGenerator;
        _configuration = configuration;
        if (configuration.OpenApiDocumentationPath != null)
        {
            using var stream = File.OpenRead(configuration.OpenApiDocumentationPath);
            _openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);
        }
    }

    public BlueFlowOpenApiEngine(ICodeGenerator codeGenerator) : this(codeGenerator, new())
    {
    }

    private void GenerateCodes2()
    {
        foreach (var (name, schema) in _openApiDocument.Components.Schemas)
        {
            GenerateCodes3(schema, name);
        }
    }

    private void GenerateCodes3(OpenApiSchema schema, string? key)
    {
        var data = _codeGenerator.Render(schema);
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        var fileDef = new FileDefinition
        {
            Namespace = _configuration.ProjectName,
            Content = [_codeGenerator.Render(schema)],
            Usings =
            [
                "System",
                "System.Text.Json.Serialization",
                "System.Collections",
                "System.Collections.Generic",
                "System.Text.Json"
            ]
        };
        var classesDir = _configuration.OutputDirectory;
        var code = _codeGenerator.RenderFile(fileDef);
        var filePath = GetUniqueFilePath(classesDir, $"{schema.GetBlueflowName() ?? schema.GetSelfKey()}.cs");
        File.WriteAllText(filePath, code);
    }

    public void Start2()
    {
        GenerateBaseFilesIfNotExistAsync(_configuration.OutputDirectory, _configuration.ProjectName);
        ProcessPaths();
        ProcessSchema();
        //  ExportOpenAPiDocument();
        CollectClasses();
        ExportOpenAPiDocument();
        GenerateCodes2();
      //  RunCodeCleanup();
    }

    private void RunCodeCleanup()
    {
        try
        {
            Console.WriteLine("Running ReSharper code cleanup...");
            var solutionPath = Path.Combine(_configuration.OutputDirectory, $"{_configuration.ProjectName}.sln");
            
            if (File.Exists(solutionPath))
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "jb",
                    Arguments = $"cleanupcode \"{solutionPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                };

                using var process = Process.Start(processStartInfo);
                if (process != null)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    Console.WriteLine($"Code cleanup completed with exit code: {process.ExitCode}");
                    Console.WriteLine(output);
                }
                else
                {
                    Console.WriteLine("Failed to start code cleanup process.");
                }
            }
            else
            {
                Console.WriteLine($"Solution file not found at: {solutionPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running code cleanup: {ex.Message}");
        }
    }

    private void CollectClasses()
    {
        foreach (var (name, schema) in _openApiDocument.Components.Schemas)
        {
            ProcessSchemaRecursively(schema, name, [FindClass]);
        }
    }

    private void FindClass(OpenApiSchema schema, string? key)
    {
        var name = key;
        if (!schema.IsMainComponent())
        {
            name = schema.Title ?? schema.GetBaseType() ?? key;
        }

        if (schema.IsArray())
        {
            foreach (var openApiSchema in schema.Items.Properties)
            {
                openApiSchema.Value.SetItemOfArray();
            }

            foreach (var openApiSchema in schema.Items.AnyOf)
            {
                schema.Items.SetItemOfArray();
                openApiSchema.SetItemOfArray();
            }

            foreach (var openApiSchema in schema.Items.OneOf)
            {
                schema.Items.SetItemOfArray();
                openApiSchema.SetItemOfArray();
            }

            foreach (var openApiSchema in schema.Items.AllOf)
            {
                schema.Items.SetItemOfArray();
                openApiSchema.SetItemOfArray();
            }
        }

        if (schema.IsItemOfArray() && schema.Reference == null)
        {
            name += "Item";
        }

        if (schema.IsObject())
        {
            if (schema.IsOneOf())
            {
                schema.SetBlueflowName(_codeGenerator.NamingService.Convert(name, NamingPurpose.OneOfClass));
                foreach (var openApiSchema in schema.OneOf)
                {
                    openApiSchema.SetBlueflowPropertyType("OneOf");
                }
            }
            else if (schema.IsAnyOf())
            {
                schema.SetBlueflowName(_codeGenerator.NamingService.Convert(name, NamingPurpose.AnyOfClass));
                foreach (var openApiSchema in schema.AnyOf)
                {
                    openApiSchema.SetBlueflowPropertyType("AnyOf");
                }
            }
            else if (schema.IsAllOf())
            {
                schema.SetBlueflowName(_codeGenerator.NamingService.Convert(name, NamingPurpose.AllOfClass));
                foreach (var openApiSchema in schema.AllOf)
                {
                    openApiSchema.SetBlueflowPropertyType("AllOf");
                }
            }
            else
            {
                schema.SetBlueflowName(_codeGenerator.NamingService.Convert(name, NamingPurpose.Class));
            }
        }
        else if (schema.IsEnum())
        {
            schema.SetBlueflowName(_codeGenerator.NamingService.Convert(name, NamingPurpose.Enum));
        }
    }


    private void ProcessPaths()
    {
        // Generate schemas for path parameters and replace parameters with references
        SchemaHelpers.GeneratePathParameterSchemas(_openApiDocument);
    }

    private void ProcessSchema()
    {
      
        
        foreach (var (name, schema) in _openApiDocument.Components.Schemas)
        {
            schema.SetAsMainComponent();
        }

        Action<OpenApiSchema, string?>[] rules =
        [
            (schema, key) => schema.SetBlueflowId(),
            SchemaHelpers.SetBlueflowSelfKey,
            (schema, key) => schema.ConvertPolyToEnumIfApplicable(),
            (schema, key) => schema.ConvertNullableReferenceIfApplicable(),
            (schema, key) => ConvertTypeIfItIsPoly(schema, key, "object"),
            FixMissingTypes
        ];

        foreach (var (name, schema) in _openApiDocument.Components.Schemas)
        {
            ProcessSchemaRecursively(schema, name, rules);
        }
    }


    private void ProcessSchemaRecursively(OpenApiSchema? schema, string key, params Action<OpenApiSchema, string?>[] rules)
    {
        if (schema == null) return;

        foreach (var r in rules) r(schema, key);

        foreach (var prop in schema.Properties.Where(r => r.Value.Reference == null))
            ProcessSchemaRecursively(prop.Value, prop.Key, rules);

        if (schema is { Type: "array", Items.Reference: null })
            ProcessSchemaRecursively(schema.Items, key, rules);

        foreach (var s in schema.AllOf.Where(r => r.Reference == null)) ProcessSchemaRecursively(s, key, rules);
        foreach (var s in schema.OneOf.Where(r => r.Reference == null)) ProcessSchemaRecursively(s, key, rules);
        foreach (var s in schema.AnyOf.Where(r => r.Reference == null)) ProcessSchemaRecursively(s, key, rules);
    }

    public void ExportOpenAPiDocument()
    {
        var originalPath = _configuration.OpenApiDocumentationPath;
        var directory = Path.GetDirectoryName(originalPath);
        var filename = Path.GetFileNameWithoutExtension(originalPath);
        var extension = Path.GetExtension(originalPath);
        var newPath = Path.Combine(directory ?? string.Empty, $"{filename}_v2{extension}");
        using var outputStream = File.Create(newPath);
        var writer = new OpenApiYamlWriter(new StreamWriter(outputStream));
        _openApiDocument.SerializeAsV3(writer);
        writer.Flush();
    }


    // Returns a unique file path by appending _1, _2, etc. if needed
    private string GetUniqueFilePath(string directory, string baseFileName)
    {
        var filePath = Path.Combine(directory, baseFileName);
        if (!File.Exists(filePath))
            return filePath;
        var name = Path.GetFileNameWithoutExtension(baseFileName);
        var ext = Path.GetExtension(baseFileName);
        var counter = 1;
        string newFilePath;
        do
        {
            newFilePath = Path.Combine(directory, $"{name}_{counter}{ext}");
            counter++;
        } while (File.Exists(newFilePath));

        return newFilePath;
    }


    private string ApplyClassNamePrefixSuffix(string baseName)
    {
        var prefix = _codeGenerator.Configuration.ClassNamePrefix;
        var suffix = _codeGenerator.Configuration.ClassNameSuffix;
        var result = baseName;
        if (!string.IsNullOrEmpty(prefix) && !result.StartsWith(prefix))
            result = prefix + result;
        if (!string.IsNullOrEmpty(suffix) && !result.EndsWith(suffix))
            result = result + suffix;
        return result;
    }


    public async Task GenerateBaseFilesIfNotExistAsync(string outputDirectory, string projectName)
    {
        Directory.CreateDirectory(outputDirectory);
        var baseFiles = _codeGenerator.RenderBase();
        var baseFileNames = new[] { $"{projectName}.sln", $"{projectName}.csproj" };

        for (var i = 0; i < baseFiles.Count && i < baseFileNames.Length; i++)
        {
            var filePath = Path.Combine(outputDirectory, baseFileNames[i]);
            if (!File.Exists(filePath))
            {
                // For solution and project templates, render with Scriban if needed
                var content = baseFiles[i];
                if (baseFileNames[i].EndsWith(".sln"))
                {
                    var template = Template.Parse(content);
                    content = template.Render(new { solution_guid = Guid.NewGuid().ToString("B").ToUpper(), project_name = projectName, project_guid = Guid.NewGuid().ToString("B").ToUpper() });
                }
                else if (baseFileNames[i].EndsWith(".csproj"))
                {
                    var template = Template.Parse(content);
                    content = template.Render(new { target_framework = "net9.0" });
                }
                else if (baseFileNames[i].EndsWith(".cs"))
                {
                    var template = Template.Parse(content);
                    content = template.Render(new { project_namespace = projectName });
                }

                await File.WriteAllTextAsync(filePath, content);
            }
        }
    }

    private static void ConvertTypeIfItIsPoly(OpenApiSchema schema, string? key, string? to)
    {
        if (schema.IsPoly() && schema.Type == null)
        {
            schema.Type = to;
        }
    }

    private static void FixMissingTypes(OpenApiSchema schema, string? key)
    {
        if (schema.Type == null && schema.Properties != null && schema.Properties.Any())
        {
            schema.Type = "object";
        }
    }
}