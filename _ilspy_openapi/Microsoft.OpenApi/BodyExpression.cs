namespace Microsoft.OpenApi;

/// <summary>
/// Body expression.
/// </summary>
public sealed class BodyExpression : SourceExpression
{
	/// <summary>
	/// body string
	/// </summary>
	public const string Body = "body";

	/// <summary>
	/// Prefix for a pointer
	/// </summary>
	public const string PointerPrefix = "#";

	/// <summary>
	/// Gets the expression string.
	/// </summary>
	public override string Expression
	{
		get
		{
			if (string.IsNullOrWhiteSpace(base.Value))
			{
				return "body";
			}
			return "body#" + base.Value;
		}
	}

	/// <summary>
	/// Gets the fragment string.
	/// </summary>
	public string? Fragment => base.Value;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.BodyExpression" /> class.
	/// </summary>
	public BodyExpression()
		: base(null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.BodyExpression" /> class.
	/// </summary>
	/// <param name="pointer">a JSON Pointer [RFC 6901](https://tools.ietf.org/html/rfc6901).</param>
	public BodyExpression(JsonPointer? pointer)
		: base(pointer?.ToString())
	{
		Utils.CheckArgumentNull(pointer, "pointer");
	}
}
