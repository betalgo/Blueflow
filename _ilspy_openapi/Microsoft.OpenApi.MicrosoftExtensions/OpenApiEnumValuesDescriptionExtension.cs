using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add enum values descriptions.
/// Based of the AutoRest specification https://github.com/Azure/autorest/blob/main/docs/extensions/readme.md#x-ms-enum
/// </summary>
public class OpenApiEnumValuesDescriptionExtension : IOpenApiExtension
{
	/// <summary>
	/// Name of the extension as used in the description.
	/// </summary>
	public static string Name => "x-ms-enum";

	/// <summary>
	/// The of the enum.
	/// </summary>
	public string EnumName { get; set; } = string.Empty;

	/// <summary>
	/// Descriptions for the enum symbols, where the value MUST match the enum symbols in the main description
	/// </summary>
	public List<EnumDescription> ValuesDescriptions { get; set; } = new List<EnumDescription>();

	/// <inheritdoc />
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		if (!string.IsNullOrEmpty(EnumName) && ValuesDescriptions.Any())
		{
			writer.WriteStartObject();
			writer.WriteProperty("Name".ToFirstCharacterLowerCase(), EnumName);
			writer.WriteProperty<bool>("modelAsString", value: false);
			writer.WriteRequiredCollection("values", ValuesDescriptions, delegate(IOpenApiWriter w, EnumDescription x)
			{
				w.WriteStartObject();
				w.WriteProperty("Value".ToFirstCharacterLowerCase(), x.Value);
				w.WriteProperty("Description".ToFirstCharacterLowerCase(), x.Description);
				w.WriteProperty("Name".ToFirstCharacterLowerCase(), x.Name);
				w.WriteEndObject();
			});
			writer.WriteEndObject();
		}
	}

	/// <summary>
	/// Parse the extension from the raw IOpenApiAny object.
	/// </summary>
	/// <param name="source">The source element to parse.</param>
	/// <returns>The <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiEnumValuesDescriptionExtension" />.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">When the source element is not an object</exception>
	public static OpenApiEnumValuesDescriptionExtension Parse(JsonNode source)
	{
		JsonObject obj = (source as JsonObject) ?? throw new ArgumentOutOfRangeException("source");
		OpenApiEnumValuesDescriptionExtension openApiEnumValuesDescriptionExtension = new OpenApiEnumValuesDescriptionExtension();
		if (obj.TryGetPropertyValue("values", out JsonNode jsonNode) && jsonNode is JsonArray source2)
		{
			openApiEnumValuesDescriptionExtension.ValuesDescriptions.AddRange(from x in source2.OfType<JsonObject>()
				select new EnumDescription(x));
		}
		return openApiEnumValuesDescriptionExtension;
	}
}
