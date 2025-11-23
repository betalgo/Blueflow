using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Security Scheme Object Reference.
/// </summary>
public class OpenApiSecuritySchemeReference : BaseOpenApiReferenceHolder<OpenApiSecurityScheme, IOpenApiSecurityScheme, OpenApiReferenceWithDescription>, IOpenApiSecurityScheme, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiSecurityScheme>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public string? Description
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Reference.Description))
			{
				return base.Reference.Description;
			}
			return Target?.Description;
		}
		set
		{
			base.Reference.Description = value;
		}
	}

	/// <inheritdoc />
	public string? Name => Target?.Name;

	/// <inheritdoc />
	public ParameterLocation? In => Target?.In;

	/// <inheritdoc />
	public string? Scheme => Target?.Scheme;

	/// <inheritdoc />
	public string? BearerFormat => Target?.BearerFormat;

	/// <inheritdoc />
	public OpenApiOAuthFlows? Flows => Target?.Flows;

	/// <inheritdoc />
	public Uri? OpenIdConnectUrl => Target?.OpenIdConnectUrl;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions => Target?.Extensions;

	/// <inheritdoc />
	public SecuritySchemeType? Type => Target?.Type;

	/// <inheritdoc />
	public bool Deprecated => Target?.Deprecated ?? false;

	/// <summary>
	/// Constructor initializing the reference object.
	/// </summary>
	/// <param name="referenceId">The reference Id.</param>
	/// <param name="hostDocument">The host OpenAPI document.</param>
	/// <param name="externalResource">The externally referenced file.</param>
	public OpenApiSecuritySchemeReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.SecurityScheme, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="openApiSecuritySchemeReference">The reference to copy</param>
	private OpenApiSecuritySchemeReference(OpenApiSecuritySchemeReference openApiSecuritySchemeReference)
		: base((BaseOpenApiReferenceHolder<OpenApiSecurityScheme, IOpenApiSecurityScheme, OpenApiReferenceWithDescription>)openApiSecuritySchemeReference)
	{
	}

	/// <inheritdoc />
	public override IOpenApiSecurityScheme CopyReferenceAsTargetElementWithOverrides(IOpenApiSecurityScheme source)
	{
		if (!(source is OpenApiSecurityScheme))
		{
			return source;
		}
		return new OpenApiSecurityScheme(this);
	}

	/// <inheritdoc />
	public IOpenApiSecurityScheme CreateShallowCopy()
	{
		return new OpenApiSecuritySchemeReference(this);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
	{
		return new OpenApiReferenceWithDescription(sourceReference);
	}
}
