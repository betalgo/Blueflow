using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi;

/// <summary>
/// Extension methods for <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> serialization.
/// </summary>
public static class OpenApiSerializableExtensions
{
	/// <summary>
	/// Serialize the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to the Open API document (JSON) using the given stream and specification version.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="stream">The output stream.</param>
	/// <param name="specVersion">The Open API specification version.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static Task SerializeAsJsonAsync<T>(this T element, Stream stream, OpenApiSpecVersion specVersion, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		return element.SerializeAsync(stream, specVersion, "json", cancellationToken);
	}

	/// <summary>
	/// Serializes the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to the Open API document (YAML) using the given stream and specification version.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="stream">The output stream.</param>
	/// <param name="specVersion">The Open API specification version.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static Task SerializeAsYamlAsync<T>(this T element, Stream stream, OpenApiSpecVersion specVersion, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		return element.SerializeAsync(stream, specVersion, "yaml", cancellationToken);
	}

	/// <summary>
	/// Serializes the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to the Open API document using
	/// the given stream, specification version and the format.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="stream">The given stream.</param>
	/// <param name="specVersion">The Open API specification version.</param>
	/// <param name="format">The output format (JSON or YAML).</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static Task SerializeAsync<T>(this T element, Stream stream, OpenApiSpecVersion specVersion, string format, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		return element.SerializeAsync(stream, specVersion, format, null, cancellationToken);
	}

	/// <summary>
	/// Serializes the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to the Open API document using
	/// the given stream, specification version and the format.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="stream">The given stream.</param>
	/// <param name="specVersion">The Open API specification version.</param>
	/// <param name="format">The output format (JSON or YAML).</param>
	/// <param name="settings">Provide configuration settings for controlling writing output</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static Task SerializeAsync<T>(this T element, Stream stream, OpenApiSpecVersion specVersion, string format, OpenApiWriterSettings? settings = null, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		Utils.CheckArgumentNull(stream, "stream");
		FormattingStreamWriter textWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
		string text = format.ToLowerInvariant();
		IOpenApiWriter openApiWriter;
		if (text == "json")
		{
			openApiWriter = ((!(settings is OpenApiJsonWriterSettings settings2)) ? new OpenApiJsonWriter(textWriter, settings) : new OpenApiJsonWriter(textWriter, settings2));
		}
		else
		{
			if (!(text == "yaml"))
			{
				throw new OpenApiException(string.Format(SRResource.OpenApiFormatNotSupported, format));
			}
			openApiWriter = new OpenApiYamlWriter(textWriter, settings);
		}
		IOpenApiWriter writer = openApiWriter;
		return element.SerializeAsync(writer, specVersion, cancellationToken);
	}

	/// <summary>
	/// Serializes the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to Open API document using the given specification version and writer.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="writer">The output writer.</param>
	/// <param name="specVersion">Version of the specification the output should conform to</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static Task SerializeAsync<T>(this T element, IOpenApiWriter writer, OpenApiSpecVersion specVersion, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		Utils.CheckArgumentNull(element, "element");
		Utils.CheckArgumentNull(writer, "writer");
		switch (specVersion)
		{
		case OpenApiSpecVersion.OpenApi3_2:
			element.SerializeAsV32(writer);
			break;
		case OpenApiSpecVersion.OpenApi3_1:
			element.SerializeAsV31(writer);
			break;
		case OpenApiSpecVersion.OpenApi3_0:
			element.SerializeAsV3(writer);
			break;
		case OpenApiSpecVersion.OpenApi2_0:
			element.SerializeAsV2(writer);
			break;
		default:
			throw new OpenApiException(string.Format(SRResource.OpenApiSpecVersionNotSupported, specVersion));
		}
		return writer.FlushAsync(cancellationToken);
	}

	/// <summary>
	/// Serializes the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to the Open API document as a string in JSON format.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="specVersion">The Open API specification version.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static Task<string> SerializeAsJsonAsync<T>(this T element, OpenApiSpecVersion specVersion, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		return element.SerializeAsync(specVersion, "json", cancellationToken);
	}

	/// <summary>
	/// Serializes the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to the Open API document as a string in YAML format.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="specVersion">The Open API specification version.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static Task<string> SerializeAsYamlAsync<T>(this T element, OpenApiSpecVersion specVersion, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		return element.SerializeAsync(specVersion, "yaml", cancellationToken);
	}

	/// <summary>
	/// Serializes the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /> to the Open API document as a string in the given format.
	/// </summary>
	/// <typeparam name="T">the <see cref="T:Microsoft.OpenApi.IOpenApiSerializable" /></typeparam>
	/// <param name="element">The Open API element.</param>
	/// <param name="specVersion">The Open API specification version.</param>
	/// <param name="format">Open API document format.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	public static async Task<string> SerializeAsync<T>(this T element, OpenApiSpecVersion specVersion, string format, CancellationToken cancellationToken = default(CancellationToken)) where T : IOpenApiSerializable
	{
		Utils.CheckArgumentNull(element, "element");
		using MemoryStream stream = new MemoryStream();
		await element.SerializeAsync(stream, specVersion, format, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		stream.Position = 0L;
		using StreamReader streamReader = new StreamReader(stream);
		return await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}
}
