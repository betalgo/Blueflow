using Betalgo.Blueflow.OpenAPIToCode.Generators;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Betalgo.Blueflow.OpenAPIToCode;

public class BlueFlowOpenApiEngine
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly BlueFlowOpenApiEngineConfiguration _configuration;
    public readonly OpenApiDocument _openApiDocument;
    //private readonly Dictionary<Guid, OpenApiSchema> _definitionsSchema = new();
    private static int count = 0;
    public BlueFlowOpenApiEngine(ICodeGenerator codeGenerator, BlueFlowOpenApiEngineConfiguration configuration)
    {
        _codeGenerator = codeGenerator;
        _configuration = configuration;
        using var stream = File.OpenRead(configuration.OpenApiDocumentationPath);
        _openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);
    }

    public BlueFlowOpenApiEngine(ICodeGenerator codeGenerator)
        : this(codeGenerator, new BlueFlowOpenApiEngineConfiguration())
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
            Usings = [
                "System",
                "System.Text.Json.Serialization",
                "System.Collections",
                "System.Collections.Generic",
                $"System.Text.Json"]
        };
        var classesDir = _configuration.OutputDirectory;
        var code = _codeGenerator.RenderFile(fileDef);
        var filePath = GetUniqueFilePath(classesDir, $"{schema.GetBlueflowName() ?? schema.GetSelfKey()}.cs");
        File.WriteAllText(filePath, code);
    }
    public void Start2()
    {
        GenerateBaseFilesIfNotExistAsync(_configuration.OutputDirectory, _configuration.ProjectName);
        ProcessSchema();
        //  ExportOpenAPiDocument();
        CollectClasses();
        ExportOpenAPiDocument();
        GenerateCodes2();
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


    private void ProcessSchema()
    {
        foreach (var (name, schema) in _openApiDocument.Components.Schemas)
        {
            schema.SetAsMainComponent();
        }
        Action<OpenApiSchema, string?>[] rules =
                [
                    SchemaHelpers.SetBlueflowId,
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



    private void ProcessSchemaRecursively(OpenApiSchema? schema,
        string key,
        params Action<OpenApiSchema, string?>[] rules)
    {
        if (schema == null) return;

        foreach (var r in rules) r(schema, key);

        foreach (var prop in schema.Properties.Where(r => r.Value.Reference == null))
            ProcessSchemaRecursively(prop.Value, prop.Key, rules);

        if (schema is { Type: "array", Items: not null } && schema.Items.Reference == null)
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
        var writer = new Microsoft.OpenApi.Writers.OpenApiYamlWriter(new StreamWriter(outputStream));
        _openApiDocument.SerializeAsV3(writer);
        writer.Flush();
    }


    // Returns a unique file path by appending _1, _2, etc. if needed
    private string GetUniqueFilePath(string directory, string baseFileName)
    {
        string filePath = Path.Combine(directory, baseFileName);
        if (!File.Exists(filePath))
            return filePath;
        string name = Path.GetFileNameWithoutExtension(baseFileName);
        string ext = Path.GetExtension(baseFileName);
        int counter = 1;
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

        for (int i = 0; i < baseFiles.Count && i < baseFileNames.Length; i++)
        {
            var filePath = Path.Combine(outputDirectory, baseFileNames[i]);
            if (!File.Exists(filePath))
            {
                // For solution and project templates, render with Scriban if needed
                var content = baseFiles[i];
                if (baseFileNames[i].EndsWith(".sln"))
                {
                    var template = Scriban.Template.Parse(content);
                    content = template.Render(new { solution_guid = Guid.NewGuid().ToString("B").ToUpper(), project_name = projectName, project_guid = Guid.NewGuid().ToString("B").ToUpper() });
                }
                else if (baseFileNames[i].EndsWith(".csproj"))
                {
                    var template = Scriban.Template.Parse(content);
                    content = template.Render(new { target_framework = "net9.0" });
                }
                else if (baseFileNames[i].EndsWith(".cs"))
                {
                    var template = Scriban.Template.Parse(content);
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

    /// <summary>
    /// Returns a dictionary mapping x-blueflow-id GUIDs to their schema/property/enum names from the OpenAPI file.
    /// </summary>
    public Dictionary<string, string> GetBlueflowIdDictionary(string openApiFilePath)
    {
        var result = new Dictionary<string, string>();
        using var stream = File.OpenRead(openApiFilePath);
        var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);

        // Root document
        if (openApiDocument.Extensions != null && openApiDocument.Extensions.TryGetValue("x-blueflow-id", out var rootIdExt))
        {
            if (rootIdExt is OpenApiString rootId)
                result[rootId.Value] = "root";
        }

        // Schemas
        if (openApiDocument.Components?.Schemas != null)
        {
            foreach (var (key, schema) in openApiDocument.Components.Schemas)
            {
                if (schema.Extensions != null && schema.Extensions.TryGetValue("x-blueflow-id", out var ext) && ext is OpenApiString id)
                    result[id.Value] = key;
                // Properties
                if (schema.Properties != null)
                {
                    foreach (var propKvp in schema.Properties)
                    {
                        var prop = propKvp.Value;
                        if (prop.Extensions != null && prop.Extensions.TryGetValue("x-blueflow-id", out var propExt) && propExt is OpenApiString propId)
                            result[propId.Value] = $"{key}.{propKvp.Key}";
                    }
                }
            }
        }

        // Parameters
        if (openApiDocument.Paths != null)
        {
            foreach (var pathItem in openApiDocument.Paths)
            {
                foreach (var operation in pathItem.Value.Operations.Values)
                {
                    if (operation.Parameters == null) continue;
                    foreach (var parameter in operation.Parameters)
                    {
                        if (parameter.Schema != null && parameter.Schema.Extensions != null && parameter.Schema.Extensions.TryGetValue("x-blueflow-id", out var paramExt) && paramExt is OpenApiString paramId)
                        {
                            result[paramId.Value] = $"parameter:{parameter.Name}";
                        }
                    }
                }
            }
        }
        return result;
    }

    private static Dictionary<Guid, string> _blueflowIdNameDictionary = new();
    private static bool _blueflowIdNameDictionaryLoaded = false;

    /// <summary>
    /// Loads the BlueflowId-Name dictionary from the OpenAPI file (in memory only).
    /// </summary>
    public void LoadBlueflowIdNameDictionary(string openApiFilePath)
    {
        if (_blueflowIdNameDictionaryLoaded) return;
        var dict = GetBlueflowIdDictionary(openApiFilePath)
            .Where(kvp => Guid.TryParse(kvp.Key, out _))
            .ToDictionary(kvp => Guid.Parse(kvp.Key), kvp => kvp.Value);
        _blueflowIdNameDictionary = dict;
        _blueflowIdNameDictionaryLoaded = true;
    }

    /// <summary>
    /// Updates the name for a given BlueflowId in the dictionary (in memory only).
    /// </summary>
    public void UpdateBlueflowIdName(Guid id, string newName)
    {
        _blueflowIdNameDictionary[id] = newName;
    }

    /// <summary>
    /// Gets the canonical name for a given BlueflowId from the dictionary.
    /// </summary>
    public string GetNameByBlueflowId(Guid id)
    {
        if (_blueflowIdNameDictionary.TryGetValue(id, out var name))
            return name;
        return string.Empty;
    }

    /// <summary>
    /// Gets an OpenApiSchema by its reference.
    /// </summary>
    private OpenApiSchema? GetSchemaByReference(OpenApiReference reference, OpenApiDocument? document = null)
    {
        if (string.IsNullOrEmpty(reference?.Id) || document?.Components?.Schemas == null)
            return null;

        // For schema references, extract the name from the reference path
        if (reference.Type == ReferenceType.Schema)
        {
            string schemaName = reference.Id;
            if (document.Components.Schemas.TryGetValue(schemaName, out var schema))
                return schema;
        }

        return null;
    }

    // Refactor: Use GetNameByBlueflowId for all name assignments in code generation
    // Example for ClassDefinition and PropertyDefinition:
    // When creating ClassDefinition or PropertyDefinition, set Name = GetNameByBlueflowId(id)

    /// <summary>
    /// Determines the TypeId for a property that references another type.
    /// </summary>
    private Guid? GetTypeIdForProperty(OpenApiSchema prop, string propertyName)
    {
        // Case 1: Direct reference to another schema
        if (!string.IsNullOrEmpty(prop.Reference?.Id))
        {
            // Look for the reference in our dictionary by name
            var entries = _blueflowIdNameDictionary
                .Where(kvp => kvp.Value == prop.Reference.Id)
                .ToList();

            if (entries.Any())
            {
                return entries.First().Key;
            }
        }
        // Case 2: For array items with references
        else if (prop.Type == "array" && prop.Items?.Reference != null && !string.IsNullOrEmpty(prop.Items.Reference.Id))
        {
            // Look for the items reference in our dictionary by name
            var entries = _blueflowIdNameDictionary
                .Where(kvp => kvp.Value == prop.Items.Reference.Id)
                .ToList();

            if (entries.Any())
            {
                return entries.First().Key;
            }
        }
        // Case 3: For enum types identified earlier
        else if (prop.Enum != null && prop.Enum.Count > 0 && (prop.Type == "string" || prop.Type == null))
        {
            // For enums, use the enum's BlueflowId that was assigned earlier
            if (prop.Extensions != null &&
                prop.Extensions.TryGetValue("x-blueflow-id", out var enumExt) &&
                enumExt is OpenApiString enumIdStr &&
                Guid.TryParse(enumIdStr.Value, out var enumId))
            {
                return enumId;
            }
        }

        return null;
    }
}