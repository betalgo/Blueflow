using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Media Type Object.
/// </summary>
public class OpenApiMediaType : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible, IOpenApiMediaType, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiMediaType>, IOpenApiReferenceable
{
	/// <inheritdoc />
	public IOpenApiSchema? Schema { get; set; }

	/// <inheritdoc />
	public IOpenApiSchema? ItemSchema { get; set; }

	/// <inheritdoc />
	public JsonNode? Example { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExample>? Examples { get; set; }

	/// <inheritdoc />
	public IDictionary<string, OpenApiEncoding>? Encoding { get; set; }

	/// <inheritdoc />
	public OpenApiEncoding? ItemEncoding { get; set; }

	/// <inheritdoc />
	public IList<OpenApiEncoding>? PrefixEncoding { get; set; }

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> to Open Api v3.0.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiMediaType()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiMediaType" /> object
	/// </summary>
	public OpenApiMediaType(OpenApiMediaType? mediaType)
	{
		Schema = mediaType?.Schema?.CreateShallowCopy();
		ItemSchema = mediaType?.ItemSchema?.CreateShallowCopy();
		Example = ((mediaType?.Example != null) ? JsonNodeCloneHelper.Clone(mediaType.Example) : null);
		Examples = ((mediaType?.Examples != null) ? new Dictionary<string, IOpenApiExample>(mediaType.Examples, StringComparer.Ordinal) : null);
		Encoding = ((mediaType?.Encoding != null) ? new Dictionary<string, OpenApiEncoding>(mediaType.Encoding, StringComparer.Ordinal) : null);
		ItemEncoding = ((mediaType?.ItemEncoding != null) ? new OpenApiEncoding(mediaType.ItemEncoding) : null);
		PrefixEncoding = ((mediaType?.PrefixEncoding != null) ? new List<OpenApiEncoding>(mediaType.PrefixEncoding.Select((OpenApiEncoding e) => new OpenApiEncoding(e))) : null);
		Extensions = ((mediaType?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(mediaType.Extensions, StringComparer.Ordinal) : null);
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiMediaType" /> object
	/// </summary>
	internal OpenApiMediaType(IOpenApiMediaType mediaType)
	{
		Schema = mediaType?.Schema?.CreateShallowCopy();
		ItemSchema = mediaType?.ItemSchema?.CreateShallowCopy();
		Example = ((mediaType?.Example != null) ? JsonNodeCloneHelper.Clone(mediaType.Example) : null);
		Examples = ((mediaType?.Examples != null) ? new Dictionary<string, IOpenApiExample>(mediaType.Examples, StringComparer.Ordinal) : null);
		Encoding = ((mediaType?.Encoding != null) ? new Dictionary<string, OpenApiEncoding>(mediaType.Encoding, StringComparer.Ordinal) : null);
		ItemEncoding = ((mediaType?.ItemEncoding != null) ? new OpenApiEncoding(mediaType.ItemEncoding) : null);
		PrefixEncoding = ((mediaType?.PrefixEncoding != null) ? new List<OpenApiEncoding>(mediaType.PrefixEncoding.Select((OpenApiEncoding e) => new OpenApiEncoding(e))) : null);
		Extensions = ((mediaType?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(mediaType.Extensions, StringComparer.Ordinal) : null);
	}

	/// <inheritdoc />
	public IOpenApiMediaType CreateShallowCopy()
	{
		return new OpenApiMediaType(this);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiMediaType" /> to Open Api v3.2.
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV32(w);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiMediaType" /> to Open Api v3.1.
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV31(w);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiMediaType" /> to Open Api v3.0.
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV3(w);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiMediaType" /> to Open Api v3.0.
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteOptionalObject("schema", Schema, callback);
		if (version >= OpenApiSpecVersion.OpenApi3_2)
		{
			writer.WriteOptionalObject("itemSchema", ItemSchema, callback);
		}
		else if (version < OpenApiSpecVersion.OpenApi3_2 && ItemSchema != null)
		{
			writer.WriteOptionalObject("x-oai-itemSchema", ItemSchema, callback);
		}
		writer.WriteOptionalObject("example", Example, delegate(IOpenApiWriter w, JsonNode e)
		{
			w.WriteAny(e);
		});
		if (Examples != null && Examples.Any())
		{
			writer.WriteOptionalMap("examples", Examples, callback);
		}
		writer.WriteOptionalMap("encoding", Encoding, callback);
		if (ItemEncoding != null)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteOptionalObject("itemEncoding", ItemEncoding, callback);
			}
			else
			{
				writer.WriteOptionalObject("x-oai-itemEncoding", ItemEncoding, callback);
			}
		}
		if (PrefixEncoding != null)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteOptionalCollection("prefixEncoding", PrefixEncoding, callback);
			}
			else
			{
				writer.WriteOptionalCollection("x-oai-prefixEncoding", PrefixEncoding, callback);
			}
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiMediaType" /> to Open Api v2.0.
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
	}
}
