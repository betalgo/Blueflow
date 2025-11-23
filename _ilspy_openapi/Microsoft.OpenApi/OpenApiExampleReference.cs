using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Example Object Reference.
/// </summary>
public class OpenApiExampleReference : BaseOpenApiReferenceHolder<OpenApiExample, IOpenApiExample, OpenApiReferenceWithDescriptionAndSummary>, IOpenApiExample, IOpenApiDescribedElement, IOpenApiElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiExample>, IOpenApiReferenceable, IOpenApiSerializable
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
	public IDictionary<string, IOpenApiExtension>? Extensions => Target?.Extensions;

	/// <inheritdoc />
	public string? ExternalValue => Target?.ExternalValue;

	/// <inheritdoc />
	public JsonNode? Value => Target?.Value;

	/// <inheritdoc />
	public JsonNode? DataValue => Target?.DataValue;

	/// <inheritdoc />
	public string? SerializedValue => Target?.SerializedValue;

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
	public OpenApiExampleReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Example, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="example">The example reference to copy</param>
	private OpenApiExampleReference(OpenApiExampleReference example)
		: base((BaseOpenApiReferenceHolder<OpenApiExample, IOpenApiExample, OpenApiReferenceWithDescriptionAndSummary>)example)
	{
	}

	/// <inheritdoc />
	public override IOpenApiExample CopyReferenceAsTargetElementWithOverrides(IOpenApiExample source)
	{
		if (!(source is OpenApiExample))
		{
			return source;
		}
		return new OpenApiExample(this);
	}

	/// <inheritdoc />
	public override void SerializeAsV2(IOpenApiWriter writer)
	{
		base.Reference.SerializeAsV2(writer);
	}

	/// <inheritdoc />
	public IOpenApiExample CreateShallowCopy()
	{
		return new OpenApiExampleReference(this);
	}

	/// <inheritdoc />
	protected override OpenApiReferenceWithDescriptionAndSummary CopyReference(OpenApiReferenceWithDescriptionAndSummary sourceReference)
	{
		return new OpenApiReferenceWithDescriptionAndSummary(sourceReference);
	}
}
