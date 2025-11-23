using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Server Object: an object representing a Server.
/// </summary>
public class OpenApiServer : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible
{
	/// <summary>
	/// An optional string describing the host designated by the URL. CommonMark syntax MAY be used for rich text representation.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// An optional string identifying the server. This MUST be unique across servers in the same document.
	/// Note: This field is supported in OpenAPI 3.2.0+. For earlier versions, it will be serialized as x-oai-name extension.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// REQUIRED. A URL to the target host. This URL supports Server Variables and MAY be relative,
	/// to indicate that the host location is relative to the location where the OpenAPI document is being served.
	/// Variable substitutions will be made when a variable is named in {brackets}.
	/// </summary>
	public string? Url { get; set; }

	/// <summary>
	/// A map between a variable name and its value. The value is used for substitution in the server's URL template.
	/// </summary>
	public IDictionary<string, OpenApiServerVariable>? Variables { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiServer()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiServer" /> object
	/// </summary>
	public OpenApiServer(OpenApiServer server)
	{
		Description = server?.Description ?? Description;
		Name = server?.Name ?? Name;
		Url = server?.Url ?? Url;
		Variables = ((server?.Variables != null) ? new Dictionary<string, OpenApiServerVariable>(server.Variables) : null);
		Extensions = ((server?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(server.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiServer" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiServer" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiServer" /> to Open Api v3.0
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiServer" /> to Open Api v3.0
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("url", Url);
		if (!string.IsNullOrEmpty(Name))
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteProperty("name", Name);
			}
			else
			{
				writer.WriteProperty("x-oai-name", Name);
			}
		}
		writer.WriteProperty("description", Description);
		writer.WriteOptionalMap("variables", Variables, callback);
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiServer" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
	}
}
