using Betalgo.Blueflow.OpenApiExtensions;
using Microsoft.OpenApi.Models;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp.Services;

/// <summary>
///     CSharp-specific implementation of ITypeMappingService.
/// </summary>
public class CSharpTypeMappingService : ITypeMappingService
{
    private static readonly Dictionary<string, string> TypeMap = new()
    {
        { "integer", "int" },
        { "int32", "int" },
        { "int64", "long" },
        { "number", "double" },
        { "float", "float" },
        { "double", "double" },
        { "decimal", "decimal" },
        { "string", "string" },
        { "byte", "byte" },
        { "binary", "byte[]" },
        { "boolean", "bool" },
        { "bool", "bool" },
        { "date", "DateOnly" },
        { "date-time", "DateTime" },
        { "datetime", "DateTime" },
        { "uuid", "Guid" },
        { "guid", "Guid" },
        { "uri", "Uri" },
        { "url", "Uri" },
        { "array", "List" },
        { "object", "object" },
        { "any", "object" },
        { "null", "object" }
    };

    public string MapType(OpenApiSchema schema)
    {
        if (string.IsNullOrWhiteSpace(schema?.Type))
            return "object";

        string? type;
        if (schema.IsArray())
        {
            type = $"List<{MapType(schema.Items!)}>";
        }
        else
        {
            if (schema.IsObject())
            {
                type = schema.GetBlueflowName() ?? "object";
            }
            else
            {
                type = TypeMap.GetValueOrDefault(schema.Type.ToLowerInvariant(), schema.Type);
            }

            if (type == "string" && schema.Enum.Any())
            {
                type = schema.GetBlueflowName();
            }
        }


        //if (schema.Nullable && type != "string" && type != "object" && type != "byte[]" && type != "Uri")
        if (schema.Nullable)
            type += "?";


        return type;
    }
}