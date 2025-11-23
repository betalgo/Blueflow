using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Responses object.
/// </summary>
public class OpenApiResponses : OpenApiExtensibleDictionary<IOpenApiResponse>
{
	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiResponses()
	{
	}

	/// <summary>
	/// Initializes a copy of <see cref="T:Microsoft.OpenApi.OpenApiResponses" /> object
	/// </summary>
	/// <param name="openApiResponses">The <see cref="T:Microsoft.OpenApi.OpenApiResponses" /></param>
	public OpenApiResponses(OpenApiResponses openApiResponses)
		: base((Dictionary<string, IOpenApiResponse>)openApiResponses, (Dictionary<string, IOpenApiExtension>?)null)
	{
	}
}
