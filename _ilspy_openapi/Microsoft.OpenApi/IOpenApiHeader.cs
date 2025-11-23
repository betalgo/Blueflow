using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the headers object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiHeader : IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiHeader>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <summary>
	/// Determines whether this header is mandatory.
	/// </summary>
	bool Required { get; }

	/// <summary>
	/// Specifies that a header is deprecated and SHOULD be transitioned out of usage.
	/// </summary>
	bool Deprecated { get; }

	/// <summary>
	/// Sets the ability to pass empty-valued headers.
	/// </summary>
	bool AllowEmptyValue { get; }

	/// <summary>
	/// Describes how the header value will be serialized depending on the type of the header value.
	/// </summary>
	ParameterStyle? Style { get; }

	/// <summary>
	/// When this is true, header values of type array or object generate separate parameters
	/// for each value of the array or key-value pair of the map.
	/// </summary>
	bool Explode { get; }

	/// <summary>
	/// Determines whether the header value SHOULD allow reserved characters, as defined by RFC3986.
	/// </summary>
	bool AllowReserved { get; }

	/// <summary>
	/// The schema defining the type used for the request body.
	/// </summary>
	IOpenApiSchema? Schema { get; }

	/// <summary>
	/// Example of the media type.
	/// You must use the <see cref="M:Microsoft.OpenApi.JsonNullSentinel.IsJsonNullSentinel(System.Text.Json.Nodes.JsonNode)" /> method to check whether Default was assigned a null value in the document.
	/// Assign <see cref="P:Microsoft.OpenApi.JsonNullSentinel.JsonNull" /> to use get null as a serialized value.
	/// </summary>
	JsonNode? Example { get; }

	/// <summary>
	/// Examples of the media type.
	/// </summary>
	IDictionary<string, IOpenApiExample>? Examples { get; }

	/// <summary>
	/// A map containing the representations for the header.
	/// </summary>
	IDictionary<string, IOpenApiMediaType>? Content { get; }
}
