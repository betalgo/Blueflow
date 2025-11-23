using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Response Object Reference.
/// </summary>
public class OpenApiResponseReference : BaseOpenApiReferenceHolder<OpenApiResponse, IOpenApiResponse, OpenApiReferenceWithDescriptionAndSummary>, IOpenApiResponse, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiResponse>, IOpenApiReferenceable, IOpenApiSerializable, IOpenApiSummarizedElement
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
	public IDictionary<string, IOpenApiMediaType>? Content => Target?.Content;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiHeader>? Headers => Target?.Headers;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiLink>? Links => Target?.Links;

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
	public OpenApiResponseReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Response, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="openApiResponseReference">The reference to copy</param>
	private OpenApiResponseReference(OpenApiResponseReference openApiResponseReference)
		: base((BaseOpenApiReferenceHolder<OpenApiResponse, IOpenApiResponse, OpenApiReferenceWithDescriptionAndSummary>)openApiResponseReference)
	{
	}

	/// <inheritdoc />
	public override IOpenApiResponse CopyReferenceAsTargetElementWithOverrides(IOpenApiResponse source)
	{
		if (!(source is OpenApiResponse))
		{
			return source;
		}
		return new OpenApiResponse(this);
	}

	/// <inheritdoc />
	public IOpenApiResponse CreateShallowCopy()
	{
		return new OpenApiResponseReference(this);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescriptionAndSummary CopyReference(OpenApiReferenceWithDescriptionAndSummary sourceReference)
	{
		return new OpenApiReferenceWithDescriptionAndSummary(sourceReference);
	}
}
