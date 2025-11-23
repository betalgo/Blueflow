namespace Microsoft.OpenApi;

/// <summary>
/// Header expression, The token identifier in header is case-insensitive.
/// </summary>
public class HeaderExpression : SourceExpression
{
	/// <summary>
	/// header. string
	/// </summary>
	public const string Header = "header.";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression => "header." + base.Value;

	/// <summary>
	/// Gets the token string.
	/// </summary>
	public string? Token => base.Value;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.HeaderExpression" /> class.
	/// </summary>
	/// <param name="token">The token string, it's case-insensitive.</param>
	public HeaderExpression(string token)
		: base(token)
	{
		Utils.CheckArgumentNullOrEmpty(token, "token");
	}
}
