using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the path item object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiPathItem : IOpenApiDescribedElement, IOpenApiElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiPathItem>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <summary>
	/// Gets the definition of operations on this path.
	/// </summary>
	Dictionary<HttpMethod, OpenApiOperation>? Operations { get; }

	/// <summary>
	/// An alternative server array to service all operations in this path.
	/// </summary>
	IList<OpenApiServer>? Servers { get; }

	/// <summary>
	/// A list of parameters that are applicable for all the operations described under this path.
	/// These parameters can be overridden at the operation level, but cannot be removed there.
	/// </summary>
	IList<IOpenApiParameter>? Parameters { get; }
}
