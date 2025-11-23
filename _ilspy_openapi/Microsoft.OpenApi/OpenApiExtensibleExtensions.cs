using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Extension methods to verify validity and add an extension to Extensions property.
/// </summary>
public static class OpenApiExtensibleExtensions
{
	/// <summary>
	/// Add extension into the Extensions
	/// </summary>
	/// <typeparam name="T"><see cref="T:Microsoft.OpenApi.IOpenApiExtensible" />.</typeparam>
	/// <param name="element">The extensible Open API element. </param>
	/// <param name="name">The extension name.</param>
	/// <param name="any">The extension value.</param>
	public static void AddExtension<T>(this T element, string name, IOpenApiExtension any) where T : IOpenApiExtensible
	{
		Utils.CheckArgumentNull(element, "element");
		Utils.CheckArgumentNullOrEmpty(name, "name");
		if (!name.StartsWith("x-", StringComparison.OrdinalIgnoreCase))
		{
			throw new OpenApiException(string.Format(SRResource.ExtensionFieldNameMustBeginWithXDash, name));
		}
		ref T reference;
		if (default(T) == null)
		{
			T val = element;
			reference = ref val;
		}
		else
		{
			reference = ref element;
		}
		ref T reference2 = ref reference;
		if (reference2.Extensions == null)
		{
			IDictionary<string, IOpenApiExtension> dictionary;
			IDictionary<string, IOpenApiExtension> extensions = (dictionary = new Dictionary<string, IOpenApiExtension>());
			reference2.Extensions = extensions;
		}
		element.Extensions[name] = Utils.CheckArgumentNull(any, "any");
	}
}
