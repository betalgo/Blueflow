namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiResponse" />.
/// </summary>
[OpenApiRule]
public static class OpenApiResponseRules
{
	/// <summary>
	/// Validate the field is required.
	/// </summary>
	public static ValidationRule<IOpenApiResponse> ResponseRequiredFields => new ValidationRule<IOpenApiResponse>("ResponseRequiredFields", delegate(IValidationContext context, IOpenApiResponse response)
	{
		if (response.Description == null)
		{
			context.Enter("description");
			context.CreateError("ResponseRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "description", "response"));
			context.Exit();
		}
	});
}
