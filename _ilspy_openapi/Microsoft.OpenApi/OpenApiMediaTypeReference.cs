using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Media Type Object Reference.
/// </summary>
public class OpenApiMediaTypeReference : BaseOpenApiReferenceHolder<OpenApiMediaType, IOpenApiMediaType, BaseOpenApiReference>, IOpenApiMediaType, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiMediaType>, IOpenApiReferenceable, IOpenApiSerializable, IOpenApiElement
{
	/// <inheritdoc />
	public IOpenApiSchema? Schema => Target?.Schema;

	/// <inheritdoc />
	public IOpenApiSchema? ItemSchema => Target?.ItemSchema;

	/// <inheritdoc />
	public JsonNode? Example => Target?.Example;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExample>? Examples => Target?.Examples;

	/// <inheritdoc />
	public IDictionary<string, OpenApiEncoding>? Encoding => Target?.Encoding;

	/// <inheritdoc />
	public OpenApiEncoding? ItemEncoding => Target?.ItemEncoding;

	/// <inheritdoc />
	public IList<OpenApiEncoding>? PrefixEncoding => Target?.PrefixEncoding;

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
	public OpenApiMediaTypeReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.MediaType, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="mediaTypeReference">The media type reference to copy</param>
	private OpenApiMediaTypeReference(OpenApiMediaTypeReference mediaTypeReference)
		: base((BaseOpenApiReferenceHolder<OpenApiMediaType, IOpenApiMediaType, BaseOpenApiReference>)mediaTypeReference)
	{
	}

	/// <inheritdoc />
	public override IOpenApiMediaType CopyReferenceAsTargetElementWithOverrides(IOpenApiMediaType source)
	{
		if (!(source is OpenApiMediaType))
		{
			return source;
		}
		return new OpenApiMediaType(this);
	}

	/// <inheritdoc />
	public IOpenApiMediaType CreateShallowCopy()
	{
		return new OpenApiMediaTypeReference(this);
	}

	/// <inheritdoc />
	protected override BaseOpenApiReference CopyReference(BaseOpenApiReference sourceReference)
	{
		return new BaseOpenApiReference(sourceReference);
	}

	/// <inheritdoc />
	public override void SerializeAsV31(IOpenApiWriter writer)
	{
		CopyReferenceAsTargetElementWithOverrides(new OpenApiMediaType()).SerializeAsV31(writer);
	}

	/// <inheritdoc />
	public override void SerializeAsV3(IOpenApiWriter writer)
	{
		CopyReferenceAsTargetElementWithOverrides(new OpenApiMediaType()).SerializeAsV3(writer);
	}
}
