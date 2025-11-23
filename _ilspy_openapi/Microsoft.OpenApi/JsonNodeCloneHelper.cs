using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

internal static class JsonNodeCloneHelper
{
	internal static JsonNode? Clone(JsonNode? value)
	{
		return value?.DeepClone();
	}
}
