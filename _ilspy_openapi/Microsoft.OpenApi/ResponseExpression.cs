namespace Microsoft.OpenApi;

/// <summary>
/// $response. expression.
/// </summary>
public sealed class ResponseExpression : RuntimeExpression
{
	/// <summary>
	/// $response. string
	/// </summary>
	public const string Response = "$response.";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression => "$response." + Source.Expression;

	/// <summary>
	/// The <see cref="T:Microsoft.OpenApi.SourceExpression" /> expression.
	/// </summary>
	public SourceExpression Source { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.ResponseExpression" /> class.
	/// </summary>
	/// <param name="source">The source of the response.</param>
	public ResponseExpression(SourceExpression source)
	{
		Source = Utils.CheckArgumentNull(source, "source");
	}
}
