namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiInfo" />.
/// </summary>
[OpenApiRule]
public static class OpenApiInfoRules
{
	/// <summary>
	/// Validate the field is required.
	/// </summary>
	public static ValidationRule<OpenApiInfo> InfoRequiredFields => new ValidationRule<OpenApiInfo>("InfoRequiredFields", delegate(IValidationContext context, OpenApiInfo item)
	{
		if (item.Title == null)
		{
			context.Enter("title");
			context.CreateError("InfoRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "title", "info"));
			context.Exit();
		}
		if (item.Version == null)
		{
			context.Enter("version");
			context.CreateError("InfoRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "version", "info"));
			context.Exit();
		}
	});
}
