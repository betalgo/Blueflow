namespace Blueflow.Chopper.OpenApi;

internal sealed class SplitOptions
{
    public string? SourceFile { get; init; }

    public Uri? SourceUrl { get; init; }

    public string OutputDirectory { get; init; } = string.Empty;

    public bool CleanOutput { get; init; }

    public IReadOnlyCollection<SplitSection> Sections { get; init; } = Array.Empty<SplitSection>();
}