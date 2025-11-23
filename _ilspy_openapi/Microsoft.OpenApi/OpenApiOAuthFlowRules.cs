namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlow" />.
/// </summary>
[OpenApiRule]
public static class OpenApiOAuthFlowRules
{
	/// <summary>
	/// Validate the field is required.
	/// </summary>
	public static ValidationRule<OpenApiOAuthFlow> OAuthFlowRequiredFields => new ValidationRule<OpenApiOAuthFlow>("OAuthFlowRequiredFields", delegate(IValidationContext context, OpenApiOAuthFlow flow)
	{
		if (flow.AuthorizationUrl == null)
		{
			context.Enter("authorizationUrl");
			context.CreateError("OAuthFlowRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "authorizationUrl", "OAuth Flow"));
			context.Exit();
		}
		if (flow.TokenUrl == null)
		{
			context.Enter("tokenUrl");
			context.CreateError("OAuthFlowRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "tokenUrl", "OAuth Flow"));
			context.Exit();
		}
		if (flow.Scopes == null)
		{
			context.Enter("scopes");
			context.CreateError("OAuthFlowRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "scopes", "OAuth Flow"));
			context.Exit();
		}
	});
}
