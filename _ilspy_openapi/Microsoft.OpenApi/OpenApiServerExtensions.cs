using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Extension methods for <see cref="T:Microsoft.OpenApi.OpenApiServer" /> serialization.
/// </summary>
public static class OpenApiServerExtensions
{
	/// <summary>
	/// Replaces URL variables in a server's URL
	/// </summary>
	/// <param name="server">The OpenAPI server object</param>
	/// <param name="values">The server variable values that will be used to replace the default values.</param>
	/// <returns>A URL with the provided variables substituted.</returns>
	/// <exception cref="T:System.ArgumentException">
	/// Thrown when:
	///   1. A substitution has no valid value in both the supplied dictionary and the default
	///   2. A substitution's value is not available in the enum provided
	/// </exception>
	public static string? ReplaceServerUrlVariables(this OpenApiServer server, Dictionary<string, string>? values = null)
	{
		string text = server.Url;
		if (server.Variables != null && text != null)
		{
			foreach (KeyValuePair<string, OpenApiServerVariable> variable in server.Variables)
			{
				string value;
				if (values != null)
				{
					if (values.TryGetValue(variable.Key, out value) && !string.IsNullOrEmpty(value))
					{
						goto IL_005d;
					}
				}
				value = variable.Value.Default;
				goto IL_005d;
				IL_005d:
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException(string.Format(SRResource.ParseServerUrlDefaultValueNotAvailable, variable.Key), "server");
				}
				if (value != null)
				{
					List<string> list = variable.Value.Enum;
					if (list != null && (list.Count == 0 || !list.Contains(value)))
					{
						throw new ArgumentException(string.Format(SRResource.ParseServerUrlValueNotValid, value, variable.Key), "values");
					}
				}
				text = text?.Replace("{" + variable.Key + "}", value);
			}
		}
		return text;
	}
}
