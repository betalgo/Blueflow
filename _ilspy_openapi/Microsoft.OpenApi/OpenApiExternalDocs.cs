using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// ExternalDocs object.
/// </summary>
public class OpenApiExternalDocs : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible
{
	/// <summary>
	/// A short description of the target documentation.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// REQUIRED. The URL for the target documentation. Value MUST be in the format of a URL.
	/// </summary>
	public Uri? Url { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiExternalDocs()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> object
	/// </summary>
	public OpenApiExternalDocs(OpenApiExternalDocs externalDocs)
	{
		Description = externalDocs?.Description ?? Description;
		Url = ((externalDocs?.Url != null) ? new Uri(externalDocs.Url.OriginalString, UriKind.RelativeOrAbsolute) : null);
		Extensions = ((externalDocs?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(externalDocs.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> to Open Api v3.2.
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi3_2);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> to Open Api v3.1.
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi3_1);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> to Open Api v3.0.
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi3_0);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> to Open Api v2.0.
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi2_0);
	}

	private void WriteInternal(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("description", Description);
		writer.WriteProperty("url", Url?.OriginalString);
		writer.WriteExtensions(Extensions, specVersion);
		writer.WriteEndObject();
	}
}
