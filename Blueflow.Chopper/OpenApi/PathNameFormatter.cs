using System.Text;

namespace Blueflow.Chopper.OpenApi;

internal static class PathNameFormatter
{
    private const string ParameterPrefix = "param";

    public static IReadOnlyList<string> BuildSegments(string rawPath)
    {
        var trimmed = rawPath.Trim('/');
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return ["root"];
        }

        var segments = trimmed.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(SanitizeSegment).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        return segments.Length == 0 ? ["root"] : segments;
    }

    public static string SanitizeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "snippet";
        }

        var invalid = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);

        foreach (var c in value)
        {
            builder.Append(invalid.Contains(c) ? '-' : char.ToLowerInvariant(c));
        }

        var sanitized = builder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(sanitized) ? "snippet" : sanitized;
    }

    private static string SanitizeSegment(string segment)
    {
        var normalized = segment;
        if (segment.StartsWith('{') && segment.EndsWith('}'))
        {
            normalized = $"{ParameterPrefix}-{segment.Trim('{', '}', ' ')}";
        }

        var builder = new StringBuilder(normalized.Length);
        var invalid = Path.GetInvalidPathChars();

        foreach (var c in normalized)
        {
            if (invalid.Contains(c))
            {
                builder.Append('-');
                continue;
            }

            if (char.IsLetterOrDigit(c) || c is '-' or '_' or '.')
            {
                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append('-');
            }
        }

        var sanitized = builder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(sanitized) ? "segment" : sanitized;
    }
}