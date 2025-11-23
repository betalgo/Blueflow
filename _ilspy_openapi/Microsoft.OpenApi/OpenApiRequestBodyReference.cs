using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Request Body Object Reference.
/// </summary>
public class OpenApiRequestBodyReference : BaseOpenApiReferenceHolder<OpenApiRequestBody, IOpenApiRequestBody, OpenApiReferenceWithDescription>, IOpenApiRequestBody, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiRequestBody>, IOpenApiReferenceable, IOpenApiSerializable
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
	public IDictionary<string, IOpenApiMediaType>? Content => Target?.Content;

	/// <inheritdoc />
	public bool Required => Target?.Required ?? false;

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
	public OpenApiRequestBodyReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.RequestBody, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="openApiRequestBodyReference">The reference to copy</param>
	private OpenApiRequestBodyReference(OpenApiRequestBodyReference openApiRequestBodyReference)
		: base((BaseOpenApiReferenceHolder<OpenApiRequestBody, IOpenApiRequestBody, OpenApiReferenceWithDescription>)openApiRequestBodyReference)
	{
	}

	/// <inheritdoc />
	public override IOpenApiRequestBody CopyReferenceAsTargetElementWithOverrides(IOpenApiRequestBody source)
	{
		if (!(source is OpenApiRequestBody))
		{
			return source;
		}
		return new OpenApiRequestBody(this);
	}

	/// <inheritdoc />
	public override void SerializeAsV2(IOpenApiWriter writer)
	{
	}

	/// <inheritdoc />
	public IOpenApiParameter? ConvertToBodyParameter(IOpenApiWriter writer)
	{
		if (writer.GetSettings().ShouldInlineReference(base.Reference))
		{
			return Target?.ConvertToBodyParameter(writer);
		}
		if (base.Reference.Id == null)
		{
			return null;
		}
		return new OpenApiParameterReference(base.Reference.Id, base.Reference.HostDocument);
	}

	/// <inheritdoc />
	public IEnumerable<IOpenApiParameter>? ConvertToFormDataParameters(IOpenApiWriter writer)
	{
		if (writer.GetSettings().ShouldInlineReference(base.Reference))
		{
			return Target?.ConvertToFormDataParameters(writer);
		}
		if (Content == null || !Content.Any())
		{
			return Array.Empty<IOpenApiParameter>();
		}
		return Content.First().Value.Schema?.Properties?.Select((KeyValuePair<string, IOpenApiSchema> x) => new OpenApiParameterReference(x.Key, base.Reference.HostDocument));
	}

	/// <inheritdoc />
	public IOpenApiRequestBody CreateShallowCopy()
	{
		return new OpenApiRequestBodyReference(this);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
	{
		return new OpenApiReferenceWithDescription(sourceReference);
	}
}
