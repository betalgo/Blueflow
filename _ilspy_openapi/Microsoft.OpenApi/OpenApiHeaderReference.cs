using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Header Object Reference.
/// </summary>
public class OpenApiHeaderReference : BaseOpenApiReferenceHolder<OpenApiHeader, IOpenApiHeader, OpenApiReferenceWithDescription>, IOpenApiHeader, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiHeader>, IOpenApiReferenceable, IOpenApiSerializable
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
	public bool Required => Target?.Required ?? false;

	/// <inheritdoc />
	public bool Deprecated => Target?.Deprecated ?? false;

	/// <inheritdoc />
	public bool AllowEmptyValue => Target?.AllowEmptyValue ?? false;

	/// <inheritdoc />
	public IOpenApiSchema? Schema => Target?.Schema;

	/// <inheritdoc />
	public ParameterStyle? Style => Target?.Style;

	/// <inheritdoc />
	public bool Explode => Target?.Explode ?? false;

	/// <inheritdoc />
	public bool AllowReserved => Target?.AllowReserved ?? false;

	/// <inheritdoc />
	public JsonNode? Example => Target?.Example;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExample>? Examples => Target?.Examples;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiMediaType>? Content => Target?.Content;

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
	public OpenApiHeaderReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Header, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="header">The <see cref="T:Microsoft.OpenApi.OpenApiHeaderReference" /> object to copy</param>
	private OpenApiHeaderReference(OpenApiHeaderReference header)
		: base((BaseOpenApiReferenceHolder<OpenApiHeader, IOpenApiHeader, OpenApiReferenceWithDescription>)header)
	{
	}

	/// <inheritdoc />
	public override IOpenApiHeader CopyReferenceAsTargetElementWithOverrides(IOpenApiHeader source)
	{
		if (!(source is OpenApiHeader))
		{
			return source;
		}
		return new OpenApiHeader(this);
	}

	/// <inheritdoc />
	public IOpenApiHeader CreateShallowCopy()
	{
		return new OpenApiHeaderReference(this);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
	{
		return new OpenApiReferenceWithDescription(sourceReference);
	}
}
