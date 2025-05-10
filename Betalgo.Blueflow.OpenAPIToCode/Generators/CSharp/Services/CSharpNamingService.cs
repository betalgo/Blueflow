using System.Globalization;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;
using Betalgo.Blueflow.OpenAPIToCode.Utils;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp.Services;

/// <summary>
///     CSharp-specific implementation of INamingService with C# naming conventions.
/// </summary>
public class CSharpNamingService : INamingService
{
    private const string ReservedKeywordSuffix = "Value";

    // List of C# reserved keywords
    private static readonly HashSet<string> CSharpReservedKeywords =
    [
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false",
        "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private",
        "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
        "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "async", "await", "by", "descending", "dynamic", "equals", "from", "get", "global", "group", "into", "join", "let", "nameof", "on",
        "orderby", "partial", "remove", "select", "set", "unmanaged", "value", "var", "when", "where", "yield"
    ];

    private static readonly char[] IllegalCSharpIdentifierChars =
    [
        ' ', '-', '_', '/', '\\', '[', ']', '{', '}', '(', ')', '.', ',', ';', ':', '\'', '"', '<', '>', '?', '!', '@', '#', '$', '%', '^', '&', '*', '+', '=', '|', '~', '`'
    ];

    public string Convert(string name, NamingPurpose purpose)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var result = purpose switch
        {
            NamingPurpose.EnumMember => ToPascalCase(name),
            NamingPurpose.Class => ToPascalCase(name) + "Model",
            NamingPurpose.Enum => ToPascalCase(name) + "Enum",
            NamingPurpose.OneOfClass => ToPascalCase(name) + "OneOfModel",
            NamingPurpose.AnyOfClass => ToPascalCase(name) + "AnyOfModel",
            NamingPurpose.AllOfClass => ToPascalCase(name) + "AllOfModel",
            NamingPurpose.Property => ToPascalCase(name),
            NamingPurpose.AsProperty => "As" + ToPascalCase(name),
            NamingPurpose.Method => ToPascalCase(name),
            NamingPurpose.Parameter => ToCamelCase(name),
            _ => name
        };
        // Always validate identifier for all naming purposes
        return ToValidCSharpIdentifier(result);
    }

    private static string ToValidCSharpIdentifier(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        // If the first character is not a letter or underscore, prepend underscore
        if (!char.IsLetter(input[0]) && input[0] != '_')
            input = "_" + input;
        // Replace any remaining invalid characters with underscores
        var chars = input.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray();
        var identifier = new string(chars);
        // If identifier is a reserved keyword, append suffix instead of using @
        if (CSharpReservedKeywords.Contains(identifier))
            identifier = identifier + ReservedKeywordSuffix;
        return identifier;
    }

    public string ToPascalCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        // Split on all illegal C# identifier characters
        if (input.IndexOfAny(IllegalCSharpIdentifierChars) >= 0)
        {
            var words = input.Split(IllegalCSharpIdentifierChars, StringSplitOptions.RemoveEmptyEntries);
            var result = string.Concat(words.Select(ToPascalCase));
            return result;
        }

        // If input is already PascalCase or camelCase, just uppercase the first letter
        return char.ToUpperInvariant(input[0]) + input[1..];
    }

    private string ToCamelCase(string input)
    {
        var pascal = ToPascalCase(input);
        if (string.IsNullOrEmpty(pascal)) return pascal;
        return char.ToLowerInvariant(pascal[0]) + pascal[1..];
    }
}