using Microsoft.OpenApi;

namespace Blueflow.Chopper.OpenApi;

internal static class PathFragmentFactory
{
    public static OpenApiPathItem Create(IOpenApiPathItem source, HttpMethod httpMethod)
    {
        if (source.Operations is null || !source.Operations.TryGetValue(httpMethod, out var operation))
        {
            throw new InvalidOperationException($"Operation '{httpMethod}' does not exist on path.");
        }

        var fragment = new OpenApiPathItem
        {
            Summary = source.Summary,
            Description = source.Description,
            Servers = source.Servers?.ToList(),
            Parameters = source.Parameters?.ToList()
        };

        if (source.Extensions is { Count: > 0 })
        {
            fragment.Extensions = new Dictionary<string, IOpenApiExtension>(source.Extensions);
        }

        fragment.AddOperation(httpMethod, operation);
        return fragment;
    }
}