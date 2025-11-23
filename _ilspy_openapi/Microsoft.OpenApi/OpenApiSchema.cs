using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// The Schema Object allows the definition of input and output data types.
/// </summary>
public class OpenApiSchema : IOpenApiExtensible, IOpenApiElement, IOpenApiSchema, IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiSchema>, IOpenApiReferenceable, IOpenApiSerializable, IMetadataContainer
{
	private string? _exclusiveMaximum;

	private string? _exclusiveMinimum;

	private string? _maximum;

	private string? _minimum;

	private static readonly Array jsonSchemaTypeValues = System.Enum.GetValues<JsonSchemaType>();

	/// <inheritdoc />
	public string? Title { get; set; }

	/// <inheritdoc />
	public Uri? Schema { get; set; }

	/// <inheritdoc />
	public string? Id { get; set; }

	/// <inheritdoc />
	public string? Comment { get; set; }

	/// <inheritdoc />
	public IDictionary<string, bool>? Vocabulary { get; set; }

	/// <inheritdoc />
	public string? DynamicRef { get; set; }

	/// <inheritdoc />
	public string? DynamicAnchor { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiSchema>? Definitions { get; set; }

	/// <inheritdoc />
	public string? ExclusiveMaximum
	{
		get
		{
			if (!string.IsNullOrEmpty(_exclusiveMaximum))
			{
				return _exclusiveMaximum;
			}
			if (IsExclusiveMaximum == true && !string.IsNullOrEmpty(_maximum))
			{
				return _maximum;
			}
			return null;
		}
		set
		{
			_exclusiveMaximum = value;
			IsExclusiveMaximum = value != null;
		}
	}

	/// <summary>
	/// Compatibility property for OpenAPI 3.0 or earlier serialization of the exclusive maximum value.
	/// </summary>
	/// DO NOT CHANGE THE VISIBILITY OF THIS PROPERTY TO PUBLIC
	internal bool? IsExclusiveMaximum { get; set; }

	/// <inheritdoc />
	public string? ExclusiveMinimum
	{
		get
		{
			if (!string.IsNullOrEmpty(_exclusiveMinimum))
			{
				return _exclusiveMinimum;
			}
			if (IsExclusiveMinimum == true && !string.IsNullOrEmpty(_minimum))
			{
				return _minimum;
			}
			return null;
		}
		set
		{
			_exclusiveMinimum = value;
			IsExclusiveMinimum = value != null;
		}
	}

	/// <summary>
	/// Compatibility property for OpenAPI 3.0 or earlier serialization of the exclusive minimum value.
	/// </summary>
	/// DO NOT CHANGE THE VISIBILITY OF THIS PROPERTY TO PUBLIC
	internal bool? IsExclusiveMinimum { get; set; }

	/// <inheritdoc />
	public JsonSchemaType? Type { get; set; }

	private bool IsNullable
	{
		get
		{
			if (!Type.HasValue || !Type.Value.HasFlag(JsonSchemaType.Null))
			{
				if (Extensions != null && Extensions.TryGetValue("x-nullable", out IOpenApiExtension value) && value is JsonNodeExtension jsonNodeExtension)
				{
					JsonNode node = jsonNodeExtension.Node;
					if (node != null)
					{
						return node.GetValueKind() == JsonValueKind.True;
					}
				}
				return false;
			}
			return true;
		}
	}

	/// <inheritdoc />
	public string? Const { get; set; }

	/// <inheritdoc />
	public string? Format { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public string? Maximum
	{
		get
		{
			if (IsExclusiveMaximum == true)
			{
				return null;
			}
			return _maximum;
		}
		set
		{
			_maximum = value;
		}
	}

	/// <inheritdoc />
	public string? Minimum
	{
		get
		{
			if (IsExclusiveMinimum == true)
			{
				return null;
			}
			return _minimum;
		}
		set
		{
			_minimum = value;
		}
	}

	/// <inheritdoc />
	public int? MaxLength { get; set; }

	/// <inheritdoc />
	public int? MinLength { get; set; }

	/// <inheritdoc />
	public string? Pattern { get; set; }

	/// <inheritdoc />
	public decimal? MultipleOf { get; set; }

	/// <inheritdoc />
	public JsonNode? Default { get; set; }

	/// <inheritdoc />
	public bool ReadOnly { get; set; }

	/// <inheritdoc />
	public bool WriteOnly { get; set; }

	/// <inheritdoc />
	public IList<IOpenApiSchema>? AllOf { get; set; }

	/// <inheritdoc />
	public IList<IOpenApiSchema>? OneOf { get; set; }

	/// <inheritdoc />
	public IList<IOpenApiSchema>? AnyOf { get; set; }

	/// <inheritdoc />
	public IOpenApiSchema? Not { get; set; }

	/// <inheritdoc />
	public ISet<string>? Required { get; set; }

	/// <inheritdoc />
	public IOpenApiSchema? Items { get; set; }

	/// <inheritdoc />
	public int? MaxItems { get; set; }

	/// <inheritdoc />
	public int? MinItems { get; set; }

	/// <inheritdoc />
	public bool? UniqueItems { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiSchema>? Properties { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiSchema>? PatternProperties { get; set; }

	/// <inheritdoc />
	public int? MaxProperties { get; set; }

	/// <inheritdoc />
	public int? MinProperties { get; set; }

	/// <inheritdoc />
	public bool AdditionalPropertiesAllowed { get; set; } = true;

	/// <inheritdoc />
	public IOpenApiSchema? AdditionalProperties { get; set; }

	/// <inheritdoc />
	public OpenApiDiscriminator? Discriminator { get; set; }

	/// <inheritdoc />
	public JsonNode? Example { get; set; }

	/// <inheritdoc />
	public IList<JsonNode>? Examples { get; set; }

	/// <inheritdoc />
	public IList<JsonNode>? Enum { get; set; }

	/// <inheritdoc />
	public bool UnevaluatedProperties { get; set; }

	/// <inheritdoc />
	public OpenApiExternalDocs? ExternalDocs { get; set; }

	/// <inheritdoc />
	public bool Deprecated { get; set; }

	/// <inheritdoc />
	public OpenApiXml? Xml { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <inheritdoc />
	public IDictionary<string, JsonNode>? UnrecognizedKeywords { get; set; }

	/// <inheritdoc />
	public IDictionary<string, object>? Metadata { get; set; }

	/// <inheritdoc />
	public IDictionary<string, HashSet<string>>? DependentRequired { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiSchema()
	{
	}

	/// <summary>
	/// Initializes a copy of <see cref="T:Microsoft.OpenApi.IOpenApiSchema" /> object
	/// </summary>
	/// <param name="schema">The schema object to copy from.</param>
	internal OpenApiSchema(IOpenApiSchema schema)
	{
		Utils.CheckArgumentNull(schema, "schema");
		Title = schema.Title ?? Title;
		Id = schema.Id ?? Id;
		Const = schema.Const ?? Const;
		Schema = schema.Schema ?? Schema;
		Comment = schema.Comment ?? Comment;
		Vocabulary = ((schema.Vocabulary != null) ? new Dictionary<string, bool>(schema.Vocabulary) : null);
		DynamicAnchor = schema.DynamicAnchor ?? DynamicAnchor;
		DynamicRef = schema.DynamicRef ?? DynamicRef;
		Definitions = ((schema.Definitions != null) ? new Dictionary<string, IOpenApiSchema>(schema.Definitions) : null);
		UnevaluatedProperties = schema.UnevaluatedProperties;
		ExclusiveMaximum = schema.ExclusiveMaximum ?? ExclusiveMaximum;
		ExclusiveMinimum = schema.ExclusiveMinimum ?? ExclusiveMinimum;
		if (schema is OpenApiSchema openApiSchema)
		{
			IsExclusiveMaximum = openApiSchema.IsExclusiveMaximum;
			IsExclusiveMinimum = openApiSchema.IsExclusiveMinimum;
		}
		Type = schema.Type ?? Type;
		Format = schema.Format ?? Format;
		Description = schema.Description ?? Description;
		Maximum = schema.Maximum ?? Maximum;
		Minimum = schema.Minimum ?? Minimum;
		MaxLength = schema.MaxLength ?? MaxLength;
		MinLength = schema.MinLength ?? MinLength;
		Pattern = schema.Pattern ?? Pattern;
		MultipleOf = schema.MultipleOf ?? MultipleOf;
		Default = ((schema.Default != null) ? JsonNodeCloneHelper.Clone(schema.Default) : null);
		ReadOnly = schema.ReadOnly;
		WriteOnly = schema.WriteOnly;
		IList<IOpenApiSchema> allOf2;
		if (schema.AllOf != null)
		{
			IList<IOpenApiSchema>? allOf = schema.AllOf;
			int count = allOf.Count;
			List<IOpenApiSchema> list = new List<IOpenApiSchema>(count);
			CollectionsMarshal.SetCount(list, count);
			Span<IOpenApiSchema> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			foreach (IOpenApiSchema item in allOf)
			{
				span[num] = item;
				num++;
			}
			allOf2 = list;
		}
		else
		{
			allOf2 = null;
		}
		AllOf = allOf2;
		if (schema.OneOf != null)
		{
			IList<IOpenApiSchema>? oneOf = schema.OneOf;
			int num = oneOf.Count;
			List<IOpenApiSchema> list = new List<IOpenApiSchema>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<IOpenApiSchema> span = CollectionsMarshal.AsSpan(list);
			int count = 0;
			foreach (IOpenApiSchema item2 in oneOf)
			{
				span[count] = item2;
				count++;
			}
			allOf2 = list;
		}
		else
		{
			allOf2 = null;
		}
		OneOf = allOf2;
		if (schema.AnyOf != null)
		{
			IList<IOpenApiSchema>? anyOf = schema.AnyOf;
			int count = anyOf.Count;
			List<IOpenApiSchema> list = new List<IOpenApiSchema>(count);
			CollectionsMarshal.SetCount(list, count);
			Span<IOpenApiSchema> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			foreach (IOpenApiSchema item3 in anyOf)
			{
				span[num] = item3;
				num++;
			}
			allOf2 = list;
		}
		else
		{
			allOf2 = null;
		}
		AnyOf = allOf2;
		Not = schema.Not?.CreateShallowCopy();
		Required = ((schema.Required != null) ? new HashSet<string>(schema.Required) : null);
		Items = schema.Items?.CreateShallowCopy();
		MaxItems = schema.MaxItems ?? MaxItems;
		MinItems = schema.MinItems ?? MinItems;
		UniqueItems = schema.UniqueItems ?? UniqueItems;
		Properties = ((schema.Properties != null) ? new Dictionary<string, IOpenApiSchema>(schema.Properties) : null);
		PatternProperties = ((schema.PatternProperties != null) ? new Dictionary<string, IOpenApiSchema>(schema.PatternProperties) : null);
		MaxProperties = schema.MaxProperties ?? MaxProperties;
		MinProperties = schema.MinProperties ?? MinProperties;
		AdditionalPropertiesAllowed = schema.AdditionalPropertiesAllowed;
		AdditionalProperties = schema.AdditionalProperties?.CreateShallowCopy();
		Discriminator = ((schema.Discriminator != null) ? new OpenApiDiscriminator(schema.Discriminator) : null);
		Example = ((schema.Example != null) ? JsonNodeCloneHelper.Clone(schema.Example) : null);
		IList<JsonNode> examples2;
		if (schema.Examples != null)
		{
			IList<JsonNode>? examples = schema.Examples;
			int num = examples.Count;
			List<JsonNode> list2 = new List<JsonNode>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<JsonNode> span2 = CollectionsMarshal.AsSpan(list2);
			int count = 0;
			foreach (JsonNode item4 in examples)
			{
				span2[count] = item4;
				count++;
			}
			examples2 = list2;
		}
		else
		{
			examples2 = null;
		}
		Examples = examples2;
		if (schema.Enum != null)
		{
			IList<JsonNode>? list3 = schema.Enum;
			int count = list3.Count;
			List<JsonNode> list2 = new List<JsonNode>(count);
			CollectionsMarshal.SetCount(list2, count);
			Span<JsonNode> span2 = CollectionsMarshal.AsSpan(list2);
			int num = 0;
			foreach (JsonNode item5 in list3)
			{
				span2[num] = item5;
				num++;
			}
			examples2 = list2;
		}
		else
		{
			examples2 = null;
		}
		Enum = examples2;
		ExternalDocs = ((schema.ExternalDocs != null) ? new OpenApiExternalDocs(schema.ExternalDocs) : null);
		Deprecated = schema.Deprecated;
		Xml = ((schema.Xml != null) ? new OpenApiXml(schema.Xml) : null);
		Extensions = ((schema.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(schema.Extensions) : null);
		Metadata = ((schema is IMetadataContainer { Metadata: not null } metadataContainer) ? new Dictionary<string, object>(metadataContainer.Metadata) : null);
		UnrecognizedKeywords = ((schema.UnrecognizedKeywords != null) ? new Dictionary<string, JsonNode>(schema.UnrecognizedKeywords) : null);
		DependentRequired = ((schema.DependentRequired != null) ? new Dictionary<string, HashSet<string>>(schema.DependentRequired) : null);
	}

	/// <inheritdoc />
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <inheritdoc />
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <inheritdoc />
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	private static void SerializeBounds(IOpenApiWriter writer, OpenApiSpecVersion version, string propertyName, string exclusivePropertyName, string isExclusivePropertyName, string? value, string? exclusiveValue, bool? isExclusiveValue)
	{
		if (version >= OpenApiSpecVersion.OpenApi3_1)
		{
			if (!string.IsNullOrEmpty(exclusiveValue) && exclusiveValue != null)
			{
				writer.WritePropertyName(exclusivePropertyName);
				writer.WriteRaw(exclusiveValue);
			}
			else if (isExclusiveValue == true && !string.IsNullOrEmpty(value) && value != null)
			{
				writer.WritePropertyName(exclusivePropertyName);
				writer.WriteRaw(value);
			}
			else if (!string.IsNullOrEmpty(value) && value != null)
			{
				writer.WritePropertyName(propertyName);
				writer.WriteRaw(value);
			}
		}
		else if (!string.IsNullOrEmpty(exclusiveValue) && exclusiveValue != null)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteRaw(exclusiveValue);
			writer.WriteProperty<bool>(isExclusivePropertyName, value: true);
		}
		else if (!string.IsNullOrEmpty(value) && value != null)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteRaw(value);
			if (isExclusiveValue.HasValue)
			{
				writer.WriteProperty<bool>(isExclusivePropertyName, isExclusiveValue.Value);
			}
		}
	}

	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		writer.WriteStartObject();
		if (version >= OpenApiSpecVersion.OpenApi3_1)
		{
			WriteJsonSchemaKeywords(writer);
		}
		writer.WriteProperty("title", Title);
		writer.WriteProperty("multipleOf", MultipleOf);
		SerializeBounds(writer, version, "maximum", "exclusiveMaximum", "exclusiveMaximum", Maximum, ExclusiveMaximum, IsExclusiveMaximum);
		SerializeBounds(writer, version, "minimum", "exclusiveMinimum", "exclusiveMinimum", Minimum, ExclusiveMinimum, IsExclusiveMinimum);
		writer.WriteProperty("maxLength", MaxLength);
		writer.WriteProperty("minLength", MinLength);
		writer.WriteProperty("pattern", Pattern);
		writer.WriteProperty("maxItems", MaxItems);
		writer.WriteProperty("minItems", MinItems);
		writer.WriteProperty<bool>("uniqueItems", UniqueItems);
		writer.WriteProperty("maxProperties", MaxProperties);
		writer.WriteProperty("minProperties", MinProperties);
		writer.WriteOptionalCollection("required", Required, delegate(IOpenApiWriter w, string? s)
		{
			if (!string.IsNullOrEmpty(s) && s != null)
			{
				w.WriteValue(s);
			}
		});
		IList<JsonNode> list = Enum;
		IList<JsonNode> list2;
		if ((list != null && list.Count > 0) || string.IsNullOrEmpty(Const) || version >= OpenApiSpecVersion.OpenApi3_1)
		{
			list2 = Enum;
		}
		else
		{
			list = new List<JsonNode> { JsonValue.Create(Const) };
			list2 = list;
		}
		IList<JsonNode> elements = list2;
		writer.WriteOptionalCollection("enum", elements, delegate(IOpenApiWriter nodeWriter, JsonNode s)
		{
			nodeWriter.WriteAny(s);
		});
		SerializeTypeProperty(writer, version);
		writer.WriteOptionalCollection("allOf", AllOf, callback);
		writer.WriteOptionalCollection("anyOf", AnyOf, callback);
		writer.WriteOptionalCollection("oneOf", OneOf, callback);
		writer.WriteOptionalObject("not", Not, callback);
		writer.WriteOptionalObject("items", Items, callback);
		writer.WriteOptionalMap("properties", Properties, callback);
		if (AdditionalPropertiesAllowed)
		{
			writer.WriteOptionalObject("additionalProperties", AdditionalProperties, callback);
		}
		else
		{
			writer.WriteProperty<bool>("additionalProperties", AdditionalPropertiesAllowed);
		}
		writer.WriteProperty("description", Description);
		writer.WriteProperty("format", Format);
		writer.WriteOptionalObject("default", Default, delegate(IOpenApiWriter w, JsonNode d)
		{
			w.WriteAny(d);
		});
		if (version == OpenApiSpecVersion.OpenApi3_0)
		{
			SerializeNullable(writer, version);
		}
		writer.WriteOptionalObject("discriminator", Discriminator, callback);
		writer.WriteProperty("readOnly", ReadOnly);
		writer.WriteProperty("writeOnly", WriteOnly);
		writer.WriteOptionalObject("xml", Xml, callback);
		writer.WriteOptionalObject("externalDocs", ExternalDocs, callback);
		writer.WriteOptionalObject("example", Example, delegate(IOpenApiWriter w, JsonNode e)
		{
			w.WriteAny(e);
		});
		writer.WriteProperty("deprecated", Deprecated);
		writer.WriteExtensions(Extensions, version);
		if (UnrecognizedKeywords != null && UnrecognizedKeywords.Any())
		{
			writer.WriteOptionalMap("unrecognizedKeywords", UnrecognizedKeywords, delegate(IOpenApiWriter w, JsonNode s)
			{
				w.WriteAny(s);
			});
		}
		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		SerializeAsV2(writer, new HashSet<string>(), null);
	}

	internal void WriteJsonSchemaKeywords(IOpenApiWriter writer)
	{
		writer.WriteProperty("$id", Id);
		writer.WriteProperty("$schema", Schema?.ToString());
		writer.WriteProperty("$comment", Comment);
		writer.WriteProperty("const", Const);
		writer.WriteOptionalMap("$vocabulary", Vocabulary, delegate(IOpenApiWriter w, bool s)
		{
			w.WriteValue(s);
		});
		writer.WriteOptionalMap("$defs", Definitions, delegate(IOpenApiWriter w, IOpenApiSchema s)
		{
			s.SerializeAsV31(w);
		});
		writer.WriteProperty("$dynamicRef", DynamicRef);
		writer.WriteProperty("$dynamicAnchor", DynamicAnchor);
		writer.WriteProperty("unevaluatedProperties", UnevaluatedProperties);
		writer.WriteOptionalCollection("examples", Examples, delegate(IOpenApiWriter nodeWriter, JsonNode s)
		{
			nodeWriter.WriteAny(s);
		});
		writer.WriteOptionalMap("patternProperties", PatternProperties, delegate(IOpenApiWriter w, IOpenApiSchema s)
		{
			s.SerializeAsV31(w);
		});
		writer.WriteOptionalMap("dependentRequired", DependentRequired, delegate(IOpenApiWriter w, HashSet<string> s)
		{
			w.WriteValue(s);
		});
	}

	internal void WriteAsItemsProperties(IOpenApiWriter writer)
	{
		writer.WriteProperty("type", ((JsonSchemaType?)((uint?)Type & 0xFFFFFFFEu))?.ToFirstIdentifier());
		WriteFormatProperty(writer);
		writer.WriteOptionalObject("items", Items, delegate(IOpenApiWriter w, IOpenApiSchema s)
		{
			s.SerializeAsV2(w);
		});
		writer.WriteOptionalObject("default", Default, delegate(IOpenApiWriter w, JsonNode d)
		{
			w.WriteAny(d);
		});
		SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, "maximum", "exclusiveMaximum", "exclusiveMaximum", Maximum, ExclusiveMaximum, IsExclusiveMaximum);
		SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, "minimum", "exclusiveMinimum", "exclusiveMinimum", Minimum, ExclusiveMinimum, IsExclusiveMinimum);
		writer.WriteProperty("maxLength", MaxLength);
		writer.WriteProperty("minLength", MinLength);
		writer.WriteProperty("pattern", Pattern);
		writer.WriteProperty("maxItems", MaxItems);
		writer.WriteProperty("minItems", MinItems);
		writer.WriteOptionalCollection("enum", Enum, delegate(IOpenApiWriter w, JsonNode s)
		{
			w.WriteAny(s);
		});
		writer.WriteProperty("multipleOf", MultipleOf);
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
	}

	private void WriteFormatProperty(IOpenApiWriter writer)
	{
		string value = Format;
		if (string.IsNullOrEmpty(value))
		{
			value = AllOf?.FirstOrDefault((IOpenApiSchema x) => !string.IsNullOrEmpty(x.Format))?.Format ?? AnyOf?.FirstOrDefault((IOpenApiSchema x) => !string.IsNullOrEmpty(x.Format))?.Format ?? OneOf?.FirstOrDefault((IOpenApiSchema x) => !string.IsNullOrEmpty(x.Format))?.Format;
		}
		writer.WriteProperty("format", value);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSchema" /> to Open Api v2.0 and handles not marking the provided property
	/// as readonly if its included in the provided list of required properties of parent schema.
	/// </summary>
	/// <param name="writer">The open api writer.</param>
	/// <param name="parentRequiredProperties">The list of required properties in parent schema.</param>
	/// <param name="propertyName">The property name that will be serialized.</param>
	private void SerializeAsV2(IOpenApiWriter writer, ISet<string>? parentRequiredProperties, string? propertyName)
	{
		if (parentRequiredProperties == null)
		{
			parentRequiredProperties = new HashSet<string>();
		}
		writer.WriteStartObject();
		SerializeTypeProperty(writer, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteProperty("description", Description);
		WriteFormatProperty(writer);
		writer.WriteProperty("title", Title);
		writer.WriteOptionalObject("default", Default, delegate(IOpenApiWriter w, JsonNode d)
		{
			w.WriteAny(d);
		});
		writer.WriteProperty("multipleOf", MultipleOf);
		SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, "maximum", "exclusiveMaximum", "exclusiveMaximum", Maximum, ExclusiveMaximum, IsExclusiveMaximum);
		SerializeBounds(writer, OpenApiSpecVersion.OpenApi2_0, "minimum", "exclusiveMinimum", "exclusiveMinimum", Minimum, ExclusiveMinimum, IsExclusiveMinimum);
		writer.WriteProperty("maxLength", MaxLength);
		writer.WriteProperty("minLength", MinLength);
		writer.WriteProperty("pattern", Pattern);
		writer.WriteProperty("maxItems", MaxItems);
		writer.WriteProperty("minItems", MinItems);
		writer.WriteProperty<bool>("uniqueItems", UniqueItems);
		writer.WriteProperty("maxProperties", MaxProperties);
		writer.WriteProperty("minProperties", MinProperties);
		writer.WriteOptionalCollection("required", Required, delegate(IOpenApiWriter w, string? s)
		{
			if (!string.IsNullOrEmpty(s) && s != null)
			{
				w.WriteValue(s);
			}
		});
		IList<JsonNode> list = Enum;
		IList<JsonNode> list2;
		if ((list != null && list.Count > 0) || string.IsNullOrEmpty(Const))
		{
			list2 = Enum;
		}
		else
		{
			list = new List<JsonNode> { JsonValue.Create(Const) };
			list2 = list;
		}
		IList<JsonNode> elements = list2;
		writer.WriteOptionalCollection("enum", elements, delegate(IOpenApiWriter nodeWriter, JsonNode s)
		{
			nodeWriter.WriteAny(s);
		});
		writer.WriteOptionalObject("items", Items, delegate(IOpenApiWriter w, IOpenApiSchema s)
		{
			s.SerializeAsV2(w);
		});
		writer.WriteOptionalCollection("allOf", AllOf, delegate(IOpenApiWriter w, IOpenApiSchema s)
		{
			s.SerializeAsV2(w);
		});
		if (AllOf == null || AllOf.Count == 0)
		{
			writer.WriteOptionalCollection("allOf", AnyOf?.Take(1), delegate(IOpenApiWriter w, IOpenApiSchema s)
			{
				s.SerializeAsV2(w);
			});
			if (AnyOf == null || AnyOf.Count == 0)
			{
				writer.WriteOptionalCollection("allOf", OneOf?.Take(1), delegate(IOpenApiWriter w, IOpenApiSchema s)
				{
					s.SerializeAsV2(w);
				});
			}
		}
		writer.WriteOptionalMap("properties", Properties, delegate(IOpenApiWriter w, string key, IOpenApiSchema s)
		{
			if (s is OpenApiSchema openApiSchema)
			{
				openApiSchema.SerializeAsV2(w, Required, key);
			}
			else
			{
				s.SerializeAsV2(w);
			}
		});
		if (AdditionalPropertiesAllowed)
		{
			writer.WriteOptionalObject("additionalProperties", AdditionalProperties, delegate(IOpenApiWriter w, IOpenApiSchema s)
			{
				s.SerializeAsV2(w);
			});
		}
		else
		{
			writer.WriteProperty<bool>("additionalProperties", AdditionalPropertiesAllowed);
		}
		writer.WriteProperty("discriminator", Discriminator?.PropertyName);
		if (!parentRequiredProperties.Contains(propertyName ?? string.Empty))
		{
			writer.WriteProperty("readOnly", ReadOnly);
		}
		writer.WriteOptionalObject("xml", Xml, delegate(IOpenApiWriter w, OpenApiXml s)
		{
			s.SerializeAsV2(w);
		});
		writer.WriteOptionalObject("externalDocs", ExternalDocs, delegate(IOpenApiWriter w, OpenApiExternalDocs s)
		{
			s.SerializeAsV2(w);
		});
		writer.WriteOptionalObject("example", Example, delegate(IOpenApiWriter w, JsonNode e)
		{
			w.WriteAny(e);
		});
		SerializeNullable(writer, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	private void SerializeTypeProperty(IOpenApiWriter writer, OpenApiSpecVersion version)
	{
		if (!Type.HasValue)
		{
			return;
		}
		JsonSchemaType jsonSchemaType = (IsNullable ? (Type.Value | JsonSchemaType.Null) : Type.Value);
		JsonSchemaType jsonSchemaType2 = jsonSchemaType & ~JsonSchemaType.Null;
		if ((uint)version <= 1u)
		{
			if (jsonSchemaType2 != 0 && !HasMultipleTypes(jsonSchemaType2))
			{
				writer.WriteProperty("type", jsonSchemaType2.ToFirstIdentifier());
			}
		}
		else
		{
			WriteUnifiedSchemaType(jsonSchemaType, writer);
		}
	}

	private static bool IsPowerOfTwo(int x)
	{
		if (x != 0)
		{
			return (x & (x - 1)) == 0;
		}
		return false;
	}

	private static bool HasMultipleTypes(JsonSchemaType schemaType)
	{
		return !IsPowerOfTwo((int)schemaType);
	}

	private static void WriteUnifiedSchemaType(JsonSchemaType type, IOpenApiWriter writer)
	{
		string[] array = (from JsonSchemaType flag in jsonSchemaTypeValues
			where type.HasFlag(flag)
			select flag.ToFirstIdentifier()).ToArray();
		if (array.Length > 1)
		{
			writer.WriteOptionalCollection("type", array, delegate(IOpenApiWriter w, string? s)
			{
				if (!string.IsNullOrEmpty(s) && s != null)
				{
					w.WriteValue(s);
				}
			});
		}
		else
		{
			writer.WriteProperty("type", array[0]);
		}
	}

	private void SerializeNullable(IOpenApiWriter writer, OpenApiSpecVersion version)
	{
		if (IsNullable)
		{
			switch (version)
			{
			case OpenApiSpecVersion.OpenApi2_0:
				writer.WriteProperty<bool>("x-nullable", value: true);
				break;
			case OpenApiSpecVersion.OpenApi3_0:
				writer.WriteProperty<bool>("nullable", value: true);
				break;
			}
		}
	}

	/// <inheritdoc />
	public IOpenApiSchema CreateShallowCopy()
	{
		return new OpenApiSchema(this);
	}
}
