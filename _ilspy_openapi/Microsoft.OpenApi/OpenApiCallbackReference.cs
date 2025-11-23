using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Callback Object Reference: A reference to a map of possible out-of band callbacks related to the parent operation.
/// </summary>
public class OpenApiCallbackReference : BaseOpenApiReferenceHolder<OpenApiCallback, IOpenApiCallback, BaseOpenApiReference>, IOpenApiCallback, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiCallback>, IOpenApiReferenceable, IOpenApiSerializable, IOpenApiElement
{
	/// <inheritdoc />
	public Dictionary<RuntimeExpression, IOpenApiPathItem>? PathItems => Target?.PathItems;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions => Target?.Extensions;

	/// <summary>
	/// Constructor initializing the reference object.
	/// </summary>
	/// <param name="referenceId">The reference Id.</param>
	/// <param name="hostDocument">The host OpenAPI document.</param>
	/// <param name="externalResource">Optional: External resource in the reference.
	/// It may be:
	/// 1. an absolute/relative file path, for example:  ../commons/pet.json
	/// 2. a Url, for example: http://localhost/pet.json
	/// </param>
	public OpenApiCallbackReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Callback, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="callback">The reference to copy</param>
	private OpenApiCallbackReference(OpenApiCallbackReference callback)
		: base((BaseOpenApiReferenceHolder<OpenApiCallback, IOpenApiCallback, BaseOpenApiReference>)callback)
	{
	}

	/// <inheritdoc />
	public override IOpenApiCallback CopyReferenceAsTargetElementWithOverrides(IOpenApiCallback source)
	{
		if (!(source is OpenApiCallback))
		{
			return source;
		}
		return new OpenApiCallback(this);
	}

	/// <inheritdoc />
	public override void SerializeAsV2(IOpenApiWriter writer)
	{
		base.Reference.SerializeAsV2(writer);
	}

	/// <inheritdoc />
	public IOpenApiCallback CreateShallowCopy()
	{
		return new OpenApiCallbackReference(this);
	}

	/// <inheritdoc />
	protected override BaseOpenApiReference CopyReference(BaseOpenApiReference sourceReference)
	{
		return new BaseOpenApiReference(sourceReference);
	}
}
