using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Paths object.
/// </summary>
public class OpenApiPaths : OpenApiExtensibleDictionary<IOpenApiPathItem>
{
	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiPaths()
	{
	}

	/// <summary>
	/// Initializes a copy of <see cref="T:Microsoft.OpenApi.OpenApiPaths" /> object
	/// </summary>
	/// <param name="paths">The <see cref="T:Microsoft.OpenApi.OpenApiPaths" />.</param>
	public OpenApiPaths(OpenApiPaths paths)
		: base((Dictionary<string, IOpenApiPathItem>)paths, (Dictionary<string, IOpenApiExtension>?)null)
	{
	}
}
