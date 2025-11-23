using System;
using System.Collections.Generic;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

public static class JsonPointerExtensions
{
	public static YamlNode Find(this JsonPointer currentPointer, YamlNode baseYamlNode)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		if (currentPointer.Tokens.Length == 0)
		{
			return baseYamlNode;
		}
		try
		{
			YamlNode value = baseYamlNode;
			string[] tokens = currentPointer.Tokens;
			foreach (string text in tokens)
			{
				YamlSequenceNode val = (YamlSequenceNode)(object)((value is YamlSequenceNode) ? value : null);
				if (val != null)
				{
					value = val.Children[Convert.ToInt32(text)];
					continue;
				}
				YamlMappingNode val2 = (YamlMappingNode)(object)((value is YamlMappingNode) ? value : null);
				if (val2 != null && !((IDictionary<YamlNode, YamlNode>)val2.Children).TryGetValue((YamlNode)new YamlScalarNode(text), out value))
				{
					return null;
				}
			}
			return value;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
