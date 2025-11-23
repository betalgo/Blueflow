using System.Text;
using System.Text.Json;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using Spectre.Console;

namespace Blueflow.Chopper.OpenApi;

internal sealed class OpenApiSplitter
{
    public const string ManifestFileName = "manifest.json";

    private readonly IAnsiConsole _console;
    private readonly OpenApiDocumentLoader _loader;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public OpenApiSplitter(IAnsiConsole console)
    {
        _console = console;
        _loader = new();
    }

    public async Task<SplitManifest> SplitAsync(SplitOptions options)
    {
        if (options.CleanOutput && Directory.Exists(options.OutputDirectory))
        {
            Directory.Delete(options.OutputDirectory, true);
        }

        Directory.CreateDirectory(options.OutputDirectory);

        var (document, diagnostic) = await _loader.LoadAsync(options.SourceFile, options.SourceUrl);

        ReportDiagnostics(diagnostic);

        var manifest = new SplitManifest
        {
            Source = options.SourceUrl?.ToString() ?? options.SourceFile ?? "Unknown",
            GeneratedAt = DateTimeOffset.UtcNow
        };

        if (options.Sections.Contains(SplitSection.Paths))
        {
            var pathFiles = WritePaths(document, options.OutputDirectory);
            manifest.Paths.AddRange(pathFiles);
        }

        if (options.Sections.Contains(SplitSection.Schemas))
        {
            var schemaFiles = WriteSchemas(document, options.OutputDirectory);
            manifest.Schemas.AddRange(schemaFiles);
        }

        PersistManifest(options.OutputDirectory, manifest);

        return manifest;
    }

    private void ReportDiagnostics(OpenApiDiagnostic diagnostic)
    {
        if (diagnostic.Errors.Count == 0)
        {
            _console.MarkupLine("[green]OpenAPI document parsed without errors.[/]");
            return;
        }

        _console.MarkupLine($"[yellow]{diagnostic.Errors.Count} issues detected while parsing OpenAPI file:[/]");
        foreach (var error in diagnostic.Errors.Take(5))
        {
            _console.MarkupLine($" - [red]{Markup.Escape(error.Message)}[/]");
            if (!string.IsNullOrEmpty(error.Pointer))
            {
                _console.MarkupLine($"   [grey]Location: {Markup.Escape(error.Pointer)}[/]");
            }
        }
    }

    private IReadOnlyCollection<string> WritePaths(OpenApiDocument document, string outputDirectory)
    {
        if (document.Paths == null || document.Paths.Count == 0)
        {
            _console.MarkupLine("[yellow]No paths section found.[/]");
            return Array.Empty<string>();
        }

        var artifacts = new List<string>();

        foreach (var (pathKey, pathItem) in document.Paths)
        {
            if (pathItem.Operations is null || pathItem.Operations.Count == 0)
            {
                continue;
            }

            foreach (var (httpMethod, operation) in pathItem.Operations)
            {
                var folderSegments = PathNameFormatter.BuildSegments(pathKey);
                var methodSegment = httpMethod.Method.ToLowerInvariant();
                var fallbackNameSeed = folderSegments.LastOrDefault() ?? "operation";
                var operationName = !string.IsNullOrWhiteSpace(operation.OperationId) ? operation.OperationId : $"{methodSegment}-{fallbackNameSeed}";

                var safeFile = $"{PathNameFormatter.SanitizeFileName(operationName)}.yml";
                var pathSegments = new List<string> { outputDirectory, "paths" };
                pathSegments.AddRange(folderSegments);
                pathSegments.Add(methodSegment);
                var finalPath = Path.Combine(pathSegments.ToArray());
                Directory.CreateDirectory(finalPath);

                var fragment = PathFragmentFactory.Create(pathItem, httpMethod);
                var targetFile = Path.Combine(finalPath, safeFile);

                WritePathFragment(pathKey, fragment, targetFile);
                artifacts.Add(Relativize(outputDirectory, targetFile));
            }
        }

        return artifacts;
    }

    private IReadOnlyCollection<string> WriteSchemas(OpenApiDocument document, string outputDirectory)
    {
        if (document.Components?.Schemas == null || document.Components.Schemas.Count == 0)
        {
            _console.MarkupLine("[yellow]No schemas found.[/]");
            return Array.Empty<string>();
        }

        var artifacts = new List<string>();
        var baseFolder = Path.Combine(outputDirectory, "components", "schemas");
        Directory.CreateDirectory(baseFolder);

        foreach (var (schemaName, schema) in document.Components.Schemas)
        {
            var fileName = $"{PathNameFormatter.SanitizeFileName(schemaName)}.yml";
            var filePath = Path.Combine(baseFolder, fileName);
            WriteSchemaFragment(schemaName, schema, filePath);
            artifacts.Add(Relativize(outputDirectory, filePath));
        }

        return artifacts;
    }

    private void WritePathFragment(string pathKey, OpenApiPathItem pathItem, string filePath)
    {
        using var stream = File.Create(filePath);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        var yamlWriter = new OpenApiYamlWriter(writer);

        yamlWriter.WriteStartObject();
        yamlWriter.WritePropertyName("paths");
        yamlWriter.WriteStartObject();
        yamlWriter.WritePropertyName(pathKey);
        pathItem.SerializeAsV3(yamlWriter);
        yamlWriter.WriteEndObject();
        yamlWriter.WriteEndObject();
    }

    private void WriteSchemaFragment(string schemaName, IOpenApiSchema schema, string filePath)
    {
        using var stream = File.Create(filePath);
        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        var yamlWriter = new OpenApiYamlWriter(writer);

        yamlWriter.WriteStartObject();
        yamlWriter.WritePropertyName("components");
        yamlWriter.WriteStartObject();
        yamlWriter.WritePropertyName("schemas");
        yamlWriter.WriteStartObject();
        yamlWriter.WritePropertyName(schemaName);
        schema.SerializeAsV3(yamlWriter);
        yamlWriter.WriteEndObject();
        yamlWriter.WriteEndObject();
        yamlWriter.WriteEndObject();
    }

    private void PersistManifest(string outputDirectory, SplitManifest manifest)
    {
        var manifestPath = Path.Combine(outputDirectory, ManifestFileName);
        var payload = JsonSerializer.Serialize(manifest, _serializerOptions);
        File.WriteAllText(manifestPath, payload);
    }

    private static string Relativize(string root, string filePath)
    {
        var relative = Path.GetRelativePath(root, filePath);
        return relative.Replace('\\', '/');
    }
}