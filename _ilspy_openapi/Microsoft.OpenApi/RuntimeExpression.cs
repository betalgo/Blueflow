using System;

namespace Microsoft.OpenApi;

/// <summary>
/// Base class for the Open API runtime expression.
/// </summary>
public abstract class RuntimeExpression : IEquatable<RuntimeExpression>
{
	/// <summary>
	/// The dollar sign prefix for a runtime expression.
	/// </summary>
	public const string Prefix = "$";

	/// <summary>
	/// The expression string.
	/// </summary>
	public abstract string Expression { get; }

	/// <summary>
	/// Build the runtime expression from input string.
	/// </summary>
	/// <param name="expression">The runtime expression.</param>
	/// <returns>The built runtime expression object.</returns>
	public static RuntimeExpression Build(string expression)
	{
		Utils.CheckArgumentNullOrEmpty(expression, "expression");
		if (!expression.StartsWith("$", StringComparison.OrdinalIgnoreCase))
		{
			return new CompositeExpression(expression);
		}
		if (expression.Equals("$url", StringComparison.Ordinal))
		{
			return new UrlExpression();
		}
		if (expression.Equals("$method", StringComparison.Ordinal))
		{
			return new MethodExpression();
		}
		if (expression.Equals("$statusCode", StringComparison.Ordinal))
		{
			return new StatusCodeExpression();
		}
		if (expression.StartsWith("$request.", StringComparison.Ordinal))
		{
			return new RequestExpression(SourceExpression.Build(expression.Substring("$request.".Length)));
		}
		if (expression.StartsWith("$response.", StringComparison.Ordinal))
		{
			return new ResponseExpression(SourceExpression.Build(expression.Substring("$response.".Length)));
		}
		throw new OpenApiException(string.Format(SRResource.RuntimeExpressionHasInvalidFormat, expression));
	}

	/// <summary>
	/// GetHashCode implementation for IEquatable.
	/// </summary>
	public override int GetHashCode()
	{
		return StringComparer.Ordinal.GetHashCode(Expression);
	}

	/// <summary>
	/// Equals implementation for IEquatable.
	/// </summary>
	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() == GetType())
		{
			return Equals((RuntimeExpression)obj);
		}
		return false;
	}

	/// <inheritdoc />
	public bool Equals(RuntimeExpression? other)
	{
		if (other != null)
		{
			return StringComparer.Ordinal.Equals(Expression, other.Expression);
		}
		return false;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return Expression;
	}
}
