using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// This comparer is used to maintain a globally unique list of tags encountered
/// in a particular OpenAPI document.
/// </summary>
internal sealed class OpenApiTagComparer : IEqualityComparer<IOpenApiTag>
{
	private static readonly Lazy<OpenApiTagComparer> _lazyInstance = new Lazy<OpenApiTagComparer>(() => new OpenApiTagComparer());

	internal static readonly StringComparer StringComparer = System.StringComparer.Ordinal;

	/// <summary>
	/// Default instance for the comparer.
	/// </summary>
	internal static OpenApiTagComparer Instance => _lazyInstance.Value;

	/// <inheritdoc />
	public bool Equals(IOpenApiTag? x, IOpenApiTag? y)
	{
		if (x == null && y == null)
		{
			return true;
		}
		if (x == null || y == null)
		{
			return false;
		}
		if (x == y)
		{
			return true;
		}
		if (x is OpenApiTagReference openApiTagReference && y is OpenApiTagReference openApiTagReference2)
		{
			return StringComparer.Equals(openApiTagReference.Name ?? openApiTagReference.Reference.Id, openApiTagReference2.Name ?? openApiTagReference2.Reference.Id);
		}
		return StringComparer.Equals(x.Name, y.Name);
	}

	/// <inheritdoc />
	public int GetHashCode(IOpenApiTag obj)
	{
		string text = obj?.Name;
		if (text == null && obj is OpenApiTagReference openApiTagReference)
		{
			text = openApiTagReference.Reference.Id;
		}
		if (!string.IsNullOrEmpty(text))
		{
			return StringComparer.GetHashCode(text);
		}
		return 0;
	}
}
