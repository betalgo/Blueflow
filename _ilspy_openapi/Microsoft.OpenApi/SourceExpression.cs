using System;

namespace Microsoft.OpenApi;

/// <summary>
/// Source expression.
/// </summary>
public abstract class SourceExpression : RuntimeExpression
{
	/// <summary>
	/// Gets the expression string.
	/// </summary>
	protected string? Value { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.QueryExpression" /> class.
	/// </summary>
	/// <param name="value">The value string.</param>
	protected SourceExpression(string? value)
	{
		Value = value;
	}

	/// <summary>
	/// Build the source expression from input string.
	/// </summary>
	/// <param name="expression">The source expression.</param>
	/// <returns>The built source expression.</returns>
	public new static SourceExpression Build(string expression)
	{
		if (!string.IsNullOrWhiteSpace(expression))
		{
			string[] array = expression.Split('.');
			if (array.Length == 2)
			{
				if (expression.StartsWith("header.", StringComparison.Ordinal))
				{
					return new HeaderExpression(array[1]);
				}
				if (expression.StartsWith("query.", StringComparison.Ordinal))
				{
					return new QueryExpression(array[1]);
				}
				if (expression.StartsWith("path.", StringComparison.Ordinal))
				{
					return new PathExpression(array[1]);
				}
			}
			if (expression.StartsWith("body", StringComparison.Ordinal))
			{
				string text = expression.Substring("body".Length);
				if (string.IsNullOrEmpty(text))
				{
					return new BodyExpression();
				}
				return new BodyExpression(new JsonPointer(text));
			}
		}
		throw new OpenApiException(string.Format(SRResource.SourceExpressionHasInvalidFormat, expression));
	}
}
