using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Example Object.
/// </summary>
public class OpenApiExample : IOpenApiExtensible, IOpenApiElement, IOpenApiExample, IOpenApiDescribedElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiExample>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public string? Summary { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public string? ExternalValue { get; set; }

	/// <inheritdoc />
	public JsonNode? Value { get; set; }

	/// <inheritdoc />
	public JsonNode? DataValue { get; set; }

	/// <inheritdoc />
	public string? SerializedValue { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiExample()
	{
	}

	/// <summary>
	/// Initializes a copy of <see cref="T:Microsoft.OpenApi.OpenApiExample" /> object
	/// </summary>
	/// <param name="example">The <see cref="T:Microsoft.OpenApi.IOpenApiExample" /> object</param>
	internal OpenApiExample(IOpenApiExample example)
	{
		Utils.CheckArgumentNull(example, "example");
		Summary = example.Summary ?? Summary;
		Description = example.Description ?? Description;
		Value = ((example.Value != null) ? JsonNodeCloneHelper.Clone(example.Value) : null);
		ExternalValue = example.ExternalValue ?? ExternalValue;
		DataValue = ((example.DataValue != null) ? JsonNodeCloneHelper.Clone(example.DataValue) : null);
		SerializedValue = example.SerializedValue ?? SerializedValue;
		Extensions = ((example.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(example.Extensions) : null);
	}

	/// <inheritdoc />
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2);
	}

	/// <inheritdoc />
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1);
	}

	/// <inheritdoc />
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0);
	}

	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("summary", Summary);
		writer.WriteProperty("description", Description);
		if (Value != null)
		{
			writer.WriteRequiredObject("value", Value, delegate(IOpenApiWriter w, JsonNode v)
			{
				w.WriteAny(v);
			});
		}
		writer.WriteProperty("externalValue", ExternalValue);
		if (DataValue != null)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteRequiredObject("dataValue", DataValue, delegate(IOpenApiWriter w, JsonNode v)
				{
					w.WriteAny(v);
				});
			}
			else
			{
				writer.WriteRequiredObject("x-oai-dataValue", DataValue, delegate(IOpenApiWriter w, JsonNode v)
				{
					w.WriteAny(v);
				});
			}
		}
		if (SerializedValue != null)
		{
			if (version >= OpenApiSpecVersion.OpenApi3_2)
			{
				writer.WriteProperty("serializedValue", SerializedValue);
			}
			else
			{
				writer.WriteProperty("x-oai-serializedValue", SerializedValue);
			}
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi2_0);
	}

	/// <inheritdoc />
	public IOpenApiExample CreateShallowCopy()
	{
		return new OpenApiExample(this);
	}
}
