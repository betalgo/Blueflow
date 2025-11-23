using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal abstract class ParseNode
{
	public ParsingContext Context { get; }

	protected ParseNode(ParsingContext parsingContext)
	{
		Context = parsingContext;
	}

	public MapNode CheckMapNode(string nodeName)
	{
		return (this as MapNode) ?? throw new OpenApiReaderException(nodeName + " must be a map/object", Context);
	}

	public static ParseNode Create(ParsingContext context, YamlNode node)
	{
		YamlSequenceNode val = (YamlSequenceNode)(object)((node is YamlSequenceNode) ? node : null);
		if (val != null)
		{
			return new ListNode(context, val);
		}
		YamlMappingNode val2 = (YamlMappingNode)(object)((node is YamlMappingNode) ? node : null);
		if (val2 != null)
		{
			return new MapNode(context, (YamlNode)(object)val2);
		}
		return new ValueNode(context, (node is YamlScalarNode) ? node : null);
	}

	public virtual List<T> CreateList<T>(Func<MapNode, T> map)
	{
		throw new OpenApiReaderException("Cannot create list from this type of node.", Context);
	}

	public virtual Dictionary<string, T> CreateMap<T>(Func<MapNode, T> map)
	{
		throw new OpenApiReaderException("Cannot create map from this type of node.", Context);
	}

	public virtual Dictionary<string, T> CreateMapWithReference<T>(ReferenceType referenceType, Func<MapNode, T> map) where T : class, IOpenApiReferenceable
	{
		throw new OpenApiReaderException("Cannot create map from this reference.", Context);
	}

	public virtual List<T> CreateSimpleList<T>(Func<ValueNode, T> map)
	{
		throw new OpenApiReaderException("Cannot create simple list from this type of node.", Context);
	}

	public virtual Dictionary<string, T> CreateSimpleMap<T>(Func<ValueNode, T> map)
	{
		throw new OpenApiReaderException("Cannot create simple map from this type of node.", Context);
	}

	public virtual IOpenApiAny CreateAny()
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

	public virtual List<IOpenApiAny> CreateListOfAny()
	{
		throw new OpenApiReaderException("Cannot create a list from this type of node.", Context);
	}
}
