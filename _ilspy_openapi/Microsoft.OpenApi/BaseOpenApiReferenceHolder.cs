using System;

namespace Microsoft.OpenApi;

/// <summary>
/// Base class for OpenApiReferenceHolder.
/// </summary>
/// <typeparam name="T">The concrete class implementation type for the model.</typeparam>
/// <typeparam name="U">The interface type for the model.</typeparam>
/// <typeparam name="V">The type for the reference holding the additional fields and annotations</typeparam>
public abstract class BaseOpenApiReferenceHolder<T, U, V> : IOpenApiReferenceHolder<T, U, V>, IOpenApiReferenceHolder<V>, IOpenApiReferenceHolder, IOpenApiSerializable, IOpenApiElement where T : class, IOpenApiReferenceable, U where U : IOpenApiReferenceable, IOpenApiSerializable where V : BaseOpenApiReference, new()
{
	/// <inheritdoc />
	public virtual U? Target
	{
		get
		{
			if (Reference.HostDocument == null)
			{
				return default(U);
			}
			return Reference.HostDocument.ResolveReferenceTo<U>(Reference, this as IOpenApiSchema);
		}
	}

	/// <inheritdoc />
	public T? RecursiveTarget
	{
		get
		{
			U target = Target;
			if (!(target is BaseOpenApiReferenceHolder<T, U, V> baseOpenApiReferenceHolder))
			{
				if (target is T result)
				{
					return result;
				}
				return null;
			}
			return baseOpenApiReferenceHolder.RecursiveTarget;
		}
	}

	/// <inheritdoc />
	public bool UnresolvedReference
	{
		get
		{
			if (Reference != null)
			{
				return Target == null;
			}
			return true;
		}
	}

	/// <inheritdoc />
	public V Reference { get; init; }

	/// <summary>
	/// Copy the reference as a target element with overrides.
	/// </summary>
	/// <param name="sourceReference">The source reference to copy</param>
	/// <returns>The copy of the reference</returns>
	protected abstract V CopyReference(V sourceReference);

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="source">The parameter reference to copy</param>
	protected BaseOpenApiReferenceHolder(BaseOpenApiReferenceHolder<T, U, V> source)
	{
		Utils.CheckArgumentNull(source, "source");
		Reference = CopyReference(source.Reference);
	}

	/// <summary>
	/// Constructor initializing the reference object.
	/// </summary>
	/// <param name="referenceId">The reference Id.</param>
	/// <param name="hostDocument">The host OpenAPI document.</param>
	/// <param name="referenceType">The reference type.</param>
	/// <param name="externalResource">Optional: External resource in the reference.
	/// It may be:
	/// 1. a absolute/relative file path, for example:  ../commons/pet.json
	/// 2. a Url, for example: http://localhost/pet.json
	/// </param>
	protected BaseOpenApiReferenceHolder(string referenceId, OpenApiDocument? hostDocument, ReferenceType referenceType, string? externalResource)
	{
		Utils.CheckArgumentNullOrEmpty(referenceId, "referenceId");
		Reference = new V
		{
			Id = referenceId,
			HostDocument = hostDocument,
			Type = referenceType,
			ExternalResource = externalResource
		};
	}

	/// <inheritdoc />
	public abstract U CopyReferenceAsTargetElementWithOverrides(U source);

	/// <inheritdoc />
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		if (!writer.GetSettings().ShouldInlineReference(Reference) || Reference.Type == ReferenceType.Tag)
		{
			Reference.SerializeAsV3(writer);
			return;
		}
		SerializeInternal(writer, delegate(IOpenApiWriter writer2, U element)
		{
			((IOpenApiSerializable)element)?.SerializeAsV3(writer2);
		});
	}

	/// <inheritdoc />
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		if (!writer.GetSettings().ShouldInlineReference(Reference))
		{
			Reference.SerializeAsV32(writer);
			return;
		}
		SerializeInternal(writer, delegate(IOpenApiWriter writer2, U element)
		{
			((IOpenApiSerializable)CopyReferenceAsTargetElementWithOverrides(element)/*cast due to .constrained prefix*/).SerializeAsV32(writer2);
		});
	}

	/// <inheritdoc />
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		if (!writer.GetSettings().ShouldInlineReference(Reference))
		{
			Reference.SerializeAsV31(writer);
			return;
		}
		SerializeInternal(writer, delegate(IOpenApiWriter writer2, U element)
		{
			((IOpenApiSerializable)CopyReferenceAsTargetElementWithOverrides(element)/*cast due to .constrained prefix*/).SerializeAsV31(writer2);
		});
	}

	/// <inheritdoc />
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		if (!writer.GetSettings().ShouldInlineReference(Reference))
		{
			Reference.SerializeAsV2(writer);
			return;
		}
		SerializeInternal(writer, delegate(IOpenApiWriter writer2, U element)
		{
			((IOpenApiSerializable)element)?.SerializeAsV2(writer2);
		});
	}

	/// <summary>
	/// Serialize the reference as a reference or the target object.
	/// This method is used to accelerate the serialization methods implementations.
	/// </summary>
	/// <param name="writer">The OpenApiWriter.</param>
	/// <param name="action">The action to serialize the target object.</param>
	private protected void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, U> action)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (Target != null)
		{
			action(writer, Target);
		}
	}
}
