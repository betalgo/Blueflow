using System.Text;
using System.Text.RegularExpressions;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp.Services;

/// <summary>
///     C# implementation of the documentation normalizer service.
/// </summary>
public class CSharpDocumentationNormalizerService : IDocumentationNormalizerService
{
    private string? _baseDomain;

    public void SetBaseDomain(string? baseDomain)
    {
        _baseDomain = baseDomain;
    }

    /// <summary>
    ///     Normalizes a documentation string for C# code documentation.
    /// </summary>
    /// <param name="input">The input documentation string.</param>
    /// <returns>The normalized documentation string.</returns>
    public string? Normalize(string? input)
    {
        if (input == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Normalize line endings to \n
        var normalized = Regex.Replace(input, "\r\n|\r|\n", "\n");

        // Create unique placeholders that won't be affected by XML encoding
        var inlineMarker = Guid.NewGuid().ToString("N");
        var blockMarker = Guid.NewGuid().ToString("N");

        // Extract inline code blocks to protect them during processing
        var codeBlockRegex = new Regex(@"`([^`]+)`");
        var inlineCodeBlocks = new List<string>();

        normalized = codeBlockRegex.Replace(normalized, match =>
        {
            inlineCodeBlocks.Add(match.Groups[1].Value);
            return $"{inlineMarker}{inlineCodeBlocks.Count - 1}{inlineMarker}";
        });

        // Extract multi-line code blocks
        var multilineCodeBlockRegex = new Regex(@"```([^`]*?)```", RegexOptions.Singleline);
        var multilineCodeBlocks = new List<string>();

        normalized = multilineCodeBlockRegex.Replace(normalized, match =>
        {
            multilineCodeBlocks.Add(match.Groups[1].Value.Trim());
            return $"{blockMarker}{multilineCodeBlocks.Count - 1}{blockMarker}";
        });

        // Escape XML special characters AFTER extracting code blocks
        normalized = normalized.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

        // Convert markdown links [text](url) to <see href="url">text</see>
        normalized = Regex.Replace(normalized, @"\[([^\]]+)\]\(([^)]+)\)", m =>
        {
            var url = m.Groups[2].Value;
            if (!string.IsNullOrEmpty(_baseDomain) && !url.StartsWith("http://") && !url.StartsWith("https://") && !url.StartsWith("/"))
            {
                // If the URL is relative (does not start with http(s):// or /), prepend the baseDomain
                url = _baseDomain.TrimEnd('/') + "/" + url.TrimStart('/');
            }
            else if (!string.IsNullOrEmpty(_baseDomain) && url.StartsWith("/"))
            {
                // If the URL is root-relative, prepend the baseDomain
                url = _baseDomain.TrimEnd('/') + url;
            }

            return $"<see href=\"{url}\">{m.Groups[1].Value}</see>";
        });

        // Convert bold **text** or __text__ to <b>text</b>
        normalized = Regex.Replace(normalized, @"(\*\*|__)(.*?)\1", "<b>$2</b>");

        // Convert italic *text* or _text_ to <i>text</i>
        normalized = Regex.Replace(normalized, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)", "<i>$1</i>");
        normalized = Regex.Replace(normalized, @"(?<!_)_(?!_)(.+?)(?<!_)_(?!_)", "<i>$1</i>");

        // Insert paragraph breaks for empty lines
        normalized = Regex.Replace(normalized, @"\n\s*\n", "\n<para />\n");

        // Restore inline code blocks with <c> tags
        for (var i = 0; i < inlineCodeBlocks.Count; i++)
        {
            normalized = normalized.Replace($"{inlineMarker}{i}{inlineMarker}", $"<c>{inlineCodeBlocks[i]}</c>");
        }

        // Restore multiline code blocks with <code> tags
        for (var i = 0; i < multilineCodeBlocks.Count; i++)
        {
            normalized = normalized.Replace($"{blockMarker}{i}{blockMarker}", $"<code>{multilineCodeBlocks[i]}</code>");
        }

        // Ensure every line starts with "///     "
        var lines = normalized.Split('\n');
        var result = new StringBuilder();

        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                result.AppendLine("///     " + line.Trim());
            }
            else
            {
                result.AppendLine("///     ");
            }
        }

        return result.ToString().TrimEnd();
    }
}