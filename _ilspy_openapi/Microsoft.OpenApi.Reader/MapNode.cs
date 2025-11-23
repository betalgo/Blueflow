using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Abstraction of a Map to isolate semantic parsing from details of JSON DOM
/// </summary>
internal class MapNode : ParseNode, IEnumerable<PropertyNode>, IEnumerable
{
	private readonly JsonObject _node;

	private readonly List<PropertyNode> _nodes;

	private PropertyNode GetPropertyNodeFromJsonNode(string key, JsonNode? node)
	{
		return new PropertyNode(base.Context, key, node ?? JsonNullSentinel.JsonNull);
	}

	public MapNode(ParsingContext context, JsonNode node)
		: base(context, node)
	{
		if (!(node is JsonObject node2))
		{
			throw new OpenApiReaderException("Expected map.", base.Context);
		}
		_node = node2;
		_nodes = _node.Select<KeyValuePair<string, JsonNode>, PropertyNode>((KeyValuePair<string, JsonNode> p) => GetPropertyNodeFromJsonNode(p.Key, p.Value)).ToList();
	}

	public override Dictionary<string, T> CreateMap<T>(Func<MapNode, OpenApiDocument, T> map, OpenApiDocument hostDocument)
	{
		return Enumerable.Select(_node ?? throw new OpenApiReaderException("Expected map while parsing " + typeof(T).Name, base.Context), delegate(KeyValuePair<string, JsonNode> n)
		{
			string key = n.Key;
			T value;
			try
			{
				base.Context.StartObject(key);
				value = ((n.Value is JsonObject node) ? map(new MapNode(base.Context, node), hostDocument) : default(T));
			}
			finally
			{
				base.Context.EndObject();
			}
			return new { key, value };
		}).ToDictionary(k => k.key, v => v.value);
	}

	public override Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
	{
		return (_node ?? throw new OpenApiReaderException("Expected map while parsing " + typeof(T).Name, base.Context)).Select<KeyValuePair<string, JsonNode>, (string, T)>(delegate(KeyValuePair<string, JsonNode> n)
		{
			string key = n.Key;
			try
			{
				base.Context.StartObject(key);
				if (!(n.Value is JsonValue jsonValue))
				{
					throw new OpenApiReaderException("Expected scalar while parsing " + typeof(T).Name, base.Context);
				}
				JsonValue node = jsonValue;
				return (key: key, value: map(new ValueNode(base.Context, node)));
			}
			finally
			{
				base.Context.EndObject();
			}
		}).ToDictionary<(string, T), string, T>(((string key, T value) k) => k.key, ((string key, T value) v) => v.value);
	}

	public override Dictionary<string, HashSet<T>> CreateArrayMap<T>(Func<ValueNode, OpenApiDocument?, T> map, OpenApiDocument? openApiDocument)
	{
		return (_node ?? throw new OpenApiReaderException("Expected map while parsing " + typeof(T).Name, base.Context)).Select<KeyValuePair<string, JsonNode>, (string, HashSet<T>)>(delegate(KeyValuePair<string, JsonNode> n)
		{
			string key = n.Key;
			try
			{
				base.Context.StartObject(key);
				if (!(n.Value is JsonArray source))
				{
					throw new OpenApiReaderException("Expected array while parsing " + typeof(T).Name, base.Context);
				}
				HashSet<T> item = new HashSet<T>(from node in source.OfType<JsonNode>()
					select map(new ValueNode(base.Context, node), openApiDocument));
				return (key: key, values: item);
			}
			finally
			{
				base.Context.EndObject();
			}
		}).ToDictionary<(string, HashSet<T>), string, HashSet<T>>(((string key, HashSet<T> values) kvp) => kvp.key, ((string key, HashSet<T> values) kvp) => kvp.values);
	}

	public IEnumerator<PropertyNode> GetEnumerator()
	{
		return _nodes.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string GetRaw()
	{
		return JsonSerializer.Serialize(_node, SourceGenerationContext.Default.JsonObject);
	}

	public string? GetReferencePointer()
	{
		if (!_node.TryGetPropertyValue("$ref", out JsonNode jsonNode))
		{
			return null;
		}
		return jsonNode?.GetScalarValue();
	}

	public string? GetJsonSchemaIdentifier()
	{
		if (!_node.TryGetPropertyValue("$id", out JsonNode jsonNode))
		{
			return null;
		}
		return jsonNode?.GetScalarValue();
	}

	public string? GetSummaryValue()
	{
		if (!_node.TryGetPropertyValue("summary", out JsonNode jsonNode))
		{
			return null;
		}
		return jsonNode?.GetScalarValue();
	}

	public string? GetDescriptionValue()
	{
		if (!_node.TryGetPropertyValue("description", out JsonNode jsonNode))
		{
			return null;
		}
		return jsonNode?.GetScalarValue();
	}

	public string? GetScalarValue(ValueNode key)
	{
		string scalarValue = key.GetScalarValue();
		if (scalarValue != null)
		{
			if (!(_node[scalarValue] is JsonValue jsonValue))
			{
				throw new OpenApiReaderException("Expected scalar while parsing " + key.GetScalarValue(), base.Context);
			}
			return Convert.ToString(jsonValue?.GetValue<object>(), CultureInfo.InvariantCulture);
		}
		return null;
	}

	/// <summary>
	/// Create an <see cref="T:Microsoft.OpenApi.JsonNodeExtension" />
	/// </summary>
	/// <returns>The created Json object.</returns>
	public override JsonNode CreateAny()
	{
		return _node;
	}
}
