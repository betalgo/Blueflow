using System.Collections.Generic;

internal static class IDiagnosticExtensions
{
	internal static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
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
