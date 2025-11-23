using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add deprecation information. x-ms-deprecation
/// </summary>
public class OpenApiDeprecationExtension : IOpenApiExtension
{
	private static readonly DateTimeStyles datesStyle = DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind;

	/// <summary>
	/// Name of the extension as used in the description.
	/// </summary>
	public static string Name => "x-ms-deprecation";

	/// <summary>
	/// The date at which the element has been/will be removed entirely from the service.
	/// </summary>
	public DateTimeOffset? RemovalDate { get; set; }

	/// <summary>
	/// The date at which the element has been/will be deprecated.
	/// </summary>
	public DateTimeOffset? Date { get; set; }

	/// <summary>
	/// The version this revision was introduced.
	/// </summary>
	public string Version { get; set; } = string.Empty;

	/// <summary>
	/// The description of the revision.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <inheritdoc />
	public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		if (RemovalDate.HasValue || Date.HasValue || !string.IsNullOrEmpty(Version) || !string.IsNullOrEmpty(Description))
		{
			writer.WriteStartObject();
			if (RemovalDate.HasValue)
			{
				writer.WriteProperty("RemovalDate".ToFirstCharacterLowerCase(), RemovalDate.Value);
			}
			if (Date.HasValue)
			{
				writer.WriteProperty("Date".ToFirstCharacterLowerCase(), Date.Value);
			}
			if (!string.IsNullOrEmpty(Version))
			{
				writer.WriteProperty("Version".ToFirstCharacterLowerCase(), Version);
			}
			if (!string.IsNullOrEmpty(Description))
			{
				writer.WriteProperty("Description".ToFirstCharacterLowerCase(), Description);
			}
			writer.WriteEndObject();
		}
	}

	private static DateTimeOffset? GetDateTimeOffsetValue(string propertyName, JsonObject rawObject)
	{
		if (!rawObject.TryGetPropertyValue(propertyName.ToFirstCharacterLowerCase(), out JsonNode jsonNode) || !(jsonNode is JsonValue jsonValue) || jsonNode.GetValueKind() != JsonValueKind.String)
		{
			return null;
		}
		if (jsonValue.TryGetValue<string>(out string value) && DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, datesStyle, out var result))
		{
			return result;
		}
		if (jsonValue.TryGetValue<DateTimeOffset>(out var value2))
		{
			return value2;
		}
		if (jsonValue.TryGetValue<DateTime>(out var value3))
		{
			return new DateTimeOffset(value3, TimeSpan.FromHours(0.0));
		}
		if (jsonValue.TryGetValue<DateOnly>(out var value4))
		{
			return new DateTimeOffset(value4.Year, value4.Month, value4.Day, 0, 0, 0, TimeSpan.FromHours(0.0));
		}
		return null;
	}

	/// <summary>
	/// Parses the <see cref="T:Microsoft.OpenApi.JsonNodeExtension" /> to <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiDeprecationExtension" />.
	/// </summary>
	/// <param name="source">The source object.</param>
	/// <returns>The <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiDeprecationExtension" />.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">When the source element is not an object</exception>
	public static OpenApiDeprecationExtension Parse(JsonNode source)
	{
		if (!(source is JsonObject jsonObject))
		{
			throw new ArgumentOutOfRangeException("source");
		}
		OpenApiDeprecationExtension openApiDeprecationExtension = new OpenApiDeprecationExtension
		{
			RemovalDate = GetDateTimeOffsetValue("RemovalDate", jsonObject),
			Date = GetDateTimeOffsetValue("Date", jsonObject)
		};
		if (jsonObject.TryGetPropertyValue("Version".ToFirstCharacterLowerCase(), out JsonNode jsonNode) && jsonNode is JsonValue jsonValue && jsonValue.TryGetValue<string>(out string value))
		{
			openApiDeprecationExtension.Version = value;
		}
		if (jsonObject.TryGetPropertyValue("Description".ToFirstCharacterLowerCase(), out JsonNode jsonNode2) && jsonNode2 is JsonValue jsonValue2 && jsonValue2.TryGetValue<string>(out string value2))
		{
			openApiDeprecationExtension.Description = value2;
		}
		return openApiDeprecationExtension;
	}
}
