using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add reserved parameters. x-ms-reserved-parameters
/// </summary>
public class OpenApiReservedParameterExtension : IOpenApiExtension
{
	/// <summary>
	/// Name of the extension as used in the description.
	/// </summary>
	public static string Name => "x-ms-reserved-parameter";

	/// <summary>
	/// Whether the associated parameter is reserved or not.
	/// </summary>
	public bool? IsReserved { get; set; }

	/// <inheritdoc />
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		if (IsReserved.HasValue)
		{
			writer.WriteValue(IsReserved.Value);
		}
	}

	/// <summary>
	/// Parses the <see cref="T:Microsoft.OpenApi.JsonNodeExtension" /> to <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiReservedParameterExtension" />.
	/// </summary>
	/// <param name="source">The source object.</param>
	/// <returns>The <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiReservedParameterExtension" />.</returns>
	/// <returns></returns>
	public static OpenApiReservedParameterExtension Parse(JsonNode source)
	{
		if (!(source is JsonValue jsonValue))
		{
			throw new ArgumentOutOfRangeException("source");
		}
		bool value;
		return new OpenApiReservedParameterExtension
		{
			IsReserved = (jsonValue.TryGetValue<bool>(out value) && value)
		};
	}
}
