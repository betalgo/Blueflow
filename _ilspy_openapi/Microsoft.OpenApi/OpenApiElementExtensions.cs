using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Extension methods that apply across all OpenAPIElements
/// </summary>
public static class OpenApiElementExtensions
{
	/// <summary>
	/// Validate element and all child elements
	/// </summary>
	/// <param name="element">Element to validate</param>
	/// <param name="ruleSet">Optional set of rules to use for validation</param>
	/// <returns>An IEnumerable of errors.  This function will never return null.</returns>
	public static IEnumerable<OpenApiError> Validate(this IOpenApiElement element, ValidationRuleSet ruleSet)
	{
		OpenApiValidator openApiValidator = new OpenApiValidator(ruleSet);
		new OpenApiWalker(openApiValidator).Walk(element);
		return openApiValidator.Errors.Cast<OpenApiError>().Union(openApiValidator.Warnings);
	}
}
