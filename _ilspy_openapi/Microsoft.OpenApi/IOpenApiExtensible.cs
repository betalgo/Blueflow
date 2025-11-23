using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Represents an Extensible Open API element.
/// </summary>
public interface IOpenApiExtensible : IOpenApiElement
{
	/// <summary>
	/// Specification extensions.
	/// </summary>
	IDictionary<string, IOpenApiExtension>? Extensions { get; set; }
}
