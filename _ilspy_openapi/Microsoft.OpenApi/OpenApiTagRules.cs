namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiTag" />.
/// </summary>
[OpenApiRule]
public static class OpenApiTagRules
{
	/// <summary>
	/// Validate the field is required.
	/// </summary>
	public static ValidationRule<OpenApiTag> TagRequiredFields => new ValidationRule<OpenApiTag>("TagRequiredFields", delegate(IValidationContext context, OpenApiTag tag)
	{
		if (tag.Name == null)
		{
			context.Enter("name");
			context.CreateError("TagRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "name", "tag"));
			context.Exit();
		}
	});
}
