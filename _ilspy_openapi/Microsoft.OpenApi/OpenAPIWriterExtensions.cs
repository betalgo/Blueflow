using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

internal static class OpenAPIWriterExtensions
{
	/// <summary>
	/// Temporary extension method until we add Settings property to IOpenApiWriter in next major version
	/// </summary>
	/// <param name="openApiWriter"></param>
	/// <returns></returns>
	internal static OpenApiWriterSettings GetSettings(this IOpenApiWriter openApiWriter)
	{
		if (openApiWriter is OpenApiWriterBase openApiWriterBase)
		{
			return openApiWriterBase.Settings;
		}
		return new OpenApiWriterSettings();
	}
}
/// <summary>
/// Extension methods for writing Open API documentation.
/// </summary>
public static class OpenApiWriterExtensions
{
	/// <summary>
	/// Write a string property.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	public static void WriteProperty(this IOpenApiWriter writer, string name, string? value)
	{
		if (value != null)
		{
			Utils.CheckArgumentNullOrEmpty(name, "name");
			writer.WritePropertyName(name);
			writer.WriteValue(value);
		}
	}

	/// <summary>
	/// Write required string property.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	public static void WriteRequiredProperty(this IOpenApiWriter writer, string name, string? value)
	{
		Utils.CheckArgumentNullOrEmpty(name, "name");
		writer.WritePropertyName(name);
		if (value == null)
		{
			writer.WriteNull();
		}
		else
		{
			writer.WriteValue(value);
		}
	}

	/// <summary>
	/// Write a boolean property.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	/// <param name="defaultValue">The default boolean value.</param>
	public static void WriteProperty(this IOpenApiWriter writer, string name, bool value, bool defaultValue = false)
	{
		if (value != defaultValue)
		{
			Utils.CheckArgumentNullOrEmpty(name, "name");
			writer.WritePropertyName(name);
			writer.WriteValue(value);
		}
	}

	/// <summary>
	/// Write a boolean property.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	/// <param name="defaultValue">The default boolean value.</param>
	public static void WriteProperty(this IOpenApiWriter writer, string name, bool? value, bool defaultValue = false)
	{
		if (value.HasValue && value.Value != defaultValue)
		{
			Utils.CheckArgumentNullOrEmpty(name, "name");
			writer.WritePropertyName(name);
			writer.WriteValue(value.Value);
		}
	}

	/// <summary>
	/// Write a primitive property.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	public static void WriteProperty<T>(this IOpenApiWriter writer, string name, T? value) where T : struct
	{
		if (value.HasValue)
		{
			writer.WriteProperty(name, value.Value);
		}
	}

	/// <summary>
	/// Write a string/number property.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	public static void WriteProperty<T>(this IOpenApiWriter writer, string name, T value) where T : struct
	{
		Utils.CheckArgumentNullOrEmpty(name, "name");
		writer.WritePropertyName(name);
		writer.WriteValue(value);
	}

	/// <summary>
	/// Write the optional Open API object/element.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	/// <param name="action">The property value writer action.</param>
	public static void WriteOptionalObject<T>(this IOpenApiWriter writer, string name, T? value, Action<IOpenApiWriter, T> action)
	{
		if (value != null && (!(value is IEnumerable enumerable) || value is JsonArray || enumerable.GetEnumerator().MoveNext()))
		{
			writer.WriteRequiredObject(name, value, action);
		}
	}

	/// <summary>
	/// Write the required Open API object/element.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="value">The property value.</param>
	/// <param name="action">The property value writer action.</param>
	public static void WriteRequiredObject<T>(this IOpenApiWriter writer, string name, T? value, Action<IOpenApiWriter, T> action)
	{
		Utils.CheckArgumentNull(action, "action");
		writer.WritePropertyName(name);
		if (value != null)
		{
			action(writer, value);
			return;
		}
		writer.WriteStartObject();
		writer.WriteEndObject();
	}

	/// <summary>
	/// Write the optional of collection string.
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The collection values.</param>
	/// <param name="action">The collection string writer action.</param>
	public static void WriteOptionalCollection(this IOpenApiWriter writer, string name, IEnumerable<string?>? elements, Action<IOpenApiWriter, string?> action)
	{
		if (elements != null && elements.Any())
		{
			writer.WriteCollectionInternal<string>(name, elements, action);
		}
	}

	/// <summary>
	/// Write the optional Open API object/element collection.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The collection values.</param>
	/// <param name="action">The collection element writer action.</param>
	public static void WriteOptionalCollection<T>(this IOpenApiWriter writer, string name, IEnumerable<T>? elements, Action<IOpenApiWriter, T> action)
	{
		if (elements != null && elements.Any())
		{
			writer.WriteCollectionInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the optional or empty Open API object/element collection.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The collection values.</param>
	/// <param name="action">The collection element writer action.</param>
	public static void WriteOptionalOrEmptyCollection<T>(this IOpenApiWriter writer, string name, IEnumerable<T>? elements, Action<IOpenApiWriter, T> action)
	{
		if (elements != null)
		{
			writer.WriteCollectionInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the required Open API object/element collection.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The collection values.</param>
	/// <param name="action">The collection element writer action.</param>
	public static void WriteRequiredCollection<T>(this IOpenApiWriter writer, string name, IEnumerable<T> elements, Action<IOpenApiWriter, T> action) where T : IOpenApiElement
	{
		writer.WriteCollectionInternal(name, elements, action);
	}

	/// <summary>
	/// Write the required Open API element map (string to string mapping).
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action.</param>
	public static void WriteRequiredMap(this IOpenApiWriter writer, string name, IDictionary<string, string>? elements, Action<IOpenApiWriter, string> action)
	{
		writer.WriteMapInternal(name, elements, action);
	}

	/// <summary>
	/// Write the optional Open API element map (string to string mapping).
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action.</param>
	public static void WriteOptionalMap(this IOpenApiWriter writer, string name, IDictionary<string, JsonNode>? elements, Action<IOpenApiWriter, JsonNode> action)
	{
		if (elements != null && elements.Any())
		{
			writer.WriteMapInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the optional Open API element map (string to string mapping).
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action.</param>
	public static void WriteOptionalMap(this IOpenApiWriter writer, string name, IDictionary<string, string>? elements, Action<IOpenApiWriter, string> action)
	{
		if (elements != null && elements.Any())
		{
			writer.WriteMapInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the optional Open API element map (string to string mapping).
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action.</param>
	public static void WriteOptionalMap(this IOpenApiWriter writer, string name, IDictionary<string, bool>? elements, Action<IOpenApiWriter, bool> action)
	{
		if (elements != null && elements.Any())
		{
			writer.WriteMapInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the optional Open API element map (string to array mapping).
	/// </summary>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action.</param>
	public static void WriteOptionalMap(this IOpenApiWriter writer, string name, IDictionary<string, HashSet<string>>? elements, Action<IOpenApiWriter, HashSet<string>> action)
	{
		if (elements != null && elements.Any())
		{
			writer.WriteMapInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the optional Open API element map.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action with writer and value as input.</param>
	public static void WriteOptionalMap<T>(this IOpenApiWriter writer, string name, IDictionary<string, T>? elements, Action<IOpenApiWriter, T> action) where T : IOpenApiElement
	{
		if (elements != null && elements.Any())
		{
			writer.WriteMapInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the optional Open API element map.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action with writer, key, and value as input.</param>
	public static void WriteOptionalMap<T>(this IOpenApiWriter writer, string name, IDictionary<string, T>? elements, Action<IOpenApiWriter, string, T> action) where T : IOpenApiElement
	{
		if (elements != null && elements.Any())
		{
			writer.WriteMapInternal(name, elements, action);
		}
	}

	/// <summary>
	/// Write the required Open API element map.
	/// </summary>
	/// <typeparam name="T">The Open API element type. <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
	/// <param name="writer">The Open API writer.</param>
	/// <param name="name">The property name.</param>
	/// <param name="elements">The map values.</param>
	/// <param name="action">The map element writer action.</param>
	public static void WriteRequiredMap<T>(this IOpenApiWriter writer, string name, IDictionary<string, T>? elements, Action<IOpenApiWriter, T> action) where T : IOpenApiElement
	{
		writer.WriteMapInternal(name, elements, action);
	}

	private static void WriteCollectionInternal<T>(this IOpenApiWriter writer, string name, IEnumerable<T> elements, Action<IOpenApiWriter, T> action)
	{
		Utils.CheckArgumentNull(action, "action");
		writer.WritePropertyName(name);
		writer.WriteStartArray();
		if (elements != null)
		{
			foreach (T element in elements)
			{
				if (element != null)
				{
					action(writer, element);
				}
				else
				{
					writer.WriteNull();
				}
			}
		}
		writer.WriteEndArray();
	}

	private static void WriteMapInternal<T>(this IOpenApiWriter writer, string name, IDictionary<string, T>? elements, Action<IOpenApiWriter, T> action)
	{
		writer.WriteMapInternal(name, elements, delegate(IOpenApiWriter w, string _, T s)
		{
			action(w, s);
		});
	}

	private static void WriteMapInternal<T>(this IOpenApiWriter writer, string name, IDictionary<string, T>? elements, Action<IOpenApiWriter, string, T> action)
	{
		Utils.CheckArgumentNull(action, "action");
		writer.WritePropertyName(name);
		writer.WriteStartObject();
		if (elements != null)
		{
			foreach (KeyValuePair<string, T> element in elements)
			{
				writer.WritePropertyName(element.Key);
				if (element.Value != null)
				{
					action(writer, element.Key, element.Value);
				}
				else
				{
					writer.WriteNull();
				}
			}
		}
		writer.WriteEndObject();
	}
}
