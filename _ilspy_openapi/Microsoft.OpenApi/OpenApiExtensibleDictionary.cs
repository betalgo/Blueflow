using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Generic dictionary type for Open API dictionary element.
/// </summary>
/// <typeparam name="T">The Open API element, <see cref="T:Microsoft.OpenApi.IOpenApiElement" /></typeparam>
public abstract class OpenApiExtensibleDictionary<T> : Dictionary<string, T>, IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible where T : IOpenApiSerializable
{
	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	protected OpenApiExtensibleDictionary()
		: this(new Dictionary<string, T>(), (Dictionary<string, IOpenApiExtension>?)null)
	{
	}

	/// <summary>
	/// Initializes a copy of <see cref="T:Microsoft.OpenApi.OpenApiExtensibleDictionary`1" /> class.
	/// </summary>
	/// <param name="dictionary">The generic dictionary.</param>
	/// <param name="extensions">The dictionary of <see cref="T:Microsoft.OpenApi.IOpenApiExtension" />.</param>
	protected OpenApiExtensibleDictionary(Dictionary<string, T> dictionary, Dictionary<string, IOpenApiExtension>? extensions = null)
		: base((dictionary == null) ? ((IDictionary<string, T>)new Dictionary<string, T>()) : ((IDictionary<string, T>)dictionary))
	{
		Extensions = ((extensions != null) ? new Dictionary<string, IOpenApiExtension>(extensions) : new Dictionary<string, IOpenApiExtension>());
	}

	/// <summary>
	/// Serialize to Open Api v3.2
	/// </summary>
	/// <param name="writer"></param>
	public void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize to Open Api v3.1
	/// </summary>
	/// <param name="writer"></param>
	public void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize to Open Api v3.0
	/// </summary>
	/// <param name="writer"></param>
	public void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	/// <summary>
	/// Serialize to Open Api v3.0
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		using (Dictionary<string, T>.Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, T> current = enumerator.Current;
				writer.WriteRequiredObject(current.Key, current.Value, callback);
			}
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize to Open Api v2.0
	/// </summary>
	public void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		using (Dictionary<string, T>.Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, T> current = enumerator.Current;
				writer.WriteRequiredObject(current.Key, current.Value, delegate(IOpenApiWriter w, T p)
				{
					p.SerializeAsV2(w);
				});
			}
		}
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}
}
