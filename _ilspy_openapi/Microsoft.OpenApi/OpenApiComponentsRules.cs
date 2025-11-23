using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiComponents" />.
/// </summary>
[OpenApiRule]
public static class OpenApiComponentsRules
{
	/// <summary>
	/// The key regex.
	/// </summary>
	internal static readonly Regex KeyRegex = new Regex("^[a-zA-Z0-9\\.\\-_]+$", RegexOptions.None, TimeSpan.FromMilliseconds(100.0));

	/// <summary>
	/// All the fixed fields declared above are objects
	/// that MUST use keys that match the regular expression: ^[a-zA-Z0-9\.\-_]+$.
	/// </summary>
	public static ValidationRule<OpenApiComponents> KeyMustBeRegularExpression => new ValidationRule<OpenApiComponents>("KeyMustBeRegularExpression", delegate(IValidationContext context, OpenApiComponents components)
	{
		ValidateKeys(context, components.Schemas?.Keys, "schemas");
		ValidateKeys(context, components.Responses?.Keys, "responses");
		ValidateKeys(context, components.Parameters?.Keys, "parameters");
		ValidateKeys(context, components.Examples?.Keys, "examples");
		ValidateKeys(context, components.RequestBodies?.Keys, "requestBodies");
		ValidateKeys(context, components.Headers?.Keys, "headers");
		ValidateKeys(context, components.SecuritySchemes?.Keys, "securitySchemes");
		ValidateKeys(context, components.Links?.Keys, "links");
		ValidateKeys(context, components.Callbacks?.Keys, "callbacks");
	});

	private static void ValidateKeys(IValidationContext context, IEnumerable<string>? keys, string component)
	{
		if (keys == null)
		{
			return;
		}
		foreach (string key in keys)
		{
			if (!KeyRegex.IsMatch(key))
			{
				context.CreateError("KeyMustBeRegularExpression", string.Format(SRResource.Validation_ComponentsKeyMustMatchRegularExpr, key, component, KeyRegex.ToString()));
			}
		}
	}
}
