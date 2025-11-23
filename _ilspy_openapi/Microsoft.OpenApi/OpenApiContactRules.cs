namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiContact" />.
/// </summary>
[OpenApiRule]
public static class OpenApiContactRules
{
	/// <summary>
	/// Email field MUST be email address.
	/// </summary>
	public static ValidationRule<OpenApiContact> EmailMustBeEmailFormat => new ValidationRule<OpenApiContact>("EmailMustBeEmailFormat", delegate(IValidationContext context, OpenApiContact item)
	{
		if (item != null && item.Email != null && !item.Email.IsEmailAddress())
		{
			context.Enter("email");
			context.CreateError("EmailMustBeEmailFormat", string.Format(SRResource.Validation_StringMustBeEmailAddress, item.Email));
			context.Exit();
		}
	});
}
