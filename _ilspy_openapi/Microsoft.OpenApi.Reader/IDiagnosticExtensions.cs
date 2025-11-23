using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Extension class for IList to add the Method "AddRange" used above
/// </summary>
internal static class IDiagnosticExtensions
{
	/// <summary>
	/// Extension method for IList so that another list can be added to the current list.
	/// </summary>
	/// <param name="collection"></param>
	/// <param name="enumerable"></param>
	/// <typeparam name="T"></typeparam>
	public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
	{
		if (collection == null || enumerable == null)
		{
			return;
		}
		foreach (T item in enumerable)
		{
			collection.Add(item);
		}
	}
}
