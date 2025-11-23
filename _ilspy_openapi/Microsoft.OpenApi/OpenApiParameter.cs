using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Parameter Object.
/// </summary>
public class OpenApiParameter : IOpenApiExtensible, IOpenApiElement, IOpenApiParameter, IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiParameter>, IOpenApiReferenceable, IOpenApiSerializable
{
	private bool? _explode;

	private ParameterStyle? _style;

	/// <inheritdoc />
	public string? Name { get; set; }

	/// <inheritdoc />
	public ParameterLocation? In { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public bool Required { get; set; }

	/// <inheritdoc />
	public bool Deprecated { get; set; }

	/// <inheritdoc />
	public bool AllowEmptyValue { get; set; }

	/// <inheritdoc />
	public ParameterStyle? Style
	{
		get
		{
			return _style ?? GetDefaultStyleValue();
		}
		set
		{
			_style = value;
		}
	}

	/// <inheritdoc />
	public bool Explode
	{
		get
		{
			bool? explode = _explode;
			if (explode.HasValue)
			{
				return explode == true;
			}
			bool result;
			switch (Style)
			{
			case ParameterStyle.Form:
			case ParameterStyle.Cookie:
				result = true;
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
		set
		{
			_explode = value;
		}
	}

	/// <inheritdoc />
	public bool AllowReserved { get; set; }

	/// <inheritdoc />
	public IOpenApiSchema? Schema { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExample>? Examples { get; set; }

	/// <inheritdoc />
	public JsonNode? Example { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiMediaType>? Content { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// A parameterless constructor
	/// </summary>
	public OpenApiParameter()
	{
	}

	/// <summary>
	/// Initializes a clone instance of <see cref="T:Microsoft.OpenApi.OpenApiParameter" /> object
	/// </summary>
	internal OpenApiParameter(IOpenApiParameter parameter)
	{
		Utils.CheckArgumentNull(parameter, "parameter");
		Name = parameter.Name ?? Name;
		In = parameter.In ?? In;
		Description = parameter.Description ?? Description;
		Required = parameter.Required;
		Style = parameter.Style ?? Style;
		Explode = parameter.Explode;
		AllowReserved = parameter.AllowReserved;
		Schema = parameter.Schema?.CreateShallowCopy();
		Examples = ((parameter.Examples != null) ? new Dictionary<string, IOpenApiExample>(parameter.Examples) : null);
		Example = ((parameter.Example != null) ? JsonNodeCloneHelper.Clone(parameter.Example) : null);
		Content = ((parameter.Content != null) ? new Dictionary<string, IOpenApiMediaType>(parameter.Content) : null);
		Extensions = ((parameter.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(parameter.Extensions) : null);
		AllowEmptyValue = parameter.AllowEmptyValue;
		Deprecated = parameter.Deprecated;
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

	internal void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (Style == ParameterStyle.Cookie && version < OpenApiSpecVersion.OpenApi3_2)
		{
			throw new OpenApiException($"Parameter style 'cookie' is only supported in OpenAPI 3.2 and later versions. Current version: {version}");
		}
		if (In == ParameterLocation.QueryString)
		{
			if (version < OpenApiSpecVersion.OpenApi3_2)
			{
				throw new InvalidOperationException("Parameter location 'querystring' is only supported in OpenAPI 3.2.0 and above.");
			}
			if (_style.HasValue || (_explode.HasValue && _explode.Value) || AllowReserved || Schema != null)
			{
				throw new InvalidOperationException("When 'in' is 'querystring', 'style', 'explode', 'allowReserved', and 'schema' properties MUST NOT be used as per OpenAPI 3.2 specification.");
			}
		}
		writer.WriteStartObject();
		writer.WriteProperty("name", Name);
		ParameterLocation? parameterLocation = In;
		writer.WriteProperty("in", parameterLocation.HasValue ? parameterLocation.GetValueOrDefault().GetDisplayName() : null);
		writer.WriteProperty("description", Description);
		writer.WriteProperty("required", Required);
		writer.WriteProperty("deprecated", Deprecated);
		writer.WriteProperty("allowEmptyValue", AllowEmptyValue);
		if (Style.HasValue && Style != GetDefaultStyleValue())
		{
			writer.WriteProperty("style", Style.Value.GetDisplayName());
		}
		bool? explode = _explode;
		bool defaultValue;
		switch (Style)
		{
		case ParameterStyle.Form:
		case ParameterStyle.Cookie:
			defaultValue = true;
			break;
		default:
			defaultValue = false;
			break;
		}
		writer.WriteProperty("explode", explode, defaultValue);
		writer.WriteProperty("allowReserved", AllowReserved);
		writer.WriteOptionalObject("schema", Schema, callback);
		writer.WriteOptionalObject("example", Example, delegate(IOpenApiWriter w, JsonNode s)
		{
			w.WriteAny(s);
		});
		writer.WriteOptionalMap("examples", Examples, callback);
		writer.WriteOptionalMap("content", Content, callback);
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Write the "in" property for V2 serialization.
	/// </summary>
	/// <param name="writer">Writer to use for the serialization</param>
	internal virtual void WriteInPropertyForV2(IOpenApiWriter writer)
	{
		ParameterLocation? parameterLocation = In;
		writer.WriteProperty("in", parameterLocation.HasValue ? parameterLocation.GetValueOrDefault().GetDisplayName() : null);
	}

	/// <summary>
	/// Write the request body schema for V2 serialization.
	/// </summary>
	/// <param name="writer">Writer to use for the serialization</param>
	/// <param name="extensionsClone">Extensions clone</param>
	internal virtual void WriteRequestBodySchemaForV2(IOpenApiWriter writer, Dictionary<string, IOpenApiExtension>? extensionsClone)
	{
		if (!(Schema is OpenApiSchemaReference { UnresolvedReference: not false }))
		{
			IOpenApiSchema? schema = Schema;
			if (schema == null || ((uint?)schema.Type & 0x20u) != 32)
			{
				IOpenApiSchema schema2 = Schema;
				OpenApiSchema openApiSchema = ((schema2 is OpenApiSchemaReference openApiSchemaReference2) ? openApiSchemaReference2.RecursiveTarget : ((!(schema2 is OpenApiSchema openApiSchema2)) ? null : openApiSchema2));
				OpenApiSchema openApiSchema3 = openApiSchema;
				if (openApiSchema3 != null)
				{
					openApiSchema3.WriteAsItemsProperties(writer);
					IDictionary<string, IOpenApiExtension> dictionary = Schema?.Extensions;
					if (dictionary != null)
					{
						foreach (string key in dictionary.Keys)
						{
							extensionsClone?.Remove(key);
						}
					}
				}
				writer.WriteProperty("allowEmptyValue", AllowEmptyValue);
				if (In != ParameterLocation.Query)
				{
					return;
				}
				IOpenApiSchema? schema3 = Schema;
				if (schema3 != null && schema3.Type == JsonSchemaType.Array)
				{
					if (Style == ParameterStyle.Form && Explode)
					{
						writer.WriteProperty("collectionFormat", "multi");
					}
					else if (Style == ParameterStyle.PipeDelimited)
					{
						writer.WriteProperty("collectionFormat", "pipes");
					}
					else if (Style == ParameterStyle.SpaceDelimited)
					{
						writer.WriteProperty("collectionFormat", "ssv");
					}
				}
				return;
			}
		}
		writer.WriteProperty("type", "string");
	}

	/// <inheritdoc />
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (Style == ParameterStyle.Cookie)
		{
			throw new OpenApiException($"Parameter style 'cookie' is only supported in OpenAPI 3.2 and later versions. Current version: {0}");
		}
		if (In == ParameterLocation.QueryString)
		{
			throw new InvalidOperationException("Parameter location 'querystring' is not supported in OpenAPI 2.0.");
		}
		writer.WriteStartObject();
		WriteInPropertyForV2(writer);
		writer.WriteProperty("name", Name);
		writer.WriteProperty("description", Description);
		writer.WriteProperty("required", Required);
		writer.WriteProperty("deprecated", Deprecated);
		Dictionary<string, IOpenApiExtension> dictionary = ((Extensions != null) ? new Dictionary<string, IOpenApiExtension>(Extensions) : null);
		WriteRequestBodySchemaForV2(writer, dictionary);
		if (Examples != null && Examples.Any())
		{
			writer.WritePropertyName("x-examples");
			writer.WriteStartObject();
			foreach (KeyValuePair<string, IOpenApiExample> example in Examples)
			{
				writer.WritePropertyName(example.Key);
				example.Value.SerializeAsV2(writer);
			}
			writer.WriteEndObject();
		}
		writer.WriteExtensions(dictionary, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	internal virtual ParameterStyle? GetDefaultStyleValue()
	{
		return In switch
		{
			ParameterLocation.Query => ParameterStyle.Form, 
			ParameterLocation.Header => ParameterStyle.Simple, 
			ParameterLocation.Path => ParameterStyle.Simple, 
			ParameterLocation.Cookie => ParameterStyle.Form, 
			_ => ParameterStyle.Simple, 
		};
	}

	/// <inheritdoc />
	public IOpenApiParameter CreateShallowCopy()
	{
		return new OpenApiParameter(this);
	}
}
