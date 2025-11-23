using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Parameter Object Reference.
/// </summary>
public class OpenApiParameterReference : BaseOpenApiReferenceHolder<OpenApiParameter, IOpenApiParameter, OpenApiReferenceWithDescription>, IOpenApiParameter, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiParameter>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public string? Name => Target?.Name;

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
	public bool AllowReserved => Target?.AllowReserved ?? false;

	/// <inheritdoc />
	public IOpenApiSchema? Schema => Target?.Schema;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExample>? Examples => Target?.Examples;

	/// <inheritdoc />
	public JsonNode? Example => Target?.Example;

	/// <inheritdoc />
	public ParameterLocation? In => Target?.In;

	/// <inheritdoc />
	public ParameterStyle? Style => Target?.Style;

	/// <inheritdoc />
	public bool Explode => Target?.Explode ?? false;

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
	public OpenApiParameterReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Parameter, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="parameter">The parameter reference to copy</param>
	private OpenApiParameterReference(OpenApiParameterReference parameter)
		: base((BaseOpenApiReferenceHolder<OpenApiParameter, IOpenApiParameter, OpenApiReferenceWithDescription>)parameter)
	{
	}

	/// <inheritdoc />
	public override IOpenApiParameter CopyReferenceAsTargetElementWithOverrides(IOpenApiParameter source)
	{
		if (!(source is OpenApiParameter))
		{
			return source;
		}
		return new OpenApiParameter(this);
	}

	/// <inheritdoc />
	public IOpenApiParameter CreateShallowCopy()
	{
		return new OpenApiParameterReference(this);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescription CopyReference(OpenApiReferenceWithDescription sourceReference)
	{
		return new OpenApiReferenceWithDescription(sourceReference);
	}
}
