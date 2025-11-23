namespace Microsoft.OpenApi;

/// <summary>
/// $request. expression.
/// </summary>
public sealed class RequestExpression : RuntimeExpression
{
	/// <summary>
	/// $request. string
	/// </summary>
	public const string Request = "$request.";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression => "$request." + Source.Expression;

	/// <summary>
	/// The <see cref="T:Microsoft.OpenApi.SourceExpression" /> expression.
	/// </summary>
	public SourceExpression Source { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.RequestExpression" /> class.
	/// </summary>
	/// <param name="source">The source of the request.</param>
	public RequestExpression(SourceExpression source)
	{
		Source = Utils.CheckArgumentNull(source, "source");
	}
}
