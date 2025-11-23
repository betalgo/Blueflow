using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml;
using SharpYaml.Schemas;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class MapNode : ParseNode, IEnumerable<PropertyNode>, IEnumerable
{
	private readonly YamlMappingNode _node;

	private readonly List<PropertyNode> _nodes;

	public PropertyNode this[string key]
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			if (((IDictionary<YamlNode, YamlNode>)_node.Children).TryGetValue((YamlNode)new YamlScalarNode(key), out YamlNode value))
			{
				return new PropertyNode(base.Context, key, value);
			}
			return null;
		}
	}

	public MapNode(ParsingContext context, string yamlString)
		: this(context, (YamlNode)(YamlMappingNode)YamlHelper.ParseYamlString(yamlString))
	{
	}//IL_0008: Unknown result type (might be due to invalid IL or missing references)
	//IL_0012: Expected O, but got Unknown


	public MapNode(ParsingContext context, YamlNode node)
		: base(context)
	{
		YamlMappingNode val = (YamlMappingNode)(object)((node is YamlMappingNode) ? node : null);
		if (val == null)
		{
			throw new OpenApiReaderException("Expected map.", base.Context);
		}
		_node = val;
		_nodes = ((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)_node.Children).Select((KeyValuePair<YamlNode, YamlNode> kvp) => new PropertyNode(base.Context, kvp.Key.GetScalarValue(), kvp.Value)).Cast<PropertyNode>().ToList();
	}

	public override Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
	{
		return ((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)(_node ?? throw new OpenApiReaderException("Expected map while parsing " + typeof(T).Name, base.Context))).Select(delegate(KeyValuePair<YamlNode, YamlNode> n)
		{
			string scalarValue = n.Key.GetScalarValue();
			T value;
			try
			{
				base.Context.StartObject(scalarValue);
				value = ((!(n.Value is YamlMappingNode)) ? default(T) : map(new MapNode(base.Context, (YamlNode)/*isinst with value type is only supported in some contexts*/)));
			}
			finally
			{
				base.Context.EndObject();
			}
			return new
			{
				key = scalarValue,
				value = value
			};
		}).ToDictionary(k => k.key, v => v.value);
	}

	public override Dictionary<string, T> CreateMapWithReference<T>(ReferenceType referenceType, Func<MapNode, T> map)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)(_node ?? throw new OpenApiReaderException("Expected map while parsing " + typeof(T).Name, base.Context))).Select(delegate(KeyValuePair<YamlNode, YamlNode> n)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			string scalarValue = n.Key.GetScalarValue();
			(string, T) tuple;
			try
			{
				base.Context.StartObject(scalarValue);
				tuple = (scalarValue, map(new MapNode(base.Context, (YamlNode)(YamlMappingNode)n.Value)));
				if (tuple.Item2 == null)
				{
					return ((string key, T value))default((string, T));
				}
				if (((IOpenApiReferenceable)tuple.Item2).Reference == null)
				{
					T item = tuple.Item2;
					OpenApiReference val = new OpenApiReference
					{
						Type = referenceType
					};
					(val.Id, _) = tuple;
					((IOpenApiReferenceable)item).Reference = val;
				}
			}
			finally
			{
				base.Context.EndObject();
			}
			return ((string key, T value))tuple;
		}).Where(delegate((string key, T value) n)
		{
			var (text, val) = n;
			return text != null || val != null;
		}).ToDictionary(((string key, T value) k) => k.key, ((string key, T value) v) => v.value);
	}

	public override Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
	{
		return ((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)(_node ?? throw new OpenApiReaderException("Expected map while parsing " + typeof(T).Name, base.Context))).Select(delegate(KeyValuePair<YamlNode, YamlNode> n)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			string scalarValue = n.Key.GetScalarValue();
			try
			{
				base.Context.StartObject(scalarValue);
				if (!(n.Value is YamlScalarNode))
				{
					throw new OpenApiReaderException("Expected scalar while parsing " + typeof(T).Name, base.Context);
				}
				return (key: scalarValue, value: map(new ValueNode(base.Context, (YamlNode)(YamlScalarNode)n.Value)));
			}
			finally
			{
				base.Context.EndObject();
			}
		}).ToDictionary(((string key, T value) k) => k.key, ((string key, T value) v) => v.value);
	}

	public IEnumerator<PropertyNode> GetEnumerator()
	{
		return _nodes.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _nodes.GetEnumerator();
	}

	public override string GetRaw()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return new Serializer(new SerializerSettings((IYamlSchema)new JsonSchema())
		{
			EmitJsonComptible = true
		}).Serialize((object)_node);
	}

	public T GetReferencedObject<T>(ReferenceType referenceType, string referenceId) where T : IOpenApiReferenceable, new()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		T result = new T
		{
			UnresolvedReference = true
		};
		OpenApiReference reference = base.Context.VersionService.ConvertToOpenApiReference(referenceId, referenceType);
		((IOpenApiReferenceable)result).Reference = reference;
		return result;
	}

	public string GetReferencePointer()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		if (!((IDictionary<YamlNode, YamlNode>)_node.Children).TryGetValue((YamlNode)new YamlScalarNode("$ref"), out YamlNode value))
		{
			return null;
		}
		return value.GetScalarValue();
	}

	public string GetScalarValue(ValueNode key)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		YamlNode obj = ((IDictionary<YamlNode, YamlNode>)_node.Children)[(YamlNode)new YamlScalarNode(key.GetScalarValue())];
		YamlNode obj2 = ((obj is YamlScalarNode) ? obj : null);
		if (obj2 == null)
		{
			Mark start = ((YamlNode)_node).Start;
			throw new OpenApiReaderException($"Expected scalar at line {((Mark)(ref start)).Line} for key {key.GetScalarValue()}", base.Context);
		}
		return ((YamlScalarNode)obj2).Value;
	}

	public override IOpenApiAny CreateAny()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		OpenApiObject val = new OpenApiObject();
		using IEnumerator<PropertyNode> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			PropertyNode current = enumerator.Current;
			((Dictionary<string, IOpenApiAny>)(object)val).Add(current.Name, current.Value.CreateAny());
		}
		return (IOpenApiAny)(object)val;
	}
}
