using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// OAuth Flows Object.
/// </summary>
public class OpenApiOAuthFlows : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible
{
	/// <summary>
	/// Configuration for the OAuth Implicit flow
	/// </summary>
	public OpenApiOAuthFlow? Implicit { get; set; }

	/// <summary>
	/// Configuration for the OAuth Resource Owner Password flow.
	/// </summary>
	public OpenApiOAuthFlow? Password { get; set; }

	/// <summary>
	/// Configuration for the OAuth Client Credentials flow.
	/// </summary>
	public OpenApiOAuthFlow? ClientCredentials { get; set; }

	/// <summary>
	/// Configuration for the OAuth Authorization Code flow.
	/// </summary>
	public OpenApiOAuthFlow? AuthorizationCode { get; set; }

	/// <summary>
	/// Configuration for the OAuth Device Authorization flow.
	/// </summary>
	public OpenApiOAuthFlow? DeviceAuthorization { get; set; }

	/// <summary>
	/// Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiOAuthFlows()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" /> object
	/// </summary>
	/// <param name="oAuthFlows"></param>
	public OpenApiOAuthFlows(OpenApiOAuthFlows oAuthFlows)
	{
		Implicit = ((oAuthFlows?.Implicit != null) ? new OpenApiOAuthFlow(oAuthFlows.Implicit) : null);
		Password = ((oAuthFlows?.Password != null) ? new OpenApiOAuthFlow(oAuthFlows.Password) : null);
		ClientCredentials = ((oAuthFlows?.ClientCredentials != null) ? new OpenApiOAuthFlow(oAuthFlows.ClientCredentials) : null);
		AuthorizationCode = ((oAuthFlows?.AuthorizationCode != null) ? new OpenApiOAuthFlow(oAuthFlows.AuthorizationCode) : null);
		DeviceAuthorization = ((oAuthFlows?.DeviceAuthorization != null) ? new OpenApiOAuthFlow(oAuthFlows.DeviceAuthorization) : null);
		Extensions = ((oAuthFlows?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(oAuthFlows.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" /> to Open Api v3.0
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" />
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteOptionalObject("implicit", Implicit, callback);
		writer.WriteOptionalObject("password", Password, callback);
		writer.WriteOptionalObject("clientCredentials", ClientCredentials, callback);
		writer.WriteOptionalObject("authorizationCode", AuthorizationCode, callback);
		if (version >= OpenApiSpecVersion.OpenApi3_2)
		{
			writer.WriteOptionalObject("deviceAuthorization", DeviceAuthorization, callback);
		}
		else if (DeviceAuthorization != null)
		{
			writer.WriteOptionalObject("x-oai-deviceAuthorization", DeviceAuthorization, callback);
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
	}
}
