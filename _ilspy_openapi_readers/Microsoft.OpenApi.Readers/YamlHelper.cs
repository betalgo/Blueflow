using System.IO;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers;

internal static class YamlHelper
{
	public static string GetScalarValue(this YamlNode node)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		YamlNode obj = ((node is YamlScalarNode) ? node : null);
		if (obj == null)
		{
			Mark start = node.Start;
			throw new OpenApiException($"Expected scalar at line {((Mark)(ref start)).Line}");
		}
		return ((YamlScalarNode)obj).Value;
	}

	public static YamlNode ParseYamlString(string yamlString)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		StringReader stringReader = new StringReader(yamlString);
		YamlStream val = new YamlStream();
		val.Load((TextReader)stringReader);
		return val.Documents.First().RootNode;
	}
}
