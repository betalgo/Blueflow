using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Microsoft.OpenApi;

/// <summary>
/// Enumeration type extension methods.
/// </summary>
public static class EnumExtensions
{
	private static readonly ConcurrentDictionary<Enum, string> DisplayNameCache = new ConcurrentDictionary<Enum, string>();

	/// <summary>
	/// Gets an attribute on an enum field value.
	/// </summary>
	/// <typeparam name="T">The type of the attribute to retrieve.</typeparam>
	/// <param name="enumValue">The enum value.</param>
	/// <returns>
	/// The attribute of the specified type or null.
	/// </returns>
	[UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "Fields are never trimmed for enum types.")]
	public static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
	{
		FieldInfo field = enumValue.GetType().GetField(enumValue.ToString(), BindingFlags.Static | BindingFlags.Public);
		if (field == null)
		{
			return null;
		}
		return field.GetCustomAttributes<T>(inherit: false).FirstOrDefault();
	}

	/// <summary>
	/// Gets the enum display name.
	/// </summary>
	/// <param name="enumValue">The enum value.</param>
	/// <returns>
	/// Use <see cref="T:Microsoft.OpenApi.DisplayAttribute" /> if it exists.
	/// Otherwise, use the standard string representation.
	/// </returns>
	public static string GetDisplayName(this Enum enumValue)
	{
		return DisplayNameCache.GetOrAdd(enumValue, delegate(Enum e)
		{
			DisplayAttribute attributeOfType = e.GetAttributeOfType<DisplayAttribute>();
			return (attributeOfType?.Name == null) ? e.ToString() : attributeOfType.Name;
		});
	}
}
