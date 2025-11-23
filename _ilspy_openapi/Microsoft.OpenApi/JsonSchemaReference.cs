using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Schema reference information that includes metadata annotations from JSON Schema 2020-12.
/// This class extends OpenApiReference to provide schema-specific metadata override capabilities.
/// </summary>
public class JsonSchemaReference : OpenApiReferenceWithDescription
{
	/// <summary>
	/// A default value which by default SHOULD override that of the referenced component.
	/// If the referenced object-type does not allow a default field, then this field has no effect.
	/// You must use the <see cref="M:Microsoft.OpenApi.JsonNullSentinel.IsJsonNullSentinel(System.Text.Json.Nodes.JsonNode)" /> method to check whether Default was assigned a null value in the document.
	/// Assign <see cref="P:Microsoft.OpenApi.JsonNullSentinel.JsonNull" /> to use get null as a serialized value.
	/// </summary>
	public JsonNode? Default { get; set; }

	/// <summary>
	/// A title which by default SHOULD override that of the referenced component.
	/// If the referenced object-type does not allow a title field, then this field has no effect.
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// Indicates whether the referenced component is deprecated.
	/// If the referenced object-type does not allow a deprecated field, then this field has no effect.
	/// </summary>
	public bool? Deprecated { get; set; }

	/// <summary>
	/// Indicates whether the referenced component is read-only.
	/// If the referenced object-type does not allow a readOnly field, then this field has no effect.
	/// </summary>
	public bool? ReadOnly { get; set; }

	/// <summary>
	/// Indicates whether the referenced component is write-only.
	/// If the referenced object-type does not allow a writeOnly field, then this field has no effect.
	/// </summary>
	public bool? WriteOnly { get; set; }

	/// <summary>
	/// Example values which by default SHOULD override those of the referenced component.
	/// If the referenced object-type does not allow examples, then this field has no effect.
	/// </summary>
	public IList<JsonNode>? Examples { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public JsonSchemaReference()
	{
	}

	/// <summary>
	/// Initializes a copy instance of the <see cref="T:Microsoft.OpenApi.JsonSchemaReference" /> object
	/// </summary>
	public JsonSchemaReference(JsonSchemaReference reference)
		: base(reference)
	{
		Utils.CheckArgumentNull(reference, "reference");
		Default = reference.Default;
		Title = reference.Title;
		Deprecated = reference.Deprecated;
		ReadOnly = reference.ReadOnly;
		WriteOnly = reference.WriteOnly;
		Examples = reference.Examples;
	}

	/// <inheritdoc />
	protected override void SerializeAdditionalV31Properties(IOpenApiWriter writer)
	{
		SerializeAdditionalV3XProperties(writer, base.SerializeAdditionalV31Properties);
	}

	/// <inheritdoc />
	protected override void SerializeAdditionalV32Properties(IOpenApiWriter writer)
	{
		SerializeAdditionalV3XProperties(writer, base.SerializeAdditionalV32Properties);
	}

	private void SerializeAdditionalV3XProperties(IOpenApiWriter writer, Action<IOpenApiWriter> baseSerializer)
	{
		if (base.Type != ReferenceType.Schema)
		{
			throw new InvalidOperationException($"JsonSchemaReference can only be serialized for ReferenceType.Schema, but was {base.Type}.");
		}
		baseSerializer(writer);
		writer.WriteOptionalObject("default", Default, delegate(IOpenApiWriter w, JsonNode d)
		{
			w.WriteAny(d);
		});
		writer.WriteProperty("title", Title);
		if (Deprecated.HasValue)
		{
			writer.WriteProperty("deprecated", Deprecated.Value);
		}
		if (ReadOnly.HasValue)
		{
			writer.WriteProperty("readOnly", ReadOnly.Value);
		}
		if (WriteOnly.HasValue)
		{
			writer.WriteProperty("writeOnly", WriteOnly.Value);
		}
		if (Examples != null && Examples.Any())
		{
			writer.WriteOptionalCollection("examples", Examples, delegate(IOpenApiWriter w, JsonNode e)
			{
				w.WriteAny(e);
			});
		}
	}

	/// <inheritdoc />
	protected override void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
	{
		base.SetAdditional31MetadataFromMapNode(jsonObject);
		string propertyValueFromNode = BaseOpenApiReference.GetPropertyValueFromNode(jsonObject, "title");
		if (!string.IsNullOrEmpty(propertyValueFromNode))
		{
			Title = propertyValueFromNode;
		}
		if (jsonObject.TryGetPropertyValue("deprecated", out JsonNode jsonNode) && jsonNode is JsonValue jsonValue && jsonValue.TryGetValue<bool>(out var value))
		{
			Deprecated = value;
		}
		if (jsonObject.TryGetPropertyValue("readOnly", out JsonNode jsonNode2) && jsonNode2 is JsonValue jsonValue2 && jsonValue2.TryGetValue<bool>(out var value2))
		{
			ReadOnly = value2;
		}
		if (jsonObject.TryGetPropertyValue("writeOnly", out JsonNode jsonNode3) && jsonNode3 is JsonValue jsonValue3 && jsonValue3.TryGetValue<bool>(out var value3))
		{
			WriteOnly = value3;
		}
		if (jsonObject.TryGetPropertyValue("default", out JsonNode jsonNode4))
		{
			Default = jsonNode4;
		}
		if (jsonObject.TryGetPropertyValue("examples", out JsonNode jsonNode5) && jsonNode5 is JsonArray source)
		{
			Examples = source.OfType<JsonNode>().ToList();
		}
	}
}
