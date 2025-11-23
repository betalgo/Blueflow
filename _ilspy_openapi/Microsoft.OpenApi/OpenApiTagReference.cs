using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Tag Object Reference
/// </summary>
public class OpenApiTagReference : BaseOpenApiReferenceHolder<OpenApiTag, IOpenApiTag, BaseOpenApiReference>, IOpenApiTag, IOpenApiReadOnlyExtensible, IOpenApiReadOnlyDescribedElement, IOpenApiElement, IShallowCopyable<IOpenApiTag>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <summary>
	/// Resolved target of the reference.
	/// </summary>
	public override IOpenApiTag? Target => base.Reference.HostDocument?.Tags?.FirstOrDefault((OpenApiTag t) => OpenApiTagComparer.StringComparer.Equals(t.Name, base.Reference.Id));

	/// <inheritdoc />
	public string? Description => Target?.Description;

	/// <inheritdoc />
	public OpenApiExternalDocs? ExternalDocs => Target?.ExternalDocs;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions => Target?.Extensions;

	/// <inheritdoc />
	public string? Name
	{
		get
		{
			object obj = Target?.Name;
			if (obj == null)
			{
				BaseOpenApiReference reference = base.Reference;
				if (reference == null)
				{
					return null;
				}
				obj = reference.Id;
			}
			return (string?)obj;
		}
	}

	/// <inheritdoc />
	public string? Summary => Target?.Summary;

	/// <inheritdoc />
	public OpenApiTagReference? Parent
	{
		get
		{
			if (Target is OpenApiTagReference openApiTagReference)
			{
				string? id = base.Reference.Id;
				if (id != null && id.Equals(openApiTagReference.Reference.Id, StringComparison.Ordinal))
				{
					return null;
				}
			}
			return Target?.Parent;
		}
	}

	/// <inheritdoc />
	public string? Kind => Target?.Kind;

	/// <summary>
	/// Constructor initializing the reference object.
	/// </summary>
	/// <param name="referenceId">The reference Id.</param>
	/// <param name="hostDocument">The host OpenAPI document.</param>
	/// <param name="externalResource">Optional: External resource in the reference.
	/// It may be:
	/// 1. a absolute/relative file path, for example:  ../commons/pet.json
	/// 2. a Url, for example: http://localhost/pet.json
	/// </param>
	public OpenApiTagReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Tag, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="openApiTagReference">The reference to copy</param>
	private OpenApiTagReference(OpenApiTagReference openApiTagReference)
		: base((BaseOpenApiReferenceHolder<OpenApiTag, IOpenApiTag, BaseOpenApiReference>)openApiTagReference)
	{
	}

	/// <inheritdoc />
	public override IOpenApiTag CopyReferenceAsTargetElementWithOverrides(IOpenApiTag source)
	{
		if (!(source is OpenApiTag))
		{
			return source;
		}
		return new OpenApiTag(this);
	}

	/// <inheritdoc />
	public IOpenApiTag CreateShallowCopy()
	{
		return new OpenApiTagReference(this);
	}

	/// <inheritdoc />
	protected override BaseOpenApiReference CopyReference(BaseOpenApiReference sourceReference)
	{
		return new BaseOpenApiReference(sourceReference);
	}
}
