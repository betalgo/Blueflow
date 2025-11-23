using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// A factory class for loading OpenAPI models from various sources.
/// </summary>
public static class OpenApiModelFactory
{
	private static readonly Lazy<OpenApiReaderSettings> DefaultReaderSettings = new Lazy<OpenApiReaderSettings>(() => new OpenApiReaderSettings());

	/// <summary>
	/// Loads the input stream and parses it into an Open API document.
	/// </summary>
	/// <param name="stream"> The input stream.</param>
	/// <param name="settings"> The OpenApi reader settings.</param>
	/// <param name="format">The OpenAPI format.</param>
	/// <returns>An OpenAPI document instance.</returns>
	public static ReadResult Load(MemoryStream stream, string? format = null, OpenApiReaderSettings? settings = null)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		if (settings == null)
		{
			settings = new OpenApiReaderSettings();
		}
		if (format == null)
		{
			format = InspectStreamFormat(stream);
		}
		ReadResult result = InternalLoad(stream, format, settings);
		if (!settings.LeaveStreamOpen)
		{
			stream.Dispose();
		}
		return result;
	}

	/// <summary>
	/// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="input">Stream containing OpenAPI description to parse.</param>
	/// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
	/// <param name="format"></param>
	/// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
	/// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
	/// <param name="settings">The OpenApiReader settings.</param>
	/// <returns>Instance of newly created IOpenApiElement.</returns>
	/// <returns>The OpenAPI element.</returns>
	public static T? Load<T>(MemoryStream input, OpenApiSpecVersion version, string? format, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
	{
		if (format == null)
		{
			format = InspectStreamFormat(input);
		}
		if (settings == null)
		{
			settings = DefaultReaderSettings.Value;
		}
		return settings.GetReader(format).ReadFragment<T>(input, version, openApiDocument, out diagnostic, settings);
	}

	/// <summary>
	/// Loads the input URL and parses it into an Open API document.
	/// </summary>
	/// <param name="url">The path to the OpenAPI file</param>
	/// <param name="settings"> The OpenApi reader settings.</param>
	/// <param name="token">The cancellation token</param>
	/// <returns></returns>
	public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings? settings = null, CancellationToken token = default(CancellationToken))
	{
		if (settings == null)
		{
			settings = DefaultReaderSettings.Value;
		}
		var (stream, format) = await RetrieveStreamAndFormatAsync(url, settings, token).ConfigureAwait(continueOnCapturedContext: false);
		using (stream)
		{
			return await LoadAsync(stream, format, settings, token).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	/// <summary>
	/// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="url">The path to the OpenAPI file</param>
	/// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
	/// <param name="settings">The OpenApiReader settings.</param>
	/// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
	/// <param name="token"></param>
	/// <returns>Instance of newly created IOpenApiElement.</returns>
	/// <returns>The OpenAPI element.</returns>
	public static async Task<T?> LoadAsync<T>(string url, OpenApiSpecVersion version, OpenApiDocument openApiDocument, OpenApiReaderSettings? settings = null, CancellationToken token = default(CancellationToken)) where T : IOpenApiElement
	{
		if (settings == null)
		{
			settings = DefaultReaderSettings.Value;
		}
		var (stream, format) = await RetrieveStreamAndFormatAsync(url, settings, token).ConfigureAwait(continueOnCapturedContext: false);
		using (stream)
		{
			return await LoadAsync<T>(stream, version, openApiDocument, format, settings, token);
		}
	}

	/// <summary>
	/// Loads the input stream and parses it into an Open API document.  If the stream is not buffered and it contains yaml, it will be buffered before parsing.
	/// </summary>
	/// <param name="input">The input stream.</param>
	/// <param name="settings"> The OpenApi reader settings.</param>
	/// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
	/// <param name="format">The Open API format</param>
	/// <returns></returns>
	public static async Task<ReadResult> LoadAsync(Stream input, string? format = null, OpenApiReaderSettings? settings = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		ArgumentNullException.ThrowIfNull(input, "input");
		if (settings == null)
		{
			settings = new OpenApiReaderSettings();
		}
		Stream preparedStream = null;
		if (format == null)
		{
			(preparedStream, format) = await PrepareStreamForReadingAsync(input, format, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		ReadResult result = await InternalLoadAsync(preparedStream ?? input, format, settings, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (!settings.LeaveStreamOpen)
		{
			await input.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		if (preparedStream != null && preparedStream != input)
		{
			await preparedStream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		return result;
	}

	/// <summary>
	/// Reads the stream input and ensures it is buffered before passing it to the Load method.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="input"></param>
	/// <param name="version"></param>
	/// <param name="openApiDocument">The document used to lookup tag or schema references.</param>
	/// <param name="format"></param>
	/// <param name="settings"></param>
	/// <param name="token"></param>
	/// <returns></returns>
	public static async Task<T?> LoadAsync<T>(Stream input, OpenApiSpecVersion version, OpenApiDocument openApiDocument, string? format = null, OpenApiReaderSettings? settings = null, CancellationToken token = default(CancellationToken)) where T : IOpenApiElement
	{
		Utils.CheckArgumentNull(openApiDocument, "openApiDocument");
		ArgumentNullException.ThrowIfNull(input, "input");
		OpenApiDiagnostic diagnostic;
		if (input is MemoryStream memoryStream)
		{
			return Load<T>(memoryStream, version, format, openApiDocument, out diagnostic, settings);
		}
		MemoryStream memoryStream2 = new MemoryStream();
		await input.CopyToAsync(memoryStream2, 81920, token).ConfigureAwait(continueOnCapturedContext: false);
		memoryStream2.Position = 0L;
		return Load<T>(memoryStream2, version, format, openApiDocument, out diagnostic, settings);
	}

	/// <summary>
	/// Reads the input string and parses it into an Open API document.
	/// </summary>
	/// <param name="input">The input string.</param>
	/// <param name="format">The Open API format</param>
	/// <param name="settings">The OpenApi reader settings.</param>
	/// <returns>An OpenAPI document instance.</returns>
	public static ReadResult Parse(string input, string? format = null, OpenApiReaderSettings? settings = null)
	{
		ArgumentException.ThrowIfNullOrEmpty(input, "input");
		if (format == null)
		{
			format = InspectInputFormat(input);
		}
		if (settings == null)
		{
			settings = new OpenApiReaderSettings();
		}
		using MemoryStream input2 = new MemoryStream(Encoding.UTF8.GetBytes(input));
		return InternalLoad(input2, format, settings);
	}

	/// <summary>
	/// Reads the input string and parses it into an Open API document.
	/// </summary>
	/// <param name="input">The input string.</param>
	/// <param name="version"></param>
	/// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
	/// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
	/// <param name="format">The Open API format</param>
	/// <param name="settings">The OpenApi reader settings.</param>
	/// <returns>An OpenAPI document instance.</returns>
	public static T? Parse<T>(string input, OpenApiSpecVersion version, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, string? format = null, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
	{
		ArgumentException.ThrowIfNullOrEmpty(input, "input");
		if (format == null)
		{
			format = InspectInputFormat(input);
		}
		if (settings == null)
		{
			settings = new OpenApiReaderSettings();
		}
		using MemoryStream input2 = new MemoryStream(Encoding.UTF8.GetBytes(input));
		return Load<T>(input2, version, format, openApiDocument, out diagnostic, settings);
	}

	private static async Task<ReadResult> InternalLoadAsync(Stream input, string format, OpenApiReaderSettings settings, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (settings == null)
		{
			settings = DefaultReaderSettings.Value;
		}
		IOpenApiReader reader = settings.GetReader(format);
		Uri location = ((input is FileStream fileStream) ? new Uri(fileStream.Name) : null) ?? settings.BaseUrl ?? new Uri("https://openapi.net/");
		ReadResult readResult = await reader.ReadAsync(input, location, settings, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (settings.LoadExternalRefs)
		{
			OpenApiDiagnostic openApiDiagnostic = await LoadExternalRefsAsync(readResult.Document, settings, format, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (openApiDiagnostic != null)
			{
				readResult.Diagnostic?.Errors.AddRange(openApiDiagnostic.Errors);
				readResult.Diagnostic?.Warnings.AddRange(openApiDiagnostic.Warnings);
			}
		}
		return readResult;
	}

	private static async Task<OpenApiDiagnostic> LoadExternalRefsAsync(OpenApiDocument? document, OpenApiReaderSettings settings, string? format = null, CancellationToken token = default(CancellationToken))
	{
		DefaultStreamLoader defaultStreamLoader = new DefaultStreamLoader(settings.HttpClient);
		return await new OpenApiWorkspaceLoader(document?.Workspace ?? new OpenApiWorkspace(), settings.CustomExternalLoader ?? defaultStreamLoader, settings).LoadAsync(new BaseOpenApiReference
		{
			ExternalResource = "/"
		}, document, format ?? "json", null, token).ConfigureAwait(continueOnCapturedContext: false);
	}

	private static ReadResult InternalLoad(MemoryStream input, string format, OpenApiReaderSettings settings)
	{
		if (settings == null)
		{
			settings = DefaultReaderSettings.Value;
		}
		if (settings.LoadExternalRefs)
		{
			throw new InvalidOperationException("Loading external references are not supported when using synchronous methods.");
		}
		if (input.Length == 0L || input.Position == input.Length)
		{
			throw new ArgumentException("Cannot parse the stream: input is empty or contains no elements.");
		}
		Uri location = new Uri("https://openapi.net/");
		return settings.GetReader(format).Read(input, location, settings);
	}

	private static async Task<(Stream, string?)> RetrieveStreamAndFormatAsync(string url, OpenApiReaderSettings settings, CancellationToken token = default(CancellationToken))
	{
		if (string.IsNullOrEmpty(url))
		{
			throw new ArgumentException("Parameter url is null or empty. Please provide the correct path or URL to the file.");
		}
		if (url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https:", StringComparison.OrdinalIgnoreCase))
		{
			HttpResponseMessage obj = await settings.HttpClient.GetAsync(url, token).ConfigureAwait(continueOnCapturedContext: false);
			string? obj2 = obj.Content.Headers.ContentType?.MediaType;
			return new ValueTuple<Stream, string>(item2: ((obj2 != null) ? obj2.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0] : null)?.Split('/').Last().Split('+')
				.Last()
				.Split('-')
				.Last(), item1: await obj.Content.ReadAsStreamAsync(token).ConfigureAwait(continueOnCapturedContext: false));
		}
		string format;
		Stream item;
		try
		{
			string text = ((!url.StartsWith("file:", StringComparison.OrdinalIgnoreCase)) ? url : new Uri(url).LocalPath);
			format = Path.GetExtension(text).Split('.').LastOrDefault();
			item = new FileInfo(text).OpenRead();
		}
		catch (Exception ex) when (((ex is UriFormatException || ex is FormatException || ex is FileNotFoundException || ex is PathTooLongException || ex is DirectoryNotFoundException || ex is IOException || ex is UnauthorizedAccessException || ex is SecurityException || ex is NotSupportedException) ? 1 : 0) != 0)
		{
			throw new InvalidOperationException("Could not open the file at " + url, ex);
		}
		return (item, format);
	}

	private static string InspectInputFormat(string input)
	{
		if (!input.StartsWith("{", StringComparison.OrdinalIgnoreCase) && !input.StartsWith("[", StringComparison.OrdinalIgnoreCase))
		{
			return "yaml";
		}
		return "json";
	}

	private static string InspectStreamFormat(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		long position = stream.Position;
		int num = stream.ReadByte();
		if (char.IsWhiteSpace((char)num))
		{
			num = stream.ReadByte();
		}
		stream.Position = position;
		char c = (char)num;
		if (c == '[' || c == '{')
		{
			return "json";
		}
		return "yaml";
	}

	private static async Task<(Stream, string)> PrepareStreamForReadingAsync(Stream input, string? format, CancellationToken token = default(CancellationToken))
	{
		Stream preparedStream = input;
		if (!input.CanSeek)
		{
			using MemoryStream bufferStream = new MemoryStream();
			await input.CopyToAsync(bufferStream, 1024, token).ConfigureAwait(continueOnCapturedContext: false);
			bufferStream.Position = 0L;
			if (format == null)
			{
				format = InspectStreamFormat(bufferStream);
			}
			if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
			{
				preparedStream = input;
			}
			else
			{
				preparedStream = new MemoryStream();
				bufferStream.Position = 0L;
				await bufferStream.CopyToAsync(preparedStream, 81920, token).ConfigureAwait(continueOnCapturedContext: false);
				await input.CopyToAsync(preparedStream, 81920, token).ConfigureAwait(continueOnCapturedContext: false);
				preparedStream.Position = 0L;
			}
		}
		else
		{
			if (format == null)
			{
				format = InspectStreamFormat(input);
			}
			if (!format.Equals("json", StringComparison.OrdinalIgnoreCase))
			{
				preparedStream = new MemoryStream();
				await input.CopyToAsync(preparedStream, 81920, token).ConfigureAwait(continueOnCapturedContext: false);
				preparedStream.Position = 0L;
			}
		}
		return (preparedStream, format);
	}
}
