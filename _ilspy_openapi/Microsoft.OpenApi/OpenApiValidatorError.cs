namespace Microsoft.OpenApi;

/// <summary>
/// Errors detected when validating an OpenAPI Element
/// </summary>
public class OpenApiValidatorError : OpenApiError
{
	/// <summary>
	/// Name of rule that detected the error.
	/// </summary>
	public string RuleName { get; set; }

	/// <summary>
	/// Initializes the <see cref="T:Microsoft.OpenApi.OpenApiError" /> class.
	/// </summary>
	public OpenApiValidatorError(string ruleName, string pointer, string message)
		: base(pointer, message)
	{
		RuleName = ruleName;
	}
}
