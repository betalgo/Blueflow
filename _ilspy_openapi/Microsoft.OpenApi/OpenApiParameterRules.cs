namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiParameter" />.
/// </summary>
[OpenApiRule]
public static class OpenApiParameterRules
{
	/// <summary>
	/// Validate the field is required.
	/// </summary>
	public static ValidationRule<IOpenApiParameter> ParameterRequiredFields => new ValidationRule<IOpenApiParameter>("ParameterRequiredFields", delegate(IValidationContext context, IOpenApiParameter item)
	{
		if (item.Name == null)
		{
			context.Enter("name");
			context.CreateError("ParameterRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "name", "parameter"));
			context.Exit();
		}
		if (!item.In.HasValue)
		{
			context.Enter("in");
			context.CreateError("ParameterRequiredFields", string.Format(SRResource.Validation_FieldIsRequired, "in", "parameter"));
			context.Exit();
		}
	});

	/// <summary>
	/// Validate the "required" field is true when "in" is path.
	/// </summary>
	public static ValidationRule<IOpenApiParameter> RequiredMustBeTrueWhenInIsPath => new ValidationRule<IOpenApiParameter>("RequiredMustBeTrueWhenInIsPath", delegate(IValidationContext context, IOpenApiParameter item)
	{
		if (item.In == ParameterLocation.Path && !item.Required)
		{
			context.Enter("required");
			context.CreateError("RequiredMustBeTrueWhenInIsPath", "\"required\" must be true when parameter location is \"path\"");
			context.Exit();
		}
	});

	/// <summary>
	/// Validate that a path parameter should always appear in the path
	/// </summary>
	public static ValidationRule<IOpenApiParameter> PathParameterShouldBeInThePath => new ValidationRule<IOpenApiParameter>("PathParameterShouldBeInThePath", delegate(IValidationContext context, IOpenApiParameter parameter)
	{
		if (parameter.In == ParameterLocation.Path && !context.PathString.Contains("{" + parameter.Name + "}") && !context.PathString.Contains("#/components"))
		{
			context.Enter("in");
			context.CreateError("PathParameterShouldBeInThePath", "Declared path parameter \"" + parameter.Name + "\" needs to be defined as a path parameter at either the path or operation level");
			context.Exit();
		}
	});
}
