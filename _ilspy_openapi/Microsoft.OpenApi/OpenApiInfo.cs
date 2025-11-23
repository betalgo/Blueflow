using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Open API Info Object, it provides the metadata about the Open API.
/// </summary>
public class OpenApiInfo : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible
{
	/// <summary>
	/// REQUIRED. The title of the application.
	/// </summary>
	public string? Title { get; set; }

	/// <summary>
	/// A short summary of the API.
	/// </summary>
	public string? Summary { get; set; }

	/// <summary>
	/// A short description of the application.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// REQUIRED. The version of the OpenAPI document.
	/// </summary>
	public string? Version { get; set; }

	/// <summary>
	/// A URL to the Terms of Service for the API. MUST be in the format of a URL.
	/// </summary>
	public Uri? TermsOfService { get; set; }

	/// <summary>
	/// The contact information for the exposed API.
	/// </summary>
	public OpenApiContact? Contact { get; set; }

	/// <summary>
	/// The license information for the exposed API.
	/// </summary>
	public OpenApiLicense? License { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiInfo()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiInfo" /> object
	/// </summary>
	public OpenApiInfo(OpenApiInfo info)
	{
		Title = info?.Title ?? Title;
		Summary = info?.Summary ?? Summary;
		Description = info?.Description ?? Description;
		Version = info?.Version ?? Version;
		TermsOfService = info?.TermsOfService ?? TermsOfService;
		Contact = ((info?.Contact != null) ? new OpenApiContact(info.Contact) : null);
		License = ((info?.License != null) ? new OpenApiLicense(info.License) : null);
		Extensions = ((info?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(info.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiInfo" /> to Open Api v3.2
	/// </summary>
	public void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
		writer.WriteProperty("summary", Summary);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiInfo" /> to Open Api v3.1
	/// </summary>
	public void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
		writer.WriteProperty("summary", Summary);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiInfo" /> to Open Api v3.0
	/// </summary>
	public void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiInfo" /> to Open Api v3.0
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("title", Title);
		writer.WriteProperty("description", Description);
		writer.WriteProperty("termsOfService", TermsOfService?.OriginalString);
		writer.WriteOptionalObject("contact", Contact, callback);
		writer.WriteOptionalObject("license", License, callback);
		writer.WriteProperty("version", Version);
		writer.WriteExtensions(Extensions, version);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiInfo" /> to Open Api v2.0
	/// </summary>
	public void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("title", Title);
		writer.WriteProperty("description", Description);
		writer.WriteProperty("termsOfService", TermsOfService?.OriginalString);
		writer.WriteOptionalObject("contact", Contact, delegate(IOpenApiWriter w, OpenApiContact c)
		{
			c.SerializeAsV2(w);
		});
		writer.WriteOptionalObject("license", License, delegate(IOpenApiWriter w, OpenApiLicense l)
		{
			l.SerializeAsV2(w);
		});
		writer.WriteProperty("version", Version);
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}
}
