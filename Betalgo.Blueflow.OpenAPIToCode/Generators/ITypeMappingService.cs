using Microsoft.OpenApi.Models;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators;

/// <summary>
///     Interface for type mapping services.
/// </summary>
public interface ITypeMappingService
{
    string MapType(OpenApiSchema openApiType);
}