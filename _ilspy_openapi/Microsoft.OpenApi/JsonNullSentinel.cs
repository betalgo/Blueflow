using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// A sentinel value representing JSON null. 
/// This can only be used for OpenAPI properties of type <see cref="T:System.Text.Json.Nodes.JsonNode" />
/// </summary>
public static class JsonNullSentinel
{
	private const string SentinelValue = "openapi-json-null-sentinel-value-2BF93600-0FE4-4250-987A-E5DDB203E464";

	private static readonly JsonValue SentinelJsonValue = JsonValue.Create("openapi-json-null-sentinel-value-2BF93600-0FE4-4250-987A-E5DDB203E464");

	/// <summary>
	/// A sentinel value representing JSON null. 
	/// This can only be used for OpenAPI properties of type <see cref="T:System.Text.Json.Nodes.JsonNode" />.
	/// This can only be used for the root level of a JSON structure.
	/// Any use outside of these constraints is unsupported and may lead to unexpected behavior.
	/// Because the value might be cloned, so the value can be added in a tree, reference equality checks will not work.
	/// You must use the <see cref="M:Microsoft.OpenApi.JsonNullSentinel.IsJsonNullSentinel(System.Text.Json.Nodes.JsonNode)" /> method to check for this sentinel.
	/// </summary>
	public static JsonValue JsonNull => SentinelJsonValue;

	/// <summary>
	/// Determines if the given node is the JSON null sentinel.
	/// </summary>
	/// <param name="node">The JsonNode to check.</param>
	/// <returns>Whether or not the given node is the JSON null sentinel.</returns>
	public static bool IsJsonNullSentinel(this JsonNode? node)
	{
		if (node != SentinelJsonValue)
		{
			if (node != null && node.GetValueKind() == JsonValueKind.String)
			{
				return JsonNode.DeepEquals(SentinelJsonValue, node);
			}
			return false;
		}
		return true;
	}
}
