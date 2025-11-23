using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// XML Object.
/// </summary>
public class OpenApiXml : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible
{
	/// <summary>
	/// Replaces the name of the element/attribute used for the described schema property.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// The URI of the namespace definition. Value MUST be in the form of an absolute URI.
	/// </summary>
	public Uri? Namespace { get; set; }

	/// <summary>
	/// The prefix to be used for the name
	/// </summary>
	public string? Prefix { get; set; }

	/// <summary>
	/// Declares whether the property definition translates to an attribute instead of an element.
	/// Default value is false.
	/// </summary>
	[Obsolete("Use NodeType property instead. This property will be removed in a future version.")]
	internal bool Attribute
	{
		get
		{
			return NodeType == OpenApiXmlNodeType.Attribute;
		}
		set
		{
			NodeType = (value ? OpenApiXmlNodeType.Attribute : OpenApiXmlNodeType.None);
		}
	}

	/// <summary>
	/// Signifies whether the array is wrapped.
	/// Default value is false.
	/// </summary>
	[Obsolete("Use NodeType property instead. This property will be removed in a future version.")]
	internal bool Wrapped
	{
		get
		{
			return NodeType == OpenApiXmlNodeType.Element;
		}
		set
		{
			NodeType = ((!value) ? OpenApiXmlNodeType.None : OpenApiXmlNodeType.Element);
		}
	}

	/// <summary>
	/// The node type of the XML representation.
	/// </summary>
	public OpenApiXmlNodeType? NodeType { get; set; }

	/// <summary>
	/// Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiXml()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiXml" /> object
	/// </summary>
	public OpenApiXml(OpenApiXml xml)
	{
		Name = xml?.Name ?? Name;
		Namespace = xml?.Namespace ?? Namespace;
		Prefix = xml?.Prefix ?? Prefix;
		NodeType = xml?.NodeType ?? NodeType;
		Extensions = ((xml?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(xml.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiXml" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		Write(writer, OpenApiSpecVersion.OpenApi3_2);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiXml" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		Write(writer, OpenApiSpecVersion.OpenApi3_1);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiXml" /> to Open Api v3.0
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		Write(writer, OpenApiSpecVersion.OpenApi3_0);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiXml" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		Write(writer, OpenApiSpecVersion.OpenApi2_0);
	}

	private void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("name", Name);
		writer.WriteProperty("namespace", Namespace?.AbsoluteUri);
		writer.WriteProperty("prefix", Prefix);
		if (specVersion >= OpenApiSpecVersion.OpenApi3_2)
		{
			if (NodeType.HasValue)
			{
				writer.WriteProperty("nodeType", NodeType.Value.GetDisplayName());
			}
		}
		else
		{
			bool value = NodeType.HasValue && NodeType == OpenApiXmlNodeType.Attribute;
			bool value2 = NodeType.HasValue && NodeType == OpenApiXmlNodeType.Element;
			writer.WriteProperty("attribute", value);
			writer.WriteProperty("wrapped", value2);
		}
		writer.WriteExtensions(Extensions, specVersion);
		writer.WriteEndObject();
	}
}
