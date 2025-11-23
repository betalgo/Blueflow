using System;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal class ValueNode : ParseNode
{
	private readonly JsonValue _node;

	public ValueNode(ParsingContext context, JsonNode node)
		: base(context, node)
	{
		if (!(node is JsonValue node2))
		{
			throw new OpenApiReaderException("Expected a value while parsing at " + base.Context.GetLocation() + ".");
		}
		_node = node2;
	}

	public override string GetScalarValue()
	{
		return Convert.ToString(_node.GetValue<object>(), CultureInfo.InvariantCulture) ?? throw new OpenApiReaderException("Expected a value at " + base.Context.GetLocation() + ".");
	}

	/// <summary>
	/// Create a <see cref="T:System.Text.Json.Nodes.JsonNode" />
	/// </summary>
	/// <returns>The created Any object.</returns>
	public override JsonNode CreateAny()
	{
		return _node;
	}
}
