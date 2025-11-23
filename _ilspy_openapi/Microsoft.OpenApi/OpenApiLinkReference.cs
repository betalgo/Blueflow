using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Link Object Reference.
/// </summary>
public class OpenApiLinkReference : BaseOpenApiReferenceHolder<OpenApiLink, IOpenApiLink, OpenApiReferenceWithDescription>, IOpenApiLink, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiLink>, IOpenApiReferenceable, IOpenApiSerializable
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
	public string? OperationRef => Target?.OperationRef;

	/// <inheritdoc />
	public string? OperationId => Target?.OperationId;

	/// <inheritdoc />
	public OpenApiServer? Server => Target?.Server;

	/// <inheritdoc />
	public IDictionary<string, RuntimeExpressionAnyWrapper>? Parameters => Target?.Parameters;

	/// <inheritdoc />
	public RuntimeExpressionAnyWrapper? RequestBody => Target?.RequestBody;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions => Target?.Extensions;

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
	public OpenApiLinkReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Link, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor.
	/// </summary>
	/// <param name="reference">The reference to copy</param>
	private OpenApiLinkReference(OpenApiLinkReference reference)
		: base((BaseOpenApiReferenceHolder<OpenApiLink, IOpenApiLink, OpenApiReferenceWithDescription>)reference)
	{
	}

	/// <inheritdoc />
	public override void SerializeAsV2(IOpenApiWriter writer)
	{
	}

	/// <inheritdoc />
	public override IOpenApiLink CopyReferenceAsTargetElementWithOverrides(IOpenApiLink source)
	{
		if (!(source is OpenApiLink))
		{
			return source;
		}
		return new OpenApiLink(this);
	}

	/// <inheritdoc />
	public IOpenApiLink CreateShallowCopy()
	{
		return new OpenApiLinkReference(this);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
	{
		return new OpenApiReferenceWithDescription(sourceReference);
	}
}
