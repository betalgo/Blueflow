using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class ValueNode : ParseNode
{
	private readonly YamlScalarNode _node;

	public ValueNode(ParsingContext context, YamlNode node)
		: base(context)
	{
		YamlScalarNode val = (YamlScalarNode)(object)((node is YamlScalarNode) ? node : null);
		if (val == null)
		{
			throw new OpenApiReaderException("Expected a value.", node);
		}
		_node = val;
	}

	public override string GetScalarValue()
	{
		return _node.Value;
	}

	public override IOpenApiAny CreateAny()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		string scalarValue = GetScalarValue();
		ScalarStyle style = _node.Style;
		bool flag = style - 2 <= 3;
		return (IOpenApiAny)new OpenApiString(scalarValue, flag);
	}
}
