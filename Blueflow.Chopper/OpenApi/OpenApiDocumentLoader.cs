using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.YamlReader;

namespace Blueflow.Chopper.OpenApi;

internal sealed class OpenApiDocumentLoader
{
    public async Task<OpenApiDocumentResult> LoadAsync(string? filePath, Uri? sourceUrl)
    {
        if (sourceUrl != null)
        {
            return await LoadFromUrlAsync(sourceUrl);
        }

        if (filePath != null)
        {
            return LoadFromFile(filePath);
        }

        throw new ArgumentException("Either file path or URL must be provided.");
    }

    private async Task<OpenApiDocumentResult> LoadFromUrlAsync(Uri url)
    {
        using var client = new HttpClient();
        var content = await client.GetByteArrayAsync(url);
        using var stream = new MemoryStream(content);
        return LoadFromStream(stream, url);
    }

    private OpenApiDocumentResult LoadFromFile(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        using var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        var baseUri = new Uri(Path.GetFullPath(filePath));
        return LoadFromStream(memoryStream, baseUri);
    }

    private OpenApiDocumentResult LoadFromStream(MemoryStream stream, Uri baseUri)
    {
        var reader = new OpenApiYamlReader();
        var settings = new OpenApiReaderSettings
        {
            BaseUrl = baseUri,
            LoadExternalRefs = true
        };

        var readResult = reader.Read(stream, baseUri, settings);
        var document = readResult.Document ?? throw new InvalidOperationException($"Unable to parse '{baseUri}'.");
        var diagnostic = readResult.Diagnostic ?? new OpenApiDiagnostic();

        return new(document, diagnostic);
    }
}

internal sealed record OpenApiDocumentResult(OpenApiDocument Document, OpenApiDiagnostic Diagnostic);