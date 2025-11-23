using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi;

/// <summary>
/// String extension methods.
/// </summary>
internal static class StringExtensions
{
	private static readonly ConcurrentDictionary<Type, ReadOnlyDictionary<string, object>> EnumDisplayCache = new ConcurrentDictionary<Type, ReadOnlyDictionary<string, object>>();

	internal static bool TryGetEnumFromDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this string? displayName, ParsingContext parsingContext, out T? result) where T : Enum
	{
		if (displayName.TryGetEnumFromDisplayName<T>(out result))
		{
			return true;
		}
		parsingContext.Diagnostic.Errors.Add(new OpenApiError(parsingContext.GetLocation(), "Enum value " + displayName + " is not recognized."));
		return false;
	}

	internal static bool TryGetEnumFromDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this string? displayName, out T? result) where T : Enum
	{
		Type type = typeof(T);
		ReadOnlyDictionary<string, object> orAdd = EnumDisplayCache.GetOrAdd(type, (Type _) => GetEnumValues<T>(type));
		if (displayName != null && orAdd.TryGetValue(displayName, out var value))
		{
			result = (T)value;
			return true;
		}
		result = default(T);
		return false;
	}

	private static ReadOnlyDictionary<string, object> GetEnumValues<T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type enumType) where T : Enum
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			DisplayAttribute customAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
			if (customAttribute != null && fieldInfo.GetValue(null) is T val && customAttribute.Name != null)
			{
				dictionary.Add(customAttribute.Name, val);
			}
		}
		return new ReadOnlyDictionary<string, object>(dictionary);
	}

	internal static string ToFirstCharacterLowerCase(this string input)
	{
		if (!string.IsNullOrEmpty(input))
		{
			return char.ToLowerInvariant(input[0]) + input.Substring(1);
		}
		return string.Empty;
	}
}
