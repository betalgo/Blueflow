using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi;

/// <summary>
/// String literal with embedded expressions
/// </summary>
public class CompositeExpression : RuntimeExpression
{
	private readonly string template;

	private readonly Regex expressionPattern = new Regex("{(?<exp>\\$[^}]*)");

	/// <summary>
	/// Expressions embedded into string literal
	/// </summary>
	public List<RuntimeExpression> ContainedExpressions { get; } = new List<RuntimeExpression>();

	/// <summary>
	/// Return original string literal with embedded expression
	/// </summary>
	public override string Expression => template;

	/// <summary>
	/// Create a composite expression from a string literal with an embedded expression
	/// </summary>
	/// <param name="expression"></param>
	public CompositeExpression(string expression)
	{
		template = expression;
		foreach (Match item in expressionPattern.Matches(expression).Cast<Match>())
		{
			string value = item.Groups["exp"].Captures.Cast<Capture>().First().Value;
			ContainedExpressions.Add(RuntimeExpression.Build(value));
		}
	}
}
