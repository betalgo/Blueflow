using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// A reader class for parsing JSON files into Open API documents.
/// </summary>
public class OpenApiJsonReader : IOpenApiReader
{
	/// <summary>
	/// Reads the memory stream input and parses it into an Open API document.
	/// </summary>
	/// <param name="input">Memory stream containing OpenAPI description to parse.</param>
	/// <param name="location">Location of where the document that is getting loaded is saved</param>
	/// <param name="settings">The Reader settings to be used during parsing.</param>
	/// <returns></returns>
	public ReadResult Read(MemoryStream input, Uri location, OpenApiReaderSettings settings)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (settings == null)
		{
			throw new ArgumentNullException("settings");
		}
		OpenApiDiagnostic openApiDiagnostic = new OpenApiDiagnostic();
		if (settings == null)
		{
			settings = new OpenApiReaderSettings();
		}
		JsonNode jsonNode;
		try
		{
			jsonNode = JsonNode.Parse(input) ?? throw new InvalidOperationException("Cannot parse input stream, input.");
		}
		catch (JsonException ex)
		{
			openApiDiagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", "Please provide the correct format, " + ex.Message));
			openApiDiagnostic.Format = "json";
			return new ReadResult
			{
				Document = null,
				Diagnostic = openApiDiagnostic
			};
		}
		return Read(jsonNode, location, settings);
	}

	/// <summary>
	/// Parses the JsonNode input into an Open API document.
	/// </summary>
	/// <param name="jsonNode">The JsonNode input.</param>
	/// <param name="location">Location of where the document that is getting loaded is saved</param>
	/// <param name="settings">The Reader settings to be used during parsing.</param>
	/// <returns></returns>
	public ReadResult Read(JsonNode jsonNode, Uri location, OpenApiReaderSettings settings)
	{
		if (jsonNode == null)
		{
			throw new ArgumentNullException("jsonNode");
		}
		if (settings == null)
		{
			throw new ArgumentNullException("settings");
		}
		OpenApiDiagnostic openApiDiagnostic = new OpenApiDiagnostic();
		ParsingContext parsingContext = new ParsingContext(openApiDiagnostic)
		{
			ExtensionParsers = settings.ExtensionParsers,
			BaseUrl = settings.BaseUrl,
			DefaultContentType = settings.DefaultContentType
		};
		OpenApiDocument openApiDocument = null;
		try
		{
			openApiDocument = parsingContext.Parse(jsonNode, location);
			openApiDocument.SetReferenceHostDocument();
		}
		catch (OpenApiException exception)
		{
			openApiDiagnostic.Errors.Add(new OpenApiError(exception));
		}
		if (openApiDocument != null && settings.RuleSet != null && settings.RuleSet.Rules.Any())
		{
			IEnumerable<OpenApiError> enumerable = openApiDocument.Validate(settings.RuleSet);
			if (enumerable != null)
			{
				foreach (OpenApiValidatorError item in enumerable.OfType<OpenApiValidatorError>())
				{
					openApiDiagnostic.Errors.Add(item);
				}
				foreach (OpenApiValidatorWarning item2 in enumerable.OfType<OpenApiValidatorWarning>())
				{
					openApiDiagnostic.Warnings.Add(item2);
				}
			}
		}
		openApiDiagnostic.Format = "json";
		return new ReadResult
		{
			Document = openApiDocument,
			Diagnostic = openApiDiagnostic
		};
	}

	/// <summary>
	/// Reads the stream input asynchronously and parses it into an Open API document.
	/// </summary>
	/// <param name="input">Memory stream containing OpenAPI description to parse.</param>
	/// <param name="location">Location of where the document that is getting loaded is saved</param>
	/// <param name="settings">The Reader settings to be used during parsing.</param>
	/// <param name="cancellationToken">Propagates notifications that operations should be cancelled.</param>
	/// <returns></returns>
	public async Task<ReadResult> ReadAsync(Stream input, Uri location, OpenApiReaderSettings settings, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (settings == null)
		{
			throw new ArgumentNullException("settings");
		}
		OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();
		JsonNode jsonNode;
		try
		{
			jsonNode = (await JsonNode.ParseAsync(input, null, default(JsonDocumentOptions), cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? throw new InvalidOperationException("failed to parse input stream, input");
		}
		catch (JsonException ex)
		{
			diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", "Please provide the correct format, " + ex.Message));
			diagnostic.Format = "json";
			return new ReadResult
			{
				Document = null,
				Diagnostic = diagnostic
			};
		}
		return Read(jsonNode, location, settings);
	}

	/// <inheritdoc />
	public T? ReadFragment<T>(MemoryStream input, OpenApiSpecVersion version, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
	{
		Utils.CheckArgumentNull(input, "input");
		Utils.CheckArgumentNull(openApiDocument, "openApiDocument");
		JsonNode input2;
		try
		{
			input2 = JsonNode.Parse(input) ?? throw new InvalidOperationException("Failed to parse stream, input");
		}
		catch (JsonException ex)
		{
			diagnostic = new OpenApiDiagnostic();
			diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", ex.Message));
			return default(T);
		}
		return ReadFragment<T>(input2, version, openApiDocument, out diagnostic);
	}

	/// <inheritdoc />
	public T? ReadFragment<T>(JsonNode input, OpenApiSpecVersion version, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
	{
		diagnostic = new OpenApiDiagnostic();
		if (settings == null)
		{
			settings = new OpenApiReaderSettings();
		}
		ParsingContext parsingContext = new ParsingContext(diagnostic)
		{
			ExtensionParsers = settings.ExtensionParsers
		};
		IOpenApiElement openApiElement = null;
		try
		{
			openApiElement = parsingContext.ParseFragment<T>(input, version, openApiDocument);
		}
		catch (OpenApiException exception)
		{
			diagnostic.Errors.Add(new OpenApiError(exception));
		}
		if (openApiElement != null && settings.RuleSet != null && settings.RuleSet.Rules.Any())
		{
			IEnumerable<OpenApiError> enumerable = openApiElement.Validate(settings.RuleSet);
			if (enumerable != null)
			{
				foreach (OpenApiError item in enumerable)
				{
					diagnostic.Errors.Add(item);
				}
			}
		}
		return (T)openApiElement;
	}
}
