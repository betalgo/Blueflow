using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// License Object.
/// </summary>
public class OpenApiLicense : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible
{
	/// <summary>
	/// REQUIRED. The license name used for the API.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// An SPDX license expression for the API. The identifier field is mutually exclusive of the url field.
	/// </summary>
	public string? Identifier { get; set; }

	/// <summary>
	/// The URL pointing to the contact information. MUST be in the format of a URL.
	/// </summary>
	public Uri? Url { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiLicense()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiLicense" /> object
	/// </summary>
	public OpenApiLicense(OpenApiLicense license)
	{
		Name = license?.Name ?? Name;
		Identifier = license?.Identifier ?? Identifier;
		Url = ((license?.Url != null) ? new Uri(license.Url.OriginalString, UriKind.RelativeOrAbsolute) : null);
		Extensions = ((license?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(license.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiLicense" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi3_2);
		writer.WriteProperty("identifier", Identifier);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiLicense" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi3_1);
		writer.WriteProperty("identifier", Identifier);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiLicense" /> to Open Api v3.0
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi3_0);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiLicense" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		WriteInternal(writer, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	private void WriteInternal(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("name", Name);
		writer.WriteProperty("url", Url?.OriginalString);
		writer.WriteExtensions(Extensions, specVersion);
	}
}
