using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Response object.
/// </summary>
public class OpenApiResponse : IOpenApiExtensible, IOpenApiElement, IOpenApiResponse, IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiResponse>, IOpenApiReferenceable, IOpenApiSerializable, IOpenApiSummarizedElement
{
	/// <inheritdoc />
	public string? Summary { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiHeader>? Headers { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiMediaType>? Content { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiLink>? Links { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiResponse()
	{
	}

	/// <summary>
	/// Initializes a copy of <see cref="T:Microsoft.OpenApi.IOpenApiResponse" /> object
	/// </summary>
	internal OpenApiResponse(IOpenApiResponse response)
	{
		Utils.CheckArgumentNull(response, "response");
		Summary = response.Summary ?? Summary;
		Description = response.Description ?? Description;
		Headers = ((response.Headers != null) ? new Dictionary<string, IOpenApiHeader>(response.Headers) : null);
		Content = ((response.Content != null) ? new Dictionary<string, IOpenApiMediaType>(response.Content) : null);
		Links = ((response.Links != null) ? new Dictionary<string, IOpenApiLink>(response.Links) : null);
		Extensions = ((response.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(response.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiResponse" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiResponse" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiResponse" /> to Open Api v3.0.
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		if (version >= OpenApiSpecVersion.OpenApi3_2)
		{
			writer.WriteProperty("summary", Summary);
		}
		writer.WriteRequiredProperty("description", Description);
		writer.WriteOptionalMap("headers", Headers, callback);
		writer.WriteOptionalMap("content", Content, callback);
		writer.WriteOptionalMap("links", Links, callback);
		if (version < OpenApiSpecVersion.OpenApi3_2 && !string.IsNullOrEmpty(Summary))
		{
			writer.WriteProperty("x-oai-summary", Summary);
		}
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
		writer.WriteRequiredProperty("description", Description);
		Dictionary<string, IOpenApiExtension> dictionary = ((Extensions != null) ? new Dictionary<string, IOpenApiExtension>(Extensions) : null);
		if (Content != null)
		{
			KeyValuePair<string, IOpenApiMediaType> keyValuePair = Content.FirstOrDefault();
			if (keyValuePair.Value != null)
			{
				writer.WriteOptionalObject("schema", keyValuePair.Value.Schema, delegate(IOpenApiWriter w, IOpenApiSchema s)
				{
					s.SerializeAsV2(w);
				});
				if (Content.Values.Any((IOpenApiMediaType m) => m.Example != null))
				{
					writer.WritePropertyName("examples");
					writer.WriteStartObject();
					foreach (KeyValuePair<string, IOpenApiMediaType> item in Content)
					{
						if (item.Value.Example != null)
						{
							writer.WritePropertyName(item.Key);
							writer.WriteAny(item.Value.Example);
						}
					}
					writer.WriteEndObject();
				}
				if (Content.Values.Any((IOpenApiMediaType m) => m.Examples != null && m.Examples.Any()))
				{
					writer.WritePropertyName("x-examples");
					writer.WriteStartObject();
					foreach (KeyValuePair<string, IOpenApiExample> item2 in Content.Select<KeyValuePair<string, IOpenApiMediaType>, IDictionary<string, IOpenApiExample>>((KeyValuePair<string, IOpenApiMediaType> x) => x.Value.Examples).OfType<Dictionary<string, IOpenApiExample>>().SelectMany((Dictionary<string, IOpenApiExample> x) => x))
					{
						writer.WritePropertyName(item2.Key);
						item2.Value.SerializeAsV2(writer);
					}
					writer.WriteEndObject();
				}
				writer.WriteExtensions(keyValuePair.Value.Extensions, OpenApiSpecVersion.OpenApi2_0);
				if (keyValuePair.Value.Extensions != null)
				{
					foreach (string key in keyValuePair.Value.Extensions.Keys)
					{
						dictionary?.Remove(key);
					}
				}
			}
		}
		writer.WriteOptionalMap("headers", Headers, delegate(IOpenApiWriter w, IOpenApiHeader h)
		{
			h.SerializeAsV2(w);
		});
		writer.WriteExtensions(dictionary, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public IOpenApiResponse CreateShallowCopy()
	{
		return new OpenApiResponse(this);
	}
}
