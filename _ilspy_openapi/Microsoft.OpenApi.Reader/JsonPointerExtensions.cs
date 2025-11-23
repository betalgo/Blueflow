using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Extensions for JSON pointers.
/// </summary>
public static class JsonPointerExtensions
{
	/// <summary>
	/// Finds the JSON node that corresponds to this JSON pointer based on the base Json node.
	/// </summary>
	public static JsonNode? Find(this JsonPointer currentPointer, JsonNode baseJsonNode)
	{
		if (currentPointer.Tokens.Length == 0)
		{
			return baseJsonNode;
		}
		try
		{
			JsonNode jsonNode = baseJsonNode;
			string[] tokens = currentPointer.Tokens;
			foreach (string text in tokens)
			{
				if (jsonNode is JsonArray jsonArray && int.TryParse(text, out var result))
				{
					jsonNode = jsonArray[result];
				}
				else if (jsonNode is JsonObject jsonObject && !jsonObject.TryGetPropertyValue(text, out jsonNode))
				{
					return null;
				}
			}
			return jsonNode;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
