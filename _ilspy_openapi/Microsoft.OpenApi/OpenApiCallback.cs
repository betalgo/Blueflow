using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Callback Object: A map of possible out-of band callbacks related to the parent operation.
/// </summary>
public class OpenApiCallback : IOpenApiExtensible, IOpenApiElement, IOpenApiCallback, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiCallback>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public Dictionary<RuntimeExpression, IOpenApiPathItem>? PathItems { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiCallback()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiCallback" /> object
	/// </summary>
	internal OpenApiCallback(IOpenApiCallback callback)
	{
		Utils.CheckArgumentNull(callback, "callback");
		PathItems = ((callback?.PathItems != null) ? new Dictionary<RuntimeExpression, IOpenApiPathItem>(callback.PathItems) : null);
		Extensions = ((callback?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(callback.Extensions) : null);
	}

	/// <summary>
	/// Add a <see cref="T:Microsoft.OpenApi.IOpenApiPathItem" /> into the <see cref="P:Microsoft.OpenApi.OpenApiCallback.PathItems" />.
	/// </summary>
	/// <param name="expression">The runtime expression.</param>
	/// <param name="pathItem">The path item.</param>
	public void AddPathItem(RuntimeExpression expression, IOpenApiPathItem pathItem)
	{
		Utils.CheckArgumentNull(expression, "expression");
		Utils.CheckArgumentNull(pathItem, "pathItem");
		if (PathItems == null)
		{
			Dictionary<RuntimeExpression, IOpenApiPathItem> dictionary = (PathItems = new Dictionary<RuntimeExpression, IOpenApiPathItem>());
		}
		PathItems.Add(expression, pathItem);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiCallback" /> to Open Api v3.2
	/// </summary>
	/// <param name="writer"></param>
	/// <exception cref="T:System.NotImplementedException"></exception>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiCallback" /> to Open Api v3.1
	/// </summary>
	/// <param name="writer"></param>
	/// <exception cref="T:System.NotImplementedException"></exception>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiCallback" /> to Open Api v3.0
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	internal void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		if (PathItems != null)
		{
			foreach (KeyValuePair<RuntimeExpression, IOpenApiPathItem> pathItem in PathItems)
			{
				writer.WriteRequiredObject(pathItem.Key.Expression, pathItem.Value, callback);
			}
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiCallback" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
	}

	/// <inheritdoc />
	public IOpenApiCallback CreateShallowCopy()
	{
		return new OpenApiCallback(this);
	}
}
