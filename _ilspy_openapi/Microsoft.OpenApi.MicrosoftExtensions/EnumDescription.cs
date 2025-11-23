using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Description of an enum symbol
/// </summary>
public class EnumDescription : IOpenApiElement
{
	/// <summary>
	/// The description for the enum symbol
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// The symbol for the enum symbol to use for code-generation
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// The symbol as described in the main enum schema.
	/// </summary>
	public string Value { get; set; } = string.Empty;

	/// <summary>
	/// Default constructor
	/// </summary>
	public EnumDescription()
	{
	}

	/// <summary>
	/// Constructor from a raw OpenApiObject
	/// </summary>
	/// <param name="source">The source object</param>
	public EnumDescription(JsonObject source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (source.TryGetPropertyValue("Value".ToFirstCharacterLowerCase(), out JsonNode jsonNode) && jsonNode is JsonValue jsonValue)
		{
			string value2;
			if (jsonValue.GetValueKind() == JsonValueKind.Number && jsonValue.TryGetValue<decimal>(out var value))
			{
				Value = value.ToString(CultureInfo.InvariantCulture);
			}
			else if (jsonValue.TryGetValue<string>(out value2))
			{
				Value = value2;
			}
		}
		if (source.TryGetPropertyValue("Description".ToFirstCharacterLowerCase(), out JsonNode jsonNode2) && jsonNode2 is JsonValue jsonValue2 && jsonValue2.TryGetValue<string>(out string value3))
		{
			Description = value3;
		}
		if (source.TryGetPropertyValue("Name".ToFirstCharacterLowerCase(), out JsonNode jsonNode3) && jsonNode3 is JsonValue jsonValue3 && jsonValue3.TryGetValue<string>(out string value4))
		{
			Name = value4;
		}
	}
}
