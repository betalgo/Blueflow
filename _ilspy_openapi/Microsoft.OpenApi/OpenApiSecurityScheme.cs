using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Security Scheme Object.
/// </summary>
public class OpenApiSecurityScheme : IOpenApiExtensible, IOpenApiElement, IOpenApiSecurityScheme, IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiSecurityScheme>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public SecuritySchemeType? Type { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public string? Name { get; set; }

	/// <inheritdoc />
	public ParameterLocation? In { get; set; }

	/// <inheritdoc />
	public string? Scheme { get; set; }

	/// <inheritdoc />
	public string? BearerFormat { get; set; }

	/// <inheritdoc />
	public OpenApiOAuthFlows? Flows { get; set; }

	/// <inheritdoc />
	public Uri? OpenIdConnectUrl { get; set; }

	/// <inheritdoc />
	public bool Deprecated { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiSecurityScheme()
	{
	}

	/// <summary>
	/// Initializes a copy of <see cref="T:Microsoft.OpenApi.IOpenApiSecurityScheme" /> object
	/// </summary>
	internal OpenApiSecurityScheme(IOpenApiSecurityScheme securityScheme)
	{
		Utils.CheckArgumentNull(securityScheme, "securityScheme");
		Type = securityScheme.Type;
		Description = securityScheme.Description ?? Description;
		Name = securityScheme.Name ?? Name;
		In = securityScheme.In;
		Scheme = securityScheme.Scheme ?? Scheme;
		BearerFormat = securityScheme.BearerFormat ?? BearerFormat;
		Flows = ((securityScheme.Flows != null) ? new OpenApiOAuthFlows(securityScheme.Flows) : null);
		OpenIdConnectUrl = ((securityScheme.OpenIdConnectUrl != null) ? new Uri(securityScheme.OpenIdConnectUrl.OriginalString, UriKind.RelativeOrAbsolute) : null);
		Deprecated = securityScheme.Deprecated;
		Extensions = ((securityScheme.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(securityScheme.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityScheme" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityScheme" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityScheme" /> to Open Api v3.0
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
		SecuritySchemeType? type = Type;
		writer.WriteProperty("type", type.HasValue ? type.GetValueOrDefault().GetDisplayName() : null);
		writer.WriteProperty("description", Description);
		switch (Type)
		{
		case SecuritySchemeType.ApiKey:
		{
			writer.WriteProperty("name", Name);
			ParameterLocation? parameterLocation = In;
			writer.WriteProperty("in", parameterLocation.HasValue ? parameterLocation.GetValueOrDefault().GetDisplayName() : null);
			break;
		}
		case SecuritySchemeType.Http:
			writer.WriteProperty("scheme", Scheme);
			writer.WriteProperty("bearerFormat", BearerFormat);
			break;
		case SecuritySchemeType.OAuth2:
			writer.WriteOptionalObject("flows", Flows, callback);
			break;
		case SecuritySchemeType.OpenIdConnect:
			writer.WriteProperty("openIdConnectUrl", OpenIdConnectUrl?.ToString());
			break;
		}
		if (Deprecated)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteProperty("deprecated", Deprecated);
			}
			else
			{
				writer.WriteProperty("x-oai-deprecated", Deprecated);
			}
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityScheme" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (Type == SecuritySchemeType.Http && Scheme != "basic")
		{
			writer.WriteStartObject();
			writer.WriteEndObject();
			return;
		}
		if (Type == SecuritySchemeType.OpenIdConnect)
		{
			writer.WriteStartObject();
			writer.WriteEndObject();
			return;
		}
		writer.WriteStartObject();
		switch (Type)
		{
		case SecuritySchemeType.Http:
			writer.WriteProperty("type", "basic");
			break;
		case SecuritySchemeType.OAuth2:
			writer.WriteProperty("type", Type.GetDisplayName());
			WriteOAuthFlowForV2(writer, Flows);
			break;
		case SecuritySchemeType.ApiKey:
		{
			writer.WriteProperty("type", Type.GetDisplayName());
			writer.WriteProperty("name", Name);
			ParameterLocation? parameterLocation = In;
			writer.WriteProperty("in", parameterLocation.HasValue ? parameterLocation.GetValueOrDefault().GetDisplayName() : null);
			break;
		}
		}
		writer.WriteProperty("description", Description);
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Arbitrarily chooses one <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlow" /> object from the <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" />
	/// to populate in V2 security scheme.
	/// </summary>
	private static void WriteOAuthFlowForV2(IOpenApiWriter writer, OpenApiOAuthFlows? flows)
	{
		if (flows != null)
		{
			if (flows.Implicit != null)
			{
				WriteOAuthFlowForV2(writer, "implicit", flows.Implicit);
			}
			else if (flows.Password != null)
			{
				WriteOAuthFlowForV2(writer, "password", flows.Password);
			}
			else if (flows.ClientCredentials != null)
			{
				WriteOAuthFlowForV2(writer, "application", flows.ClientCredentials);
			}
			else if (flows.AuthorizationCode != null)
			{
				WriteOAuthFlowForV2(writer, "accessCode", flows.AuthorizationCode);
			}
		}
	}

	private static void WriteOAuthFlowForV2(IOpenApiWriter writer, string flowValue, OpenApiOAuthFlow flow)
	{
		writer.WriteProperty("flow", flowValue);
		writer.WriteProperty("authorizationUrl", flow.AuthorizationUrl?.ToString());
		writer.WriteProperty("tokenUrl", flow.TokenUrl?.ToString());
		writer.WriteOptionalMap("scopes", flow.Scopes, delegate(IOpenApiWriter w, string s)
		{
			w.WriteValue(s);
		});
	}

	/// <inheritdoc />
	public IOpenApiSecurityScheme CreateShallowCopy()
	{
		return new OpenApiSecurityScheme(this);
	}
}
