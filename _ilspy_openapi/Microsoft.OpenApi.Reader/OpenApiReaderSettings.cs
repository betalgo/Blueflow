using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Configuration settings to control how OpenAPI documents are parsed
/// </summary>
public class OpenApiReaderSettings
{
	private static readonly Lazy<HttpClient> httpClient = new Lazy<HttpClient>(() => new HttpClient());

	private HttpClient? _httpClient;

	private Dictionary<string, IOpenApiReader> _readers = new Dictionary<string, IOpenApiReader>(StringComparer.OrdinalIgnoreCase) { 
	{
		"json",
		new OpenApiJsonReader()
	} };

	/// <summary>
	/// HttpClient to use for making requests and retrieve documents
	/// </summary>
	public HttpClient HttpClient
	{
		internal get
		{
			if (_httpClient == null)
			{
				_httpClient = httpClient.Value;
			}
			return _httpClient;
		}
		init
		{
			_httpClient = value;
		}
	}

	/// <summary>
	/// Readers to use to parse the OpenAPI document
	/// </summary>
	public Dictionary<string, IOpenApiReader> Readers
	{
		get
		{
			return _readers;
		}
		init
		{
			Utils.CheckArgumentNull(value, "value");
			_readers = ((value.Comparer is StringComparer stringComparer && stringComparer == StringComparer.OrdinalIgnoreCase) ? value : new Dictionary<string, IOpenApiReader>(value, StringComparer.OrdinalIgnoreCase));
		}
	}

	/// <summary>
	/// When external references are found, load them into a shared workspace
	/// </summary>
	public bool LoadExternalRefs { get; set; }

	/// <summary>
	/// Dictionary of parsers for converting extensions into strongly typed classes
	/// </summary>
	public Dictionary<string, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension>>? ExtensionParsers { get; set; } = new Dictionary<string, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension>>();

	/// <summary>
	/// Rules to use for validating OpenAPI specification.  If none are provided a default set of rules are applied.
	/// </summary>
	public ValidationRuleSet RuleSet { get; set; } = ValidationRuleSet.GetDefaultRuleSet();

	/// <summary>
	/// URL where relative references should be resolved from if the description does not contain Server definitions
	/// </summary>
	public Uri? BaseUrl { get; set; }

	/// <summary>
	/// Allows clients to define a custom DefaultContentType if produces array is empty
	/// </summary>
	public List<string>? DefaultContentType { get; set; }

	/// <summary>
	/// Function used to provide an alternative loader for accessing external references.
	/// </summary>
	/// <remarks>
	/// Default loader will attempt to dereference http(s) urls and file urls.
	/// </remarks>
	public IStreamLoader? CustomExternalLoader { get; set; }

	/// <summary>
	/// Whether to leave the <see cref="T:System.IO.Stream" /> object open after reading
	/// from an OpenApiStreamReader object.
	/// </summary>
	public bool LeaveStreamOpen { get; set; }

	/// <summary>
	/// Adds a reader for the specified format
	/// </summary>
	public void AddJsonReader()
	{
		TryAddReader("json", new OpenApiJsonReader());
	}

	/// <summary>
	/// Gets the reader for the specified format
	/// </summary>
	/// <param name="format">Format to fetch the reader for</param>
	/// <returns>The retrieved reader</returns>
	/// <exception cref="T:System.NotSupportedException">When no reader is registered for that format</exception>
	internal IOpenApiReader GetReader(string format)
	{
		Utils.CheckArgumentNullOrEmpty(format, "format");
		if (Readers.TryGetValue(format, out IOpenApiReader value))
		{
			return value;
		}
		throw new NotSupportedException("Format '" + format + "' is not supported.");
	}

	/// <summary>
	/// Adds a reader for the specified format.
	/// This method is a no-op if the reader already exists.
	/// This method is equivalent to TryAdd, is provided for compatibility reasons and TryAdd should be used instead when available.
	/// </summary>
	/// <param name="format">Format to add a reader for</param>
	/// <param name="reader">Reader to add</param>
	/// <returns>True if the reader was added, false if it already existed</returns>
	public bool TryAddReader(string format, IOpenApiReader reader)
	{
		Utils.CheckArgumentNullOrEmpty(format, "format");
		Utils.CheckArgumentNull(reader, "reader");
		return Readers.TryAdd(format, reader);
	}

	/// <summary>
	/// Adds parsers for Microsoft OpenAPI extensions:
	/// - <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiPagingExtension" />
	/// - <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiEnumValuesDescriptionExtension" />
	/// - <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiPrimaryErrorMessageExtension" />
	/// - <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiDeprecationExtension" />
	/// - <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiReservedParameterExtension" />
	/// - <see cref="T:Microsoft.OpenApi.MicrosoftExtensions.OpenApiEnumFlagsExtension" />
	/// NOTE: The list of extensions is subject to change.
	/// </summary>
	public void AddMicrosoftExtensionParsers()
	{
		TryAddExtensionParser(OpenApiPagingExtension.Name, (JsonNode i, OpenApiSpecVersion _) => OpenApiPagingExtension.Parse(i));
		TryAddExtensionParser(OpenApiEnumValuesDescriptionExtension.Name, (JsonNode i, OpenApiSpecVersion _) => OpenApiEnumValuesDescriptionExtension.Parse(i));
		TryAddExtensionParser(OpenApiPrimaryErrorMessageExtension.Name, (JsonNode i, OpenApiSpecVersion _) => OpenApiPrimaryErrorMessageExtension.Parse(i));
		TryAddExtensionParser(OpenApiDeprecationExtension.Name, (JsonNode i, OpenApiSpecVersion _) => OpenApiDeprecationExtension.Parse(i));
		TryAddExtensionParser(OpenApiReservedParameterExtension.Name, (JsonNode i, OpenApiSpecVersion _) => OpenApiReservedParameterExtension.Parse(i));
		TryAddExtensionParser(OpenApiEnumFlagsExtension.Name, (JsonNode i, OpenApiSpecVersion _) => OpenApiEnumFlagsExtension.Parse(i));
	}

	private void TryAddExtensionParser(string name, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension> parser)
	{
		ExtensionParsers?.TryAdd(name, parser);
	}
}
