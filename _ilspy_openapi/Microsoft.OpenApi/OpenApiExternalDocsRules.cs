namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" />.
/// </summary>
[OpenApiRule]
public static class OpenApiExternalDocsRules
{
	/// <summary>
	/// Validate the field is required.
	/// </summary>
	public static ValidationRule<OpenApiExternalDocs> UrlIsRequired => new ValidationRule<OpenApiExternalDocs>("UrlIsRequired", delegate(IValidationContext context, OpenApiExternalDocs item)
	{
		if (item.Url == null)
		{
			context.Enter("url");
			context.CreateError("UrlIsRequired", string.Format(SRResource.Validation_FieldIsRequired, "url", "External Documentation"));
			context.Exit();
		}
	});
}
