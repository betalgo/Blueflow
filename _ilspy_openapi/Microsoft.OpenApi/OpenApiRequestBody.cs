using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Request Body Object
/// </summary>
public class OpenApiRequestBody : IOpenApiExtensible, IOpenApiElement, IOpenApiRequestBody, IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiRequestBody>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public bool Required { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiMediaType>? Content { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiRequestBody()
	{
	}

	/// <summary>
	/// Initializes a copy instance of an <see cref="T:Microsoft.OpenApi.IOpenApiRequestBody" /> object
	/// </summary>
	internal OpenApiRequestBody(IOpenApiRequestBody requestBody)
	{
		Utils.CheckArgumentNull(requestBody, "requestBody");
		Description = requestBody.Description ?? Description;
		Required = requestBody.Required;
		Content = ((requestBody.Content != null) ? new Dictionary<string, IOpenApiMediaType>(requestBody.Content) : null);
		Extensions = ((requestBody.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(requestBody.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiRequestBody" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiRequestBody" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiRequestBody" /> to Open Api v3.0
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
		writer.WriteRequiredMap("content", Content, callback);
		writer.WriteProperty("required", Required);
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiRequestBody" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
	}

	/// <inheritdoc />
	public IOpenApiParameter ConvertToBodyParameter(IOpenApiWriter writer)
	{
		OpenApiBodyParameter openApiBodyParameter = new OpenApiBodyParameter
		{
			Description = Description,
			Name = "body",
			Schema = (Content?.Values.FirstOrDefault()?.Schema ?? new OpenApiSchema()),
			Examples = Content?.Values.FirstOrDefault()?.Examples,
			Required = Required,
			Extensions = Extensions?.ToDictionary((KeyValuePair<string, IOpenApiExtension> k) => k.Key, (KeyValuePair<string, IOpenApiExtension> v) => v.Value)
		};
		if (openApiBodyParameter.Extensions != null && openApiBodyParameter.Extensions.TryGetValue("x-bodyName", out IOpenApiExtension value) && value is JsonNodeExtension jsonNodeExtension)
		{
			openApiBodyParameter.Name = (string.IsNullOrEmpty(jsonNodeExtension.Node.ToString()) ? "body" : jsonNodeExtension.Node.ToString());
			openApiBodyParameter.Extensions.Remove("x-bodyName");
		}
		return openApiBodyParameter;
	}

	/// <inheritdoc />
	public IEnumerable<IOpenApiParameter> ConvertToFormDataParameters(IOpenApiWriter writer)
	{
		if (Content == null || !Content.Any())
		{
			yield break;
		}
		IDictionary<string, IOpenApiSchema> dictionary = Content.First().Value.Schema?.Properties;
		if (dictionary == null)
		{
			yield break;
		}
		foreach (KeyValuePair<string, IOpenApiSchema> item in dictionary)
		{
			IOpenApiSchema openApiSchema = item.Value.CreateShallowCopy();
			if (((uint?)openApiSchema.Type & 0x10u) == 16 && ("binary".Equals(openApiSchema.Format, StringComparison.OrdinalIgnoreCase) || "base64".Equals(openApiSchema.Format, StringComparison.OrdinalIgnoreCase)))
			{
				IOpenApiSchema openApiSchema2 = openApiSchema;
				OpenApiSchema openApiSchema4;
				if (!(openApiSchema2 is OpenApiSchema openApiSchema3))
				{
					if (!(openApiSchema2 is OpenApiSchemaReference openApiSchemaReference))
					{
						throw new InvalidOperationException("Unexpected schema type");
					}
					if (openApiSchemaReference.Target == null)
					{
						throw new InvalidOperationException("Unresolved reference target");
					}
					openApiSchema4 = (OpenApiSchema)openApiSchemaReference.Target.CreateShallowCopy();
				}
				else
				{
					openApiSchema4 = openApiSchema3;
				}
				OpenApiSchema openApiSchema5 = openApiSchema4;
				openApiSchema5.Type = "file".ToJsonSchemaType();
				openApiSchema5.Format = null;
				openApiSchema = openApiSchema5;
			}
			yield return new OpenApiFormDataParameter
			{
				Description = openApiSchema.Description,
				Name = item.Key,
				Schema = openApiSchema,
				Examples = Content.Values.FirstOrDefault()?.Examples,
				Required = (Content.First().Value.Schema?.Required?.Contains(item.Key) == true)
			};
		}
	}

	/// <inheritdoc />
	public IOpenApiRequestBody CreateShallowCopy()
	{
		return new OpenApiRequestBody(this);
	}
}
