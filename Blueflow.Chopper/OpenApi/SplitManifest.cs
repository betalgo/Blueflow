namespace Blueflow.Chopper.OpenApi;

internal sealed class SplitManifest
{
    public string Source { get; init; } = string.Empty;

    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;

    public List<string> Paths { get; } = new();

    public List<string> Schemas { get; } = new();
}