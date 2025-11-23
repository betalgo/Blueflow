using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Extensions class for strings to handle special characters.
/// </summary>
public static class SpecialCharacterStringExtensions
{
	private static readonly HashSet<string> _yamlIndicators = new HashSet<string>
	{
		"-", "?", ":", ",", "{", "}", "[", "]", "&", "*",
		"#", "?", "|", "-", ">", "!", "%", "@", "`", "'",
		"\""
	};

	private static readonly HashSet<string> _yamlPlainStringForbiddenCombinations = new HashSet<string> { ": ", " #", "[", "]", "{", "}", "," };

	private static readonly HashSet<string> _yamlPlainStringForbiddenTerminals = new HashSet<string> { ":" };

	private static readonly Dictionary<char, string> _yamlControlCharacterCharReplacements = new Dictionary<char, string>
	{
		{ '\0', "\\0" },
		{ '\u0001', "\\x01" },
		{ '\u0002', "\\x02" },
		{ '\u0003', "\\x03" },
		{ '\u0004', "\\x04" },
		{ '\u0005', "\\x05" },
		{ '\u0006', "\\x06" },
		{ '\a', "\\a" },
		{ '\b', "\\b" },
		{ '\t', "\\t" },
		{ '\n', "\\n" },
		{ '\v', "\\v" },
		{ '\f', "\\f" },
		{ '\r', "\\r" },
		{ '\u000e', "\\x0e" },
		{ '\u000f', "\\x0f" },
		{ '\u0010', "\\x10" },
		{ '\u0011', "\\x11" },
		{ '\u0012', "\\x12" },
		{ '\u0013', "\\x13" },
		{ '\u0014', "\\x14" },
		{ '\u0015', "\\x15" },
		{ '\u0016', "\\x16" },
		{ '\u0017', "\\x17" },
		{ '\u0018', "\\x18" },
		{ '\u0019', "\\x19" },
		{ '\u001a', "\\x1a" },
		{ '\u001b', "\\x1b" },
		{ '\u001c', "\\x1c" },
		{ '\u001d', "\\x1d" },
		{ '\u001e', "\\x1e" },
		{ '\u001f', "\\x1f" }
	};

	private static readonly Dictionary<string, string> _yamlControlCharacterStringReplacements = _yamlControlCharacterCharReplacements.ToDictionary<KeyValuePair<char, string>, string, string>((KeyValuePair<char, string> x) => x.Key.ToString(), (KeyValuePair<char, string> x) => x.Value);

	/// <summary>
	/// Escapes all special characters and put the string in quotes if necessary to
	/// get a YAML-compatible string.
	/// </summary>
	internal static string GetYamlCompatibleString(this string input)
	{
		string text = input;
		if (text == null || text.Length != 0)
		{
			if (!(text == "null"))
			{
				if (text == "~")
				{
					return "'~'";
				}
				if (input.Any((char c) => _yamlControlCharacterCharReplacements.ContainsKey(c)))
				{
					input = input.Replace("\\", "\\\\");
					input = input.Replace("\"", "\\\"");
					foreach (KeyValuePair<string, string> yamlControlCharacterStringReplacement in _yamlControlCharacterStringReplacements)
					{
						input = input.Replace(yamlControlCharacterStringReplacement.Key, yamlControlCharacterStringReplacement.Value);
					}
					return "\"" + input + "\"";
				}
				if (_yamlPlainStringForbiddenCombinations.Any((string fc) => input.Contains(fc)) || _yamlIndicators.Any((string i) => input.StartsWith(i, StringComparison.Ordinal)) || _yamlPlainStringForbiddenTerminals.Any((string i) => input.EndsWith(i)) || input.Trim() != input)
				{
					input = input.Replace("'", "''");
					return "'" + input + "'";
				}
				if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var _) || IsHexadecimalNotation(input) || bool.TryParse(input, out var _) || DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
				{
					return "'" + input + "'";
				}
				return input;
			}
			return "'null'";
		}
		return "''";
	}

	/// <summary>
	/// Handles control characters and backslashes and adds double quotes
	/// to get JSON-compatible string.
	/// </summary>
	internal static string GetJsonCompatibleString(this string value)
	{
		if (value == null)
		{
			return "null";
		}
		value = value.Replace("\\", "\\\\").Replace("\b", "\\b").Replace("\f", "\\f")
			.Replace("\n", "\\n")
			.Replace("\r", "\\r")
			.Replace("\t", "\\t")
			.Replace("\"", "\\\"");
		return "\"" + value + "\"";
	}

	internal static bool IsHexadecimalNotation(string input)
	{
		int result;
		if (input.StartsWith("0x", StringComparison.Ordinal))
		{
			return int.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
		}
		return false;
	}
}
