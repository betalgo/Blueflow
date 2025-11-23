using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiPaths" />.
/// </summary>
[OpenApiRule]
public static class OpenApiPathsRules
{
	/// <summary>
	/// A relative path to an individual endpoint. The field name MUST begin with a slash.
	/// </summary>
	public static ValidationRule<OpenApiPaths> PathNameMustBeginWithSlash => new ValidationRule<OpenApiPaths>("PathNameMustBeginWithSlash", delegate(IValidationContext context, OpenApiPaths item)
	{
		foreach (string key in item.Keys)
		{
			context.Enter(key);
			if (key == null || !key.StartsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				context.CreateError("PathNameMustBeginWithSlash", string.Format(SRResource.Validation_PathItemMustBeginWithSlash, key));
			}
			context.Exit();
		}
	});

	/// <summary>
	/// A relative path to an individual endpoint. The field name MUST begin with a slash.
	/// </summary>
	public static ValidationRule<OpenApiPaths> PathMustBeUnique => new ValidationRule<OpenApiPaths>("PathMustBeUnique", delegate(IValidationContext context, OpenApiPaths item)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string key in item.Keys)
		{
			string pathSignature = GetPathSignature(key);
			if (!hashSet.Add(pathSignature))
			{
				context.Enter(key);
				context.CreateError("PathMustBeUnique", string.Format(SRResource.Validation_PathSignatureMustBeUnique, pathSignature));
				context.Exit();
			}
		}
	});

	/// <summary>
	///  Replaces placeholders in the path with {}, e.g. /pets/{petId} becomes /pets/{} .
	/// </summary>
	/// <param name="path">The input path</param>
	/// <returns>The path signature</returns>
	private static string GetPathSignature(string path)
	{
		for (int num = path.IndexOf('{'); num > -1; num = path.IndexOf('{', num + 2))
		{
			int num2 = path.IndexOf('}', num);
			if (num2 < 0)
			{
				return path;
			}
			path = path.Substring(0, num + 1) + path.Substring(num2);
		}
		return path;
	}
}
