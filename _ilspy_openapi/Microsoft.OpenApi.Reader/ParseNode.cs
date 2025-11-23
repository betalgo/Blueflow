using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal abstract class ParseNode
{
	public ParsingContext Context { get; }

	public JsonNode JsonNode { get; }

	protected ParseNode(ParsingContext parsingContext, JsonNode jsonNode)
	{
		Context = parsingContext;
		JsonNode = jsonNode;
	}

	public MapNode CheckMapNode(string nodeName)
	{
		return (this as MapNode) ?? throw new OpenApiReaderException(nodeName + " must be a map/object", Context);
	}

	public static ParseNode Create(ParsingContext context, JsonNode node)
	{
		if (node is JsonArray jsonArray)
		{
			return new ListNode(context, jsonArray);
		}
		if (node is JsonObject node2)
		{
			return new MapNode(context, node2);
		}
		return new ValueNode(context, node);
	}

	public virtual List<T> CreateList<T>(Func<MapNode, OpenApiDocument, T> map, OpenApiDocument hostDocument)
	{
		throw new OpenApiReaderException("Cannot create list from this type of node.", Context);
	}

	public virtual Dictionary<string, T> CreateMap<T>(Func<MapNode, OpenApiDocument, T> map, OpenApiDocument hostDocument)
	{
		throw new OpenApiReaderException("Cannot create map from this type of node.", Context);
	}

	public virtual List<T> CreateSimpleList<T>(Func<ValueNode, OpenApiDocument?, T> map, OpenApiDocument openApiDocument)
	{
		throw new OpenApiReaderException("Cannot create simple list from this type of node.", Context);
	}

	public virtual Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
	{
		throw new OpenApiReaderException("Cannot create simple map from this type of node.", Context);
	}

	public virtual JsonNode CreateAny()
	{
		throw new OpenApiReaderException("Cannot create an Any object this type of node.", Context);
	}

	public virtual string GetRaw()
	{
		throw new OpenApiReaderException("Cannot get raw value from this type of node.", Context);
	}

	public virtual string GetScalarValue()
	{
		throw new OpenApiReaderException("Cannot create a scalar value from this type of node.", Context);
	}

	public virtual List<JsonNode> CreateListOfAny()
	{
		throw new OpenApiReaderException("Cannot create a list from this type of node.", Context);
	}

	public virtual Dictionary<string, HashSet<T>> CreateArrayMap<T>(Func<ValueNode, OpenApiDocument?, T> map, OpenApiDocument? openApiDocument)
	{
		throw new OpenApiReaderException("Cannot create array map from this type of node.", Context);
	}
}
