using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiServer" />.
/// </summary>
[OpenApiRule]
public static class OpenApiServerRules
{
	/// <summary>
	/// Validate the field is required.
	/// </summary>
	public static ValidationRule<OpenApiServer> ServerRequiredFields => new ValidationRule<OpenApiServer>("ServerRequiredFields", delegate(IValidationContext context, OpenApiServer server)
	{
		if (server.Url == null)
		{
			context.Enter("url");
			context.CreateError("ServerRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "url", "server"));
			context.Exit();
		}
		if (server.Variables != null)
		{
			context.Enter("variables");
			foreach (KeyValuePair<string, OpenApiServerVariable> variable in server.Variables)
			{
				context.Enter(variable.Key);
				ValidateServerVariableRequiredFields(context, variable.Key, variable.Value);
				context.Exit();
			}
			context.Exit();
		}
	});

	/// <summary>
	/// Validate required fields in server variable
	/// </summary>
	private static void ValidateServerVariableRequiredFields(IValidationContext context, string key, OpenApiServerVariable item)
	{
		if (string.IsNullOrEmpty(item.Default))
		{
			context.Enter("default");
			context.CreateError("ServerVariableMustHaveDefaultValue", string.Format(SRResource.Validation_FieldIsRequired, "default", key));
			context.Exit();
		}
	}
}
