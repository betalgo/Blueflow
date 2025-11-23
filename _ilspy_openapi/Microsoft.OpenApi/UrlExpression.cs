namespace Microsoft.OpenApi;

/// <summary>
/// Url expression.
/// </summary>
public sealed class UrlExpression : RuntimeExpression
{
	/// <summary>
	/// $url string.
	/// </summary>
	public const string Url = "$url";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression => "$url";

	/// <summary>
	/// Private constructor.
	/// </summary>
	public UrlExpression()
	{
	}
}
