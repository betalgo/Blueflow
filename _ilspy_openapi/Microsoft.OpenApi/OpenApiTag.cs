using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Tag Object.
/// </summary>
public class OpenApiTag : IOpenApiExtensible, IOpenApiElement, IOpenApiTag, IOpenApiReadOnlyExtensible, IOpenApiReadOnlyDescribedElement, IShallowCopyable<IOpenApiTag>, IOpenApiReferenceable, IOpenApiSerializable, IOpenApiDescribedElement
{
	/// <inheritdoc />
	public string? Name { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public OpenApiExternalDocs? ExternalDocs { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <inheritdoc />
	public string? Summary { get; set; }

	/// <inheritdoc />
	public OpenApiTagReference? Parent { get; set; }

	/// <inheritdoc />
	public string? Kind { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiTag()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.IOpenApiTag" /> object
	/// </summary>
	internal OpenApiTag(IOpenApiTag tag)
	{
		Utils.CheckArgumentNull(tag, "tag");
		Name = tag.Name ?? Name;
		Description = tag.Description ?? Description;
		ExternalDocs = ((tag.ExternalDocs != null) ? new OpenApiExternalDocs(tag.ExternalDocs) : null);
		Extensions = ((tag.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(tag.Extensions) : null);
		Summary = tag.Summary ?? Summary;
		Parent = tag.Parent ?? Parent;
		Kind = tag.Kind ?? Kind;
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiTag" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiTag" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiTag" /> to Open Api v3.0
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
		writer.WriteStartObject();
		writer.WriteProperty("name", Name);
		writer.WriteProperty("description", Description);
		writer.WriteOptionalObject("externalDocs", ExternalDocs, callback);
		if (Summary != null)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteProperty("summary", Summary);
			}
			else if (version >= OpenApiSpecVersion.OpenApi3_0)
			{
				writer.WriteProperty("x-oas-summary", Summary);
			}
		}
		if (Parent != null)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WritePropertyName("parent");
				Parent.SerializeAsV32(writer);
			}
			else
			{
				switch (version)
				{
				case OpenApiSpecVersion.OpenApi3_1:
					writer.WritePropertyName("x-oas-parent");
					Parent.SerializeAsV31(writer);
					break;
				case OpenApiSpecVersion.OpenApi3_0:
					writer.WritePropertyName("x-oas-parent");
					Parent.SerializeAsV3(writer);
					break;
				}
			}
		}
		if (Kind != null)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteProperty("kind", Kind);
			}
			else if (version >= OpenApiSpecVersion.OpenApi3_0)
			{
				writer.WriteProperty("x-oas-kind", Kind);
			}
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiTag" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi2_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV2(writer2);
		});
	}

	/// <inheritdoc />
	public IOpenApiTag CreateShallowCopy()
	{
		return new OpenApiTag(this);
	}
}
