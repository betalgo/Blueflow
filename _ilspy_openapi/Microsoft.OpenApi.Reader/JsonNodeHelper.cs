using System;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal static class JsonNodeHelper
{
	public static string? GetScalarValue(this JsonNode node)
	{
		if (!(node is JsonValue jsonValue))
		{
			throw new OpenApiException("Expected scalar value.");
		}
		return Convert.ToString(jsonValue?.GetValue<object>(), CultureInfo.InvariantCulture);
	}
}
