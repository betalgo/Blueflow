using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class ListNode : ParseNode, IEnumerable<ParseNode>, IEnumerable
{
	private readonly YamlSequenceNode _nodeList;

	public ListNode(ParsingContext context, YamlSequenceNode sequenceNode)
		: base(context)
	{
		_nodeList = sequenceNode;
	}

	public override List<T> CreateList<T>(Func<MapNode, T> map)
	{
		if (_nodeList == null)
		{
			throw new OpenApiReaderException("Expected list while parsing " + typeof(T).Name);
		}
		return (from n in (IEnumerable<YamlNode>)_nodeList
			select map(new MapNode(base.Context, (n is YamlMappingNode) ? n : null)) into i
			where i != null
			select i).ToList();
	}

	public override List<IOpenApiAny> CreateListOfAny()
	{
		return (from n in (IEnumerable<YamlNode>)_nodeList
			select ParseNode.Create(base.Context, n).CreateAny() into i
			where i != null
			select i).ToList();
	}

	public override List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
	{
		if (_nodeList == null)
		{
			throw new OpenApiReaderException("Expected list while parsing " + typeof(T).Name);
		}
		return ((IEnumerable<YamlNode>)_nodeList).Select((YamlNode n) => map(new ValueNode(base.Context, n))).ToList();
	}

	public IEnumerator<ParseNode> GetEnumerator()
	{
		return ((IEnumerable<YamlNode>)_nodeList).Select((YamlNode n) => ParseNode.Create(base.Context, n)).ToList().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override IOpenApiAny CreateAny()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		OpenApiArray val = new OpenApiArray();
		using IEnumerator<ParseNode> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ParseNode current = enumerator.Current;
			((List<IOpenApiAny>)(object)val).Add(current.CreateAny());
		}
		return (IOpenApiAny)(object)val;
	}
}
