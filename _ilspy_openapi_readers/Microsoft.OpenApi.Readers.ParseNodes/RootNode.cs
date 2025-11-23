using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class RootNode : ParseNode
{
	private readonly YamlDocument _yamlDocument;

	public RootNode(ParsingContext context, YamlDocument yamlDocument)
		: base(context)
	{
		_yamlDocument = yamlDocument;
	}

	public ParseNode Find(JsonPointer referencePointer)
	{
		YamlNode val = referencePointer.Find(_yamlDocument.RootNode);
		if (val == null)
		{
			return null;
		}
		return ParseNode.Create(base.Context, val);
	}

	public MapNode GetMap()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		return new MapNode(base.Context, (YamlNode)(YamlMappingNode)_yamlDocument.RootNode);
	}
}
