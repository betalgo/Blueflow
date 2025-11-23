namespace Microsoft.OpenApi;

/// <summary>
/// Query expression, the name in query is case-sensitive.
/// </summary>
public sealed class QueryExpression : SourceExpression
{
	/// <summary>
	/// query. string
	/// </summary>
	public const string Query = "query.";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression => "query." + base.Value;

	/// <summary>
	/// Gets the name string.
	/// </summary>
	public string? Name => base.Value;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.QueryExpression" /> class.
	/// </summary>
	/// <param name="name">The name string, it's case-insensitive.</param>
	public QueryExpression(string name)
		: base(name)
	{
		Utils.CheckArgumentNullOrEmpty(name, "name");
	}
}
