using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.OpenApi;

/// <summary>
/// Utilities methods
/// </summary>
internal static class Utils
{
	/// <summary>
	/// Check whether the input argument value is null or not.
	/// </summary>
	/// <typeparam name="T">The input value type.</typeparam>
	/// <param name="value">The input value.</param>
	/// <param name="parameterName">The input parameter name.</param>
	/// <returns>The input value.</returns>
	internal static T CheckArgumentNull<T>([NotNull] T value, [CallerArgumentExpression("value")] string parameterName = "")
	{
		if (value != null)
		{
			return value;
		}
		throw new ArgumentNullException(parameterName, "Value cannot be null: " + parameterName);
	}

	/// <summary>
	/// Check whether the input string value is null or empty.
	/// </summary>
	/// <param name="value">The input string value.</param>
	/// <param name="parameterName">The input parameter name.</param>
	/// <returns>The input value.</returns>
	internal static string CheckArgumentNullOrEmpty([NotNull] string value, [CallerArgumentExpression("value")] string parameterName = "")
	{
		if (!string.IsNullOrWhiteSpace(value))
		{
			return value;
		}
		throw new ArgumentNullException(parameterName, "Value cannot be null or empty: " + parameterName);
	}
}
