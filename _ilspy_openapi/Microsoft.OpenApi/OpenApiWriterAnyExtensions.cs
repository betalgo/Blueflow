using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Extensions methods for writing the <see cref="T:Microsoft.OpenApi.JsonNodeExtension" />
/// </summary>
public static class OpenApiWriterAnyExtensions
{
	/// <summary>
	/// Write the specification extensions
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="extensions">The specification extensions.</param>
	/// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
	public static void WriteExtensions(this IOpenApiWriter writer, IDictionary<string, IOpenApiExtension>? extensions, OpenApiSpecVersion specVersion)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (extensions == null)
		{
			return;
		}
		foreach (KeyValuePair<string, IOpenApiExtension> extension in extensions)
		{
			writer.WritePropertyName(extension.Key);
			if (extension.Value == null)
			{
				writer.WriteNull();
			}
			else
			{
				extension.Value.Write(writer, specVersion);
			}
		}
	}

	/// <summary>
	/// Write the <see cref="T:System.Text.Json.Nodes.JsonNode" /> value.
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="node">The JsonNode value</param>
	public static void WriteAny(this IOpenApiWriter writer, JsonNode? node)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (node == null || node.IsJsonNullSentinel())
		{
			writer.WriteNull();
			return;
		}
		switch (node.GetValueKind())
		{
		case JsonValueKind.Array:
			writer.WriteArray(node as JsonArray);
			break;
		case JsonValueKind.Object:
			writer.WriteObject(node as JsonObject);
			break;
		case JsonValueKind.String:
			writer.WritePrimitive(node.AsValue());
			break;
		case JsonValueKind.Number:
			writer.WritePrimitive(node.AsValue());
			break;
		case JsonValueKind.True:
		case JsonValueKind.False:
			writer.WritePrimitive(node.AsValue());
			break;
		case JsonValueKind.Null:
			writer.WriteNull();
			break;
		}
	}

	private static void WriteArray(this IOpenApiWriter writer, JsonArray? array)
	{
		writer.WriteStartArray();
		if (array != null)
		{
			foreach (JsonNode item in array)
			{
				writer.WriteAny(item);
			}
		}
		writer.WriteEndArray();
	}

	private static void WriteObject(this IOpenApiWriter writer, JsonObject? entity)
	{
		writer.WriteStartObject();
		if (entity != null)
		{
			foreach (KeyValuePair<string, JsonNode> item in entity)
			{
				writer.WritePropertyName(item.Key);
				writer.WriteAny(item.Value);
			}
		}
		writer.WriteEndObject();
	}

	private static void WritePrimitive(this IOpenApiWriter writer, JsonValue jsonValue)
	{
		DateTime value2;
		DateTimeOffset value3;
		DateOnly value4;
		TimeOnly value5;
		bool value6;
		decimal value7;
		double value8;
		float value9;
		long value10;
		int value11;
		if (jsonValue.TryGetValue<string>(out string value) && value != null)
		{
			writer.WriteValue(value);
		}
		else if (jsonValue.TryGetValue<DateTime>(out value2))
		{
			writer.WriteValue(value2.ToString("o", CultureInfo.InvariantCulture));
		}
		else if (jsonValue.TryGetValue<DateTimeOffset>(out value3))
		{
			writer.WriteValue(value3.ToString("o", CultureInfo.InvariantCulture));
		}
		else if (jsonValue.TryGetValue<DateOnly>(out value4))
		{
			writer.WriteValue(value4.ToString("o", CultureInfo.InvariantCulture));
		}
		else if (jsonValue.TryGetValue<TimeOnly>(out value5))
		{
			writer.WriteValue(value5.ToString("o", CultureInfo.InvariantCulture));
		}
		else if (jsonValue.TryGetValue<bool>(out value6))
		{
			writer.WriteValue(value6);
		}
		else if (jsonValue.TryGetValue<decimal>(out value7))
		{
			writer.WriteValue(value7);
		}
		else if (jsonValue.TryGetValue<double>(out value8))
		{
			writer.WriteValue(value8);
		}
		else if (jsonValue.TryGetValue<float>(out value9))
		{
			writer.WriteValue(value9);
		}
		else if (jsonValue.TryGetValue<long>(out value10))
		{
			writer.WriteValue(value10);
		}
		else if (jsonValue.TryGetValue<int>(out value11))
		{
			writer.WriteValue(value11);
		}
	}
}
