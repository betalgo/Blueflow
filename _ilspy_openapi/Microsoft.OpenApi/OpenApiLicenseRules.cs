namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiLicense" />.
/// </summary>
[OpenApiRule]
public static class OpenApiLicenseRules
{
	/// <summary>
	/// REQUIRED.
	/// </summary>
	public static ValidationRule<OpenApiLicense> LicenseRequiredFields => new ValidationRule<OpenApiLicense>("LicenseRequiredFields", delegate(IValidationContext context, OpenApiLicense license)
	{
		if (license.Name == null)
		{
			context.Enter("name");
			context.CreateError("LicenseRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "name", "license"));
			context.Exit();
		}
	});
}
