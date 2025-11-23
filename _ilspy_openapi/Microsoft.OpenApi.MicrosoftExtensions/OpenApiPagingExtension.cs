using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add pageable information.
/// Based of the AutoRest specification https://github.com/Azure/autorest/blob/main/docs/extensions/readme.md#x-ms-pageable
/// </summary>
public class OpenApiPagingExtension : IOpenApiExtension
{
	/// <summary>
	/// Name of the extension as used in the description.
	/// </summary>
	public static string Name => "x-ms-pageable";

	/// <summary>
	/// The name of the property that provides the collection of pageable items.
	/// </summary>
	public string ItemName { get; set; } = "value";

	/// <summary>
	/// The name of the property that provides the next link (common: nextLink)
	/// </summary>
	public string NextLinkName { get; set; } = "nextLink";

	/// <summary>
	/// The name (operationId) of the operation for retrieving the next page.
	/// </summary>
	public string OperationName { get; set; } = string.Empty;

	/// <inheritdoc />
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		writer.WriteStartObject();
		if (!string.IsNullOrEmpty(NextLinkName))
		{
			writer.WriteProperty("NextLinkName".ToFirstCharacterLowerCase(), NextLinkName);
		}
		if (!string.IsNullOrEmpty(OperationName))
		{
			writer.WriteProperty("OperationName".ToFirstCharacterLowerCase(), OperationName);
		}
		writer.WriteProperty("ItemName".ToFirstCharacterLowerCase(), ItemName);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Parse the extension from the raw IOpenApiAny object.
	/// </summary>
	/// <param name="source">The source element to parse.</param>
	/// <returns>The <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiPagingExtension" />.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">When the source element is not an object</exception>
	public static OpenApiPagingExtension Parse(JsonNode source)
	{
		JsonObject obj = (source as JsonObject) ?? throw new ArgumentOutOfRangeException("source");
		OpenApiPagingExtension openApiPagingExtension = new OpenApiPagingExtension();
		if (obj.TryGetPropertyValue("NextLinkName".ToFirstCharacterLowerCase(), out JsonNode jsonNode) && jsonNode is JsonValue jsonValue && jsonValue.TryGetValue<string>(out string value))
		{
			openApiPagingExtension.NextLinkName = value;
		}
		if (obj.TryGetPropertyValue("OperationName".ToFirstCharacterLowerCase(), out JsonNode jsonNode2) && jsonNode2 is JsonValue jsonValue2 && jsonValue2.TryGetValue<string>(out string value2))
		{
			openApiPagingExtension.OperationName = value2;
		}
		if (obj.TryGetPropertyValue("ItemName".ToFirstCharacterLowerCase(), out JsonNode jsonNode3) && jsonNode3 is JsonValue jsonValue3 && jsonValue3.TryGetValue<string>(out string value3))
		{
			openApiPagingExtension.ItemName = value3;
		}
		return openApiPagingExtension;
	}
}
