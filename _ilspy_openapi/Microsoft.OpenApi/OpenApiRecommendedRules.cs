using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.OpenApi;

/// <summary>
/// Additional recommended validation rules for OpenAPI.
/// </summary>
public static class OpenApiRecommendedRules
{
	/// <summary>
	/// A relative path to an individual endpoint. The field name MUST begin with a slash.
	/// </summary>
	public static ValidationRule<OpenApiPaths> GetOperationShouldNotHaveRequestBody => new ValidationRule<OpenApiPaths>("GetOperationShouldNotHaveRequestBody", delegate(IValidationContext context, OpenApiPaths item)
	{
		foreach (KeyValuePair<string, IOpenApiPathItem> item in item)
		{
			Dictionary<HttpMethod, OpenApiOperation> operations = item.Value.Operations;
			if (operations != null && operations.Count > 0)
			{
				context.Enter(item.Key);
				foreach (KeyValuePair<HttpMethod, OpenApiOperation> item2 in operations)
				{
					if (item2.Key.Equals(HttpMethod.Get) && item2.Value.RequestBody != null)
					{
						context.Enter(item2.Key.Method.ToLowerInvariant());
						context.Enter("requestBody");
						context.CreateWarning("GetOperationShouldNotHaveRequestBody", "GET operations should not have a request body.");
						context.Exit();
						context.Exit();
					}
				}
				context.Exit();
			}
		}
	});
}
