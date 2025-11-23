using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add deprecation information. x-ms-enum-flags
/// </summary>
public class OpenApiEnumFlagsExtension : IOpenApiExtension
{
	/// <summary>
	/// Name of the extension as used in the description.
	/// </summary>
	public static string Name => "x-ms-enum-flags";

	/// <summary>
	/// Whether the enum is a flagged enum.
	/// </summary>
	public bool IsFlags { get; set; }

	/// <inheritdoc />
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		writer.WriteStartObject();
		writer.WriteProperty<bool>("IsFlags".ToFirstCharacterLowerCase(), IsFlags);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Parse the extension from the raw OpenApiAny object.
	/// </summary>
	/// <param name="source">The source element to parse.</param>
	/// <returns>The <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiEnumFlagsExtension" />.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">When the source element is not an object</exception>
	public static OpenApiEnumFlagsExtension Parse(JsonNode source)
	{
		JsonObject obj = (source as JsonObject) ?? throw new ArgumentOutOfRangeException("source");
		OpenApiEnumFlagsExtension openApiEnumFlagsExtension = new OpenApiEnumFlagsExtension();
		if (obj.TryGetPropertyValue("IsFlags".ToFirstCharacterLowerCase(), out JsonNode jsonNode) && jsonNode is JsonValue jsonValue && jsonValue.TryGetValue<bool>(out var value))
		{
			openApiEnumFlagsExtension.IsFlags = value;
		}
		return openApiEnumFlagsExtension;
	}
}
