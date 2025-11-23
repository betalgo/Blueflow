using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the example object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiExample : IOpenApiDescribedElement, IOpenApiElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiExample>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <summary>
	/// Embedded literal example. The value field and externalValue field are mutually
	/// exclusive. To represent examples of media types that cannot naturally represented
	/// in JSON or YAML, use a string value to contain the example, escaping where necessary.
	/// You must use the <see cref="M:Microsoft.OpenApi.JsonNullSentinel.IsJsonNullSentinel(System.Text.Json.Nodes.JsonNode)" /> method to check whether Default was assigned a null value in the document.
	/// Assign <see cref="P:Microsoft.OpenApi.JsonNullSentinel.JsonNull" /> to use get null as a serialized value.
	/// </summary>
	JsonNode? Value { get; }

	/// <summary>
	/// A URL that points to the literal example.
	/// This provides the capability to reference examples that cannot easily be
	/// included in JSON or YAML documents.
	/// The value field and externalValue field are mutually exclusive.
	/// </summary>
	string? ExternalValue { get; }

	/// <summary>
	/// Embedded literal example value. 
	/// The dataValue property and the value property are mutually exclusive.
	/// To represent examples of media types that cannot be naturally represented in JSON or YAML,
	/// use a string value to contain the example with escaping where necessary.
	/// Available in OpenAPI 3.2+, serialized as extension in 3.1 and earlier.
	/// </summary>
	JsonNode? DataValue { get; }

	/// <summary>
	/// A string representation of the example.
	/// This is mutually exclusive with the value and dataValue properties.
	/// Available in OpenAPI 3.2+, serialized as extension in 3.1 and earlier.
	/// </summary>
	string? SerializedValue { get; }
}
