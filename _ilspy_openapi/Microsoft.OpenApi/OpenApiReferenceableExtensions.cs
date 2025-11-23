using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Extension methods for resolving references on <see cref="T:Microsoft.OpenApi.IOpenApiReferenceable" /> elements.
/// </summary>
internal static class OpenApiReferenceableExtensions
{
	/// <summary>
	/// Resolves a JSON Pointer with respect to an element, returning the referenced element.
	/// </summary>
	/// <param name="element">The referenceable Open API element on which to apply the JSON pointer</param>
	/// <param name="pointer">a JSON Pointer [RFC 6901](https://tools.ietf.org/html/rfc6901).</param>
	/// <returns>The element pointed to by the JSON pointer.</returns>
	public static IOpenApiReferenceable ResolveReference(this IOpenApiReferenceable element, JsonPointer pointer)
	{
		if (!pointer.Tokens.Any())
		{
			return element;
		}
		string text = pointer.Tokens.FirstOrDefault();
		string text2 = pointer.Tokens.ElementAtOrDefault(1);
		try
		{
			if (text != null && text2 != null)
			{
				if (element is OpenApiHeader headerElement)
				{
					return ResolveReferenceOnHeaderElement(headerElement, text, text2, pointer);
				}
				if (element is OpenApiParameter parameterElement)
				{
					return ResolveReferenceOnParameterElement(parameterElement, text, text2, pointer);
				}
				if (element is OpenApiResponse responseElement)
				{
					return ResolveReferenceOnResponseElement(responseElement, text, text2, pointer);
				}
			}
		}
		catch (KeyNotFoundException)
		{
			throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
		}
		throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
	}

	private static IOpenApiReferenceable ResolveReferenceOnHeaderElement(OpenApiHeader headerElement, string propertyName, string mapKey, JsonPointer pointer)
	{
		if ("examples".Equals(propertyName, StringComparison.Ordinal) && !string.IsNullOrEmpty(mapKey) && headerElement?.Examples != null && headerElement.Examples.TryGetValue(mapKey, out IOpenApiExample value))
		{
			return value;
		}
		throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
	}

	private static IOpenApiReferenceable ResolveReferenceOnParameterElement(OpenApiParameter parameterElement, string propertyName, string mapKey, JsonPointer pointer)
	{
		if ("examples".Equals(propertyName, StringComparison.Ordinal) && !string.IsNullOrEmpty(mapKey) && parameterElement?.Examples != null && parameterElement.Examples.TryGetValue(mapKey, out IOpenApiExample value))
		{
			return value;
		}
		throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
	}

	private static IOpenApiReferenceable ResolveReferenceOnResponseElement(OpenApiResponse responseElement, string propertyName, string mapKey, JsonPointer pointer)
	{
		if (!string.IsNullOrEmpty(mapKey))
		{
			if ("headers".Equals(propertyName, StringComparison.Ordinal) && responseElement?.Headers != null && responseElement.Headers.TryGetValue(mapKey, out IOpenApiHeader value))
			{
				return value;
			}
			if ("links".Equals(propertyName, StringComparison.Ordinal) && responseElement?.Links != null && responseElement.Links.TryGetValue(mapKey, out IOpenApiLink value2))
			{
				return value2;
			}
		}
		throw new OpenApiException(string.Format(SRResource.InvalidReferenceId, pointer));
	}
}
