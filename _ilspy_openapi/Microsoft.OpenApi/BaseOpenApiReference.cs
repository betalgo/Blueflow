using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi;

/// <summary>
/// A simple object to allow referencing other components in the specification, internally and externally.
/// </summary>
public class BaseOpenApiReference : IOpenApiSerializable, IOpenApiElement
{
	private OpenApiDocument? hostDocument;

	private string? _referenceV3;

	/// <summary>
	/// External resource in the reference.
	/// It maybe:
	/// 1. a absolute/relative file path, for example:  ../commons/pet.json
	/// 2. a Url, for example: http://localhost/pet.json
	/// </summary>
	public string? ExternalResource { get; init; }

	/// <summary>
	/// The element type referenced.
	/// </summary>
	/// <remarks>This must be present if <see cref="P:Microsoft.OpenApi.BaseOpenApiReference.ExternalResource" /> is not present.</remarks>
	public ReferenceType Type { get; init; }

	/// <summary>
	/// The identifier of the reusable component of one particular ReferenceType.
	/// If ExternalResource is present, this is the path to the component after the '#/'.
	/// For example, if the reference is 'example.json#/path/to/component', the Id is 'path/to/component'.
	/// If ExternalResource is not present, this is the name of the component without the reference type name.
	/// For example, if the reference is '#/components/schemas/componentName', the Id is 'componentName'.
	/// </summary>
	public string? Id { get; init; }

	/// <summary>
	/// Gets a flag indicating whether this reference is an external reference.
	/// </summary>
	public bool IsExternal => ExternalResource != null;

	/// <summary>
	/// Gets a flag indicating whether this reference is a local reference.
	/// </summary>
	public bool IsLocal => ExternalResource == null;

	/// <summary>
	/// Gets a flag indicating whether a file is a valid OpenAPI document or a fragment
	/// </summary>
	public bool IsFragment { get; init; }

	/// <summary>
	/// The OpenApiDocument that is hosting the OpenApiReference instance. This is used to enable dereferencing the reference.
	/// </summary>
	public OpenApiDocument? HostDocument
	{
		get
		{
			return hostDocument;
		}
		init
		{
			hostDocument = value;
		}
	}

	/// <summary>
	/// Gets the full reference string for v3.0.
	/// </summary>
	public string? ReferenceV3
	{
		get
		{
			if (!string.IsNullOrEmpty(_referenceV3))
			{
				return _referenceV3;
			}
			if (IsExternal)
			{
				return GetExternalReferenceV3();
			}
			if (Type == ReferenceType.Tag)
			{
				return Id;
			}
			if (Type == ReferenceType.SecurityScheme)
			{
				return Id;
			}
			if (!string.IsNullOrEmpty(Id) && Id != null && (Id.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || Id.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || Id.Contains("#/components", StringComparison.OrdinalIgnoreCase)))
			{
				return Id;
			}
			return "#/components/" + Type.GetDisplayName() + "/" + Id;
		}
		private set
		{
			if (value != null)
			{
				_referenceV3 = value;
			}
		}
	}

	/// <summary>
	/// Gets the full reference string for V2.0
	/// </summary>
	public string? ReferenceV2
	{
		get
		{
			if (IsExternal)
			{
				return GetExternalReferenceV2();
			}
			if (Type == ReferenceType.Tag)
			{
				return Id;
			}
			if (Type == ReferenceType.SecurityScheme)
			{
				return Id;
			}
			return "#/" + GetReferenceTypeNameAsV2(Type) + "/" + Id;
		}
	}

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public BaseOpenApiReference()
	{
	}

	/// <summary>
	/// Initializes a copy instance of the <see cref="T:Microsoft.OpenApi.BaseOpenApiReference" /> object
	/// </summary>
	public BaseOpenApiReference(BaseOpenApiReference reference)
	{
		Utils.CheckArgumentNull(reference, "reference");
		ExternalResource = reference.ExternalResource;
		Type = reference.Type;
		Id = reference.Id;
		HostDocument = reference.HostDocument;
	}

	/// <inheritdoc />
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, SerializeAdditionalV32Properties);
	}

	/// <inheritdoc />
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, SerializeAdditionalV31Properties);
	}

	/// <summary>
	/// Serialize additional properties for Open Api v3.2.
	/// </summary>
	/// <param name="writer"></param>
	protected virtual void SerializeAdditionalV32Properties(IOpenApiWriter writer)
	{
	}

	/// <summary>
	/// Serialize additional properties for Open Api v3.1.
	/// </summary>
	/// <param name="writer"></param>
	protected virtual void SerializeAdditionalV31Properties(IOpenApiWriter writer)
	{
	}

	/// <inheritdoc />
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.BaseOpenApiReference" />
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter>? callback = null)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (Type == ReferenceType.Tag && !string.IsNullOrEmpty(ReferenceV3) && ReferenceV3 != null)
		{
			writer.WriteValue(ReferenceV3);
			return;
		}
		writer.WriteStartObject();
		callback?.Invoke(writer);
		writer.WriteProperty("$ref", ReferenceV3);
		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (Type == ReferenceType.Tag && !string.IsNullOrEmpty(ReferenceV2) && ReferenceV2 != null)
		{
			writer.WriteValue(ReferenceV2);
			return;
		}
		if (Type == ReferenceType.SecurityScheme && !string.IsNullOrEmpty(ReferenceV2) && ReferenceV2 != null)
		{
			writer.WritePropertyName(ReferenceV2);
			return;
		}
		writer.WriteStartObject();
		writer.WriteProperty("$ref", ReferenceV2);
		writer.WriteEndObject();
	}

	private string? GetExternalReferenceV3()
	{
		if (Id != null)
		{
			if (IsFragment)
			{
				return ExternalResource + "#" + Id;
			}
			if (Id.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || Id.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				return Id;
			}
			return ExternalResource + "#/components/" + Type.GetDisplayName() + "/" + Id;
		}
		return ExternalResource;
	}

	private string? GetExternalReferenceV2()
	{
		if (Id != null)
		{
			return ExternalResource + "#/" + GetReferenceTypeNameAsV2(Type) + "/" + Id;
		}
		return ExternalResource;
	}

	private static string? GetReferenceTypeNameAsV2(ReferenceType type)
	{
		switch (type)
		{
		case ReferenceType.Schema:
			return "definitions";
		case ReferenceType.Parameter:
		case ReferenceType.RequestBody:
			return "parameters";
		case ReferenceType.Response:
			return "responses";
		case ReferenceType.Header:
			return "headers";
		case ReferenceType.Tag:
			return "tags";
		case ReferenceType.SecurityScheme:
			return "securityDefinitions";
		default:
			return null;
		}
	}

	/// <summary>
	/// Sets the host document after deserialization or before serialization.
	/// This method is internal on purpose to avoid consumers mutating the host document.
	/// </summary>
	/// <param name="currentDocument">Host document to set if none is present</param>
	internal void EnsureHostDocumentIsSet(OpenApiDocument currentDocument)
	{
		Utils.CheckArgumentNull(currentDocument, "currentDocument");
		if (hostDocument == null)
		{
			hostDocument = currentDocument;
		}
	}

	/// <summary>
	/// Gets the property value from a JsonObject node.
	/// </summary>
	/// <param name="jsonObject">The object to get the value from</param>
	/// <param name="key">The key of the property</param>
	/// <returns>The property value</returns>
	protected internal static string? GetPropertyValueFromNode(JsonObject jsonObject, string key)
	{
		if (!jsonObject.TryGetPropertyValue(key, out JsonNode jsonNode) || !(jsonNode is JsonValue jsonValue) || !jsonValue.TryGetValue<string>(out string value))
		{
			return null;
		}
		return value;
	}

	internal virtual void SetMetadataFromMapNode(MapNode mapNode)
	{
		if (mapNode.JsonNode is JsonObject additional31MetadataFromMapNode)
		{
			SetAdditional31MetadataFromMapNode(additional31MetadataFromMapNode);
		}
	}

	/// <summary>
	/// Sets additional metadata from the map node.
	/// </summary>
	/// <param name="jsonObject">The object to get the data from</param>
	protected virtual void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
	{
	}

	internal void SetJsonPointerPath(string pointer, string nodeLocation)
	{
		if (pointer.StartsWith("#/", StringComparison.OrdinalIgnoreCase) && !pointer.Contains("/components/schemas", StringComparison.OrdinalIgnoreCase))
		{
			ReferenceV3 = ResolveRelativePointer(nodeLocation, pointer);
		}
		else if ((pointer.Contains('#') || pointer.StartsWith("http", StringComparison.OrdinalIgnoreCase)) && !string.Equals(ReferenceV3, pointer, StringComparison.OrdinalIgnoreCase))
		{
			ReferenceV3 = pointer;
		}
	}

	private static string ResolveRelativePointer(string nodeLocation, string relativeRef)
	{
		List<string> list = nodeLocation.TrimStart('#').Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		string[] array = relativeRef.TrimStart('#').Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i <= list.Count - array.Length; i++)
		{
			if (array.SequenceEqual<string>(list.Skip(i).Take(array.Length), StringComparer.Ordinal))
			{
				string[] array2 = list.Take(i + array.Length).ToArray();
				if (array2 != null && array2.Length > 0)
				{
					return "#/" + string.Join("/", array2);
				}
			}
		}
		if (nodeLocation.StartsWith("#/components/schemas/", StringComparison.OrdinalIgnoreCase))
		{
			return "#/" + string.Join("/", list.Take(3).Concat(array));
		}
		return "#/" + string.Join("/", list.SkipLast(array.Length).Concat(array));
	}
}
