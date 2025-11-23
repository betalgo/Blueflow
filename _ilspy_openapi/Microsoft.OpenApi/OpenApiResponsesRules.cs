using System;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;

namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiResponses" />.
/// </summary>
[OpenApiRule]
public static class OpenApiResponsesRules
{
	/// <summary>
	/// An OpenAPI operation must contain at least one response
	/// </summary>
	public static ValidationRule<OpenApiResponses> ResponsesMustContainAtLeastOneResponse => new ValidationRule<OpenApiResponses>("ResponsesMustContainAtLeastOneResponse", delegate(IValidationContext context, OpenApiResponses responses)
	{
		if (responses.Count == 0)
		{
			context.CreateError("ResponsesMustContainAtLeastOneResponse", "Responses must contain at least one response");
		}
	});

	/// <summary>
	/// The response key must either be "default" or an HTTP status code (1xx, 2xx, 3xx, 4xx, 5xx).
	/// </summary>
	public static ValidationRule<OpenApiResponses> ResponsesMustBeIdentifiedByDefaultOrStatusCode => new ValidationRule<OpenApiResponses>("ResponsesMustBeIdentifiedByDefaultOrStatusCode", delegate(IValidationContext context, OpenApiResponses responses)
	{
		foreach (string key in responses.Keys)
		{
			if (!"default".Equals(key, StringComparison.OrdinalIgnoreCase) && !StatusCodeRegex().IsMatch(key))
			{
				context.Enter(key);
				context.CreateError("ResponsesMustBeIdentifiedByDefaultOrStatusCode", "Responses key must be 'default', an HTTP status code, or one of the following strings representing a range of HTTP status codes: '1XX', '2XX', '3XX', '4XX', '5XX' (case insensitive)");
				context.Exit();
			}
		}
	});

	/// <remarks>
	/// Pattern:<br />
	/// <code>^[1-5](?&gt;[0-9]{2}|[xX]{2})$</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match if at the beginning of the string.<br />
	/// ○ Match a character in the set [1-5].<br />
	/// ○ Match with 2 alternative expressions, atomically.<br />
	///     ○ Match a character in the set [0-9] exactly 2 times.<br />
	///     ○ Match a character in the set [Xx] exactly 2 times.<br />
	/// ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	[GeneratedRegex("^[1-5](?>[0-9]{2}|[xX]{2})$", RegexOptions.None, 100)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "8.0.13.2707")]
	internal static Regex StatusCodeRegex()
	{
		return _003CRegexGenerator_g_003EF372293D74F2F37DF39F66BF77797D52DA230767FFF375F6C4111DF72DE8B2285__StatusCodeRegex_0.Instance;
	}
}
