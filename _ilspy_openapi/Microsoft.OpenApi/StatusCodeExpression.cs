namespace Microsoft.OpenApi;

/// <summary>
/// StatusCode expression.
/// </summary>
public sealed class StatusCodeExpression : RuntimeExpression
{
	/// <summary>
	/// $statusCode string.
	/// </summary>
	public const string StatusCode = "$statusCode";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression => "$statusCode";

	/// <summary>
	/// Private constructor.
	/// </summary>
	public StatusCodeExpression()
	{
	}
}
