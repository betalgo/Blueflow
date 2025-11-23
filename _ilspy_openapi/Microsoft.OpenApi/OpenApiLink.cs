using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Link Object.
/// </summary>
public class OpenApiLink : IOpenApiExtensible, IOpenApiElement, IOpenApiLink, IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiLink>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public string? OperationRef { get; set; }

	/// <inheritdoc />
	public string? OperationId { get; set; }

	/// <inheritdoc />
	public IDictionary<string, RuntimeExpressionAnyWrapper>? Parameters { get; set; }

	/// <inheritdoc />
	public RuntimeExpressionAnyWrapper? RequestBody { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public OpenApiServer? Server { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiLink()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiLink" /> object
	/// </summary>
	internal OpenApiLink(IOpenApiLink link)
	{
		Utils.CheckArgumentNull(link, "link");
		OperationRef = link.OperationRef ?? OperationRef;
		OperationId = link.OperationId ?? OperationId;
		Parameters = ((link.Parameters != null) ? new Dictionary<string, RuntimeExpressionAnyWrapper>(link.Parameters) : null);
		RequestBody = ((link.RequestBody != null) ? new RuntimeExpressionAnyWrapper(link.RequestBody) : null);
		Description = link.Description ?? Description;
		Server = ((link.Server != null) ? new OpenApiServer(link.Server) : null);
		Extensions = ((link.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(link.Extensions) : null);
	}

	/// <inheritdoc />
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <inheritdoc />
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <inheritdoc />
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	internal void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("operationRef", OperationRef);
		writer.WriteProperty("operationId", OperationId);
		writer.WriteOptionalMap("parameters", Parameters, delegate(IOpenApiWriter w, RuntimeExpressionAnyWrapper p)
		{
			p.WriteValue(w);
		});
		writer.WriteOptionalObject("requestBody", RequestBody, delegate(IOpenApiWriter w, RuntimeExpressionAnyWrapper r)
		{
			r.WriteValue(w);
		});
		writer.WriteProperty("description", Description);
		writer.WriteOptionalObject("server", Server, callback);
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);
		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
	}

	/// <inheritdoc />
	public IOpenApiLink CreateShallowCopy()
	{
		return new OpenApiLink(this);
	}
}
