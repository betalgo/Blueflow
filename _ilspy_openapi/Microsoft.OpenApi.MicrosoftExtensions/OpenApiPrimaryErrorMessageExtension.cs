using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add tag the primary error message to use on error types. x-ms-primary-error-message
/// </summary>
public class OpenApiPrimaryErrorMessageExtension : IOpenApiExtension
{
	/// <summary>
	/// Name of the extension as used in the description.
	/// </summary>
	public static string Name => "x-ms-primary-error-message";

	/// <summary>
	/// Whether this property is the primary error message to use on error types.
	/// </summary>
	public bool IsPrimaryErrorMessage { get; set; }

	/// <inheritdoc />
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		writer.WriteValue(IsPrimaryErrorMessage);
	}

	/// <summary>
	/// Parses the <see cref="T:Microsoft.OpenApi.JsonNodeExtension" /> to <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiPrimaryErrorMessageExtension" />.
	/// </summary>
	/// <param name="source">The source object.</param>
	/// <returns>The <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiPrimaryErrorMessageExtension" />.</returns>
	public static OpenApiPrimaryErrorMessageExtension Parse(JsonNode source)
	{
		if (!(source is JsonValue jsonValue))
		{
			throw new ArgumentOutOfRangeException("source");
		}
		bool value;
		return new OpenApiPrimaryErrorMessageExtension
		{
			IsPrimaryErrorMessage = (jsonValue.TryGetValue<bool>(out value) && value)
		};
	}
}
