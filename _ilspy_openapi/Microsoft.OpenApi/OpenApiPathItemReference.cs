using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.OpenApi;

/// <summary>
/// Path Item Object Reference: to describe the operations available on a single path.
/// </summary>
public class OpenApiPathItemReference : BaseOpenApiReferenceHolder<OpenApiPathItem, IOpenApiPathItem, OpenApiReferenceWithDescriptionAndSummary>, IOpenApiPathItem, IOpenApiDescribedElement, IOpenApiElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiPathItem>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public string? Summary
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Reference.Summary))
			{
				return base.Reference.Summary;
			}
			return Target?.Summary;
		}
		set
		{
			base.Reference.Summary = value;
		}
	}

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
	public Dictionary<HttpMethod, OpenApiOperation>? Operations => Target?.Operations;

	/// <inheritdoc />
	public IList<OpenApiServer>? Servers => Target?.Servers;

	/// <inheritdoc />
	public IList<IOpenApiParameter>? Parameters => Target?.Parameters;

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
	public OpenApiPathItemReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.PathItem, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="pathItem">The reference to copy</param>
	private OpenApiPathItemReference(OpenApiPathItemReference pathItem)
		: base((BaseOpenApiReferenceHolder<OpenApiPathItem, IOpenApiPathItem, OpenApiReferenceWithDescriptionAndSummary>)pathItem)
	{
	}

	/// <inheritdoc />
	public override IOpenApiPathItem CopyReferenceAsTargetElementWithOverrides(IOpenApiPathItem source)
	{
		if (!(source is OpenApiPathItem))
		{
			return source;
		}
		return new OpenApiPathItem(this);
	}

	/// <inheritdoc />
	public IOpenApiPathItem CreateShallowCopy()
	{
		return new OpenApiPathItemReference(this);
	}

	/// <inheritdoc />
	public override void SerializeAsV2(IOpenApiWriter writer)
	{
		base.Reference.SerializeAsV2(writer);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescriptionAndSummary CopyReference(OpenApiReferenceWithDescriptionAndSummary sourceReference)
	{
		return new OpenApiReferenceWithDescriptionAndSummary(sourceReference);
	}
}
