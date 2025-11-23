using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Wrapper class around JsonDocument to isolate semantic parsing from details of Json DOM.
/// </summary>
internal class RootNode : ParseNode
{
	private readonly JsonNode _jsonNode;

	public RootNode(ParsingContext context, JsonNode jsonNode)
		: base(context, jsonNode)
	{
		_jsonNode = jsonNode;
	}

	public ParseNode? Find(JsonPointer referencePointer)
	{
		JsonNode jsonNode = referencePointer.Find(_jsonNode);
		if (jsonNode == null)
		{
			return null;
		}
		return ParseNode.Create(base.Context, jsonNode);
	}

	public MapNode GetMap()
	{
		return new MapNode(base.Context, _jsonNode);
	}
}
