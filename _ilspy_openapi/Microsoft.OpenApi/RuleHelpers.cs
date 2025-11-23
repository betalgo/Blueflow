using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

internal static class RuleHelpers
{
	internal const string DataTypeMismatchedErrorMessage = "Data and type mismatch found.";

	/// <summary>
	/// Input string must be in the format of an email address
	/// </summary>
	/// <param name="input">The input string.</param>
	/// <returns>True if it's an email address. Otherwise False.</returns>
	public static bool IsEmailAddress(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return false;
		}
		string[] array = input.Split('@');
		if (array.Length != 2)
		{
			return false;
		}
		if (string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]))
		{
			return false;
		}
		return true;
	}

	public static void ValidateDataTypeMismatch(IValidationContext context, string ruleName, JsonNode? value, IOpenApiSchema? schema)
	{
		if (schema == null)
		{
			return;
		}
		JsonValueKind? jsonValueKind = value?.GetValueKind();
		string text = ((JsonSchemaType?)((uint?)schema.Type & 0xFFFFFFFEu))?.ToFirstIdentifier();
		string format = schema.Format;
		if (schema.Type.HasValue && (schema.Type.Value & JsonSchemaType.Null) == JsonSchemaType.Null && jsonValueKind.HasValue && jsonValueKind == JsonValueKind.Null)
		{
			return;
		}
		bool flag;
		switch (text)
		{
		case "object":
			if (jsonValueKind.HasValue && jsonValueKind == JsonValueKind.String)
			{
				return;
			}
			if (!(value is JsonObject jsonObject))
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
				return;
			}
			{
				foreach (string item in from _003C_003Eh__TransparentIdentifier0 in Enumerable.Select(jsonObject, delegate(KeyValuePair<string, JsonNode> kvp)
					{
						KeyValuePair<string, JsonNode> keyValuePair = kvp;
						return new
						{
							kvp = kvp,
							key = keyValuePair.Key
						};
					})
					select _003C_003Eh__TransparentIdentifier0.key)
				{
					context.Enter(item);
					if (schema.Properties != null && schema.Properties.TryGetValue(item, out IOpenApiSchema value2))
					{
						ValidateDataTypeMismatch(context, ruleName, jsonObject[item], value2);
					}
					else
					{
						ValidateDataTypeMismatch(context, ruleName, jsonObject[item], schema.AdditionalProperties);
					}
					context.Exit();
				}
				return;
			}
		case "array":
		{
			if (jsonValueKind.HasValue && jsonValueKind == JsonValueKind.String)
			{
				return;
			}
			if (!(value is JsonArray jsonArray))
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
				return;
			}
			for (int i = 0; i < jsonArray.Count; i++)
			{
				context.Enter(i.ToString());
				ValidateDataTypeMismatch(context, ruleName, jsonArray[i], schema.Items);
				context.Exit();
			}
			return;
		}
		case "integer":
		case "number":
			flag = true;
			break;
		default:
			flag = false;
			break;
		}
		if (flag && format == "int32")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.Number)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
			return;
		}
		flag = ((text == "integer" || text == "number") ? true : false);
		if (flag && format == "int64")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.Number)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "integer")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.Number)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "number" && format == "float")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.Number)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "number" && format == "double")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.Number)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "number")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.Number)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "string" && format == "byte")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.String)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "string" && format == "date")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.String)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "string" && format == "date-time")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.String)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "string" && format == "password")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.String)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "string")
		{
			if (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.String)
			{
				context.CreateWarning(ruleName, "Data and type mismatch found.");
			}
		}
		else if (text == "boolean" && (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.True) && (!jsonValueKind.HasValue || jsonValueKind != JsonValueKind.False))
		{
			context.CreateWarning(ruleName, "Data and type mismatch found.");
		}
	}
}
