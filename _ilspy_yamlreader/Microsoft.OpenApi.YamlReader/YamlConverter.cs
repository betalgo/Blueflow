using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.YamlReader;

public static class YamlConverter
{
	private static readonly HashSet<string> YamlNullRepresentations = new HashSet<string>(StringComparer.Ordinal) { "~", "null", "Null", "NULL" };

	public static IEnumerable<JsonNode> ToJsonNode(this YamlStream yaml)
	{
		return yaml.Documents.Select((YamlDocument x) => x.ToJsonNode());
	}

	public static JsonNode ToJsonNode(this YamlDocument yaml)
	{
		return yaml.RootNode.ToJsonNode();
	}

	public static JsonNode ToJsonNode(this YamlNode yaml)
	{
		YamlMappingNode val = (YamlMappingNode)(object)((yaml is YamlMappingNode) ? yaml : null);
		if (val == null)
		{
			YamlSequenceNode val2 = (YamlSequenceNode)(object)((yaml is YamlSequenceNode) ? yaml : null);
			if (val2 == null)
			{
				YamlScalarNode val3 = (YamlScalarNode)(object)((yaml is YamlScalarNode) ? yaml : null);
				if (val3 != null)
				{
					return (JsonNode)(object)val3.ToJsonValue();
				}
				throw new NotSupportedException("This yaml isn't convertible to JSON");
			}
			return (JsonNode)(object)val2.ToJsonArray();
		}
		return (JsonNode)(object)val.ToJsonObject();
	}

	public static YamlNode ToYamlNode(this JsonNode json)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		JsonObject val = (JsonObject)(object)((json is JsonObject) ? json : null);
		if (val == null)
		{
			JsonArray val2 = (JsonArray)(object)((json is JsonArray) ? json : null);
			if (val2 == null)
			{
				JsonValue val3 = (JsonValue)(object)((json is JsonValue) ? json : null);
				if (val3 != null)
				{
					if (JsonNullSentinel.IsJsonNullSentinel((JsonNode)(object)val3))
					{
						return (YamlNode)new YamlScalarNode("null")
						{
							Style = (ScalarStyle)1
						};
					}
					return (YamlNode)(object)val3.ToYamlScalar();
				}
				throw new NotSupportedException("This isn't a supported JsonNode");
			}
			return (YamlNode)(object)val2.ToYamlSequence();
		}
		return (YamlNode)(object)val.ToYamlMapping();
	}

	public static JsonObject ToJsonObject(this YamlMappingNode yaml)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		JsonObject val = new JsonObject((JsonNodeOptions?)null);
		foreach (KeyValuePair<YamlNode, YamlNode> item in yaml)
		{
			string value = ((YamlScalarNode)item.Key).Value;
			((JsonNode)val)[value] = item.Value.ToJsonNode();
		}
		return val;
	}

	private static YamlMappingNode ToYamlMapping(this JsonObject obj)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		return new YamlMappingNode((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)((IEnumerable<KeyValuePair<string, JsonNode>>)obj).ToDictionary((KeyValuePair<string, JsonNode> x) => (YamlNode)new YamlScalarNode(x.Key)
		{
			Style = (ScalarStyle)((!NeedsQuoting(x.Key)) ? 1 : 3)
		}, (KeyValuePair<string, JsonNode> x) => x.Value.ToYamlNode()));
	}

	public static JsonArray ToJsonArray(this YamlSequenceNode yaml)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		JsonArray val = new JsonArray((JsonNodeOptions?)null);
		foreach (YamlNode item in yaml)
		{
			val.Add(item.ToJsonNode());
		}
		return val;
	}

	private static YamlSequenceNode ToYamlSequence(this JsonArray arr)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		return new YamlSequenceNode(((IEnumerable<JsonNode>)arr).Select((JsonNode x) => x.ToYamlNode()));
	}

	private static JsonValue ToJsonValue(this YamlScalarNode yaml)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected I4, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		ScalarStyle style = yaml.Style;
		switch ((int)style)
		{
		case 1:
		{
			if (decimal.TryParse(yaml.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
			{
				return JsonValue.Create(result, (JsonNodeOptions?)null);
			}
			if (bool.TryParse(yaml.Value, out var result2))
			{
				return JsonValue.Create(result2, (JsonNodeOptions?)null);
			}
			if (YamlNullRepresentations.Contains(yaml.Value))
			{
				return (JsonValue)((JsonNode)JsonNullSentinel.JsonNull).DeepClone();
			}
			return JsonValue.Create(yaml.Value, (JsonNodeOptions?)null);
		}
		case 0:
		case 2:
		case 3:
		case 4:
		case 5:
			return JsonValue.Create(yaml.Value, (JsonNodeOptions?)null);
		default:
			throw new ArgumentOutOfRangeException("yaml");
		}
	}

	private static bool NeedsQuoting(string value)
	{
		if (!string.IsNullOrEmpty(value) && !decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var _) && !bool.TryParse(value, out var _))
		{
			return YamlNullRepresentations.Contains(value);
		}
		return true;
	}

	private static YamlScalarNode ToYamlScalar(this JsonValue val)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		if ((int)((JsonNode)val).GetValueKind() == 3 && val.TryGetValue<string>(ref text))
		{
			bool num = NeedsQuoting(text);
			bool flag = Enumerable.Contains(text, '\n');
			ScalarStyle val2 = (num ? ((ScalarStyle)3) : ((!flag) ? ((ScalarStyle)1) : ((ScalarStyle)4)));
			ScalarStyle style = val2;
			return new YamlScalarNode(text)
			{
				Style = style
			};
		}
		return new YamlScalarNode(((object)val).ToString())
		{
			Style = (ScalarStyle)1
		};
	}
}
