namespace Microsoft.OpenApi;

/// <summary>
/// Path expression, the name in path is case-sensitive.
/// </summary>
public sealed class PathExpression : SourceExpression
{
	/// <summary>
	/// path. string
	/// </summary>
	public const string Path = "path.";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression => "path." + base.Value;

	/// <summary>
	/// Gets the name string.
	/// </summary>
	public string? Name => base.Value;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.PathExpression" /> class.
	/// </summary>
	/// <param name="name">The name string, it's case-insensitive.</param>
	public PathExpression(string name)
		: base(name)
	{
		Utils.CheckArgumentNullOrEmpty(name, "name");
	}
}
