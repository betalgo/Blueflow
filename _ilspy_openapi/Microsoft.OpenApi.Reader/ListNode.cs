using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal class ListNode : ParseNode, IEnumerable<ParseNode>, IEnumerable
{
	private readonly JsonArray _nodeList;

	public ListNode(ParsingContext context, JsonArray jsonArray)
		: base(context, jsonArray)
	{
		_nodeList = jsonArray;
	}

	public override List<T> CreateList<T>(Func<MapNode, OpenApiDocument, T> map, OpenApiDocument hostDocument)
	{
		if (_nodeList == null)
		{
			throw new OpenApiReaderException("Expected list while parsing " + typeof(T).Name);
		}
		return (from n in _nodeList.OfType<JsonObject>()
			select map(new MapNode(base.Context, n), hostDocument) into i
			where i != null
			select i).ToList();
	}

	public override List<JsonNode> CreateListOfAny()
	{
		return (from n in _nodeList.OfType<JsonNode>()
			select ParseNode.Create(base.Context, n).CreateAny() into i
			where i != null
			select i).ToList();
	}

	public override List<T> CreateSimpleList<T>(Func<ValueNode, OpenApiDocument?, T> map, OpenApiDocument openApiDocument)
	{
		if (_nodeList == null)
		{
			throw new OpenApiReaderException("Expected list while parsing " + typeof(T).Name);
		}
		return (from n in _nodeList.OfType<JsonNode>()
			select map(new ValueNode(base.Context, n), openApiDocument)).ToList();
	}

	public IEnumerator<ParseNode> GetEnumerator()
	{
		return (from n in _nodeList.OfType<JsonNode>()
			select ParseNode.Create(base.Context, n)).ToList().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <summary>
	/// Create a <see cref="T:System.Text.Json.Nodes.JsonArray" />
	/// </summary>
	/// <returns>The created Any object.</returns>
	public override JsonNode CreateAny()
	{
		return _nodeList;
	}
}
