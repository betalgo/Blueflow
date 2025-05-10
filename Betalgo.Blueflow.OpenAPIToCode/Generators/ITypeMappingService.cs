using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;
using Microsoft.OpenApi.Models;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators
{
    /// <summary>
    /// Interface for type mapping services.
    /// </summary>
    public interface ITypeMappingService
    {
        /// <summary>
        /// Maps an OpenAPI type (or schema) to a target language type.
        /// </summary>
        /// <param name="openApiType">The OpenAPI type or schema name.</param>
        /// <param name="isNullable">Whether the type is nullable.</param>
        /// <returns>The mapped type as a string.</returns>
        string MapType(PropertyDefinition openApiType, bool isNullable = false);
        string MapType(OpenApiSchema openApiType);
    }
}
