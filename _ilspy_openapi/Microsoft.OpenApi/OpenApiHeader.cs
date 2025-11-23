using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Header Object.
/// The Header Object follows the structure of the Parameter Object.
/// </summary>
public class OpenApiHeader : IOpenApiHeader, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiHeader>, IOpenApiReferenceable, IOpenApiSerializable, IOpenApiExtensible
{
	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public bool Required { get; set; }

	/// <inheritdoc />
	public bool Deprecated { get; set; }

	/// <inheritdoc />
	public bool AllowEmptyValue { get; set; }

	/// <inheritdoc />
	public ParameterStyle? Style { get; set; }

	/// <inheritdoc />
	public bool Explode { get; set; }

	/// <inheritdoc />
	public bool AllowReserved { get; set; }

	/// <inheritdoc />
	public IOpenApiSchema? Schema { get; set; }

	/// <inheritdoc />
	public JsonNode? Example { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExample>? Examples { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiMediaType>? Content { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiHeader()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiHeader" /> object
	/// </summary>
	internal OpenApiHeader(IOpenApiHeader header)
	{
		Utils.CheckArgumentNull(header, "header");
		Description = header.Description ?? Description;
		Required = header.Required;
		Deprecated = header.Deprecated;
		AllowEmptyValue = header.AllowEmptyValue;
		Style = header.Style ?? Style;
		Explode = header.Explode;
		AllowReserved = header.AllowReserved;
		Schema = header.Schema?.CreateShallowCopy();
		Example = ((header.Example != null) ? JsonNodeCloneHelper.Clone(header.Example) : null);
		Examples = ((header.Examples != null) ? new Dictionary<string, IOpenApiExample>(header.Examples) : null);
		Content = ((header.Content != null) ? new Dictionary<string, IOpenApiMediaType>(header.Content) : null);
		Extensions = ((header.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(header.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiHeader" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiHeader" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiHeader" /> to Open Api v3.0
	/// </summary>
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
		writer.WriteStartObject();
		writer.WriteProperty("description", Description);
		writer.WriteProperty("required", Required);
		writer.WriteProperty("deprecated", Deprecated);
		writer.WriteProperty("allowEmptyValue", AllowEmptyValue);
		ParameterStyle? style = Style;
		writer.WriteProperty("style", style.HasValue ? style.GetValueOrDefault().GetDisplayName() : null);
		writer.WriteProperty("explode", Explode);
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
	/// Serialize to OpenAPI V2 document without using reference.
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("description", Description);
		writer.WriteProperty("required", Required);
		writer.WriteProperty("deprecated", Deprecated);
		writer.WriteProperty("allowEmptyValue", AllowEmptyValue);
		ParameterStyle? style = Style;
		writer.WriteProperty("style", style.HasValue ? style.GetValueOrDefault().GetDisplayName() : null);
		writer.WriteProperty("explode", Explode);
		writer.WriteProperty("allowReserved", AllowReserved);
		IOpenApiSchema schema = Schema;
		((schema is OpenApiSchemaReference openApiSchemaReference) ? openApiSchemaReference.RecursiveTarget : ((!(schema is OpenApiSchema openApiSchema)) ? null : openApiSchema))?.WriteAsItemsProperties(writer);
		writer.WriteOptionalObject("example", Example, delegate(IOpenApiWriter w, JsonNode s)
		{
			w.WriteAny(s);
		});
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public IOpenApiHeader CreateShallowCopy()
	{
		return new OpenApiHeader(this);
	}
}
