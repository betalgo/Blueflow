using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.YamlReader;

public class OpenApiYamlReader : IOpenApiReader
{
	private const int copyBufferSize = 4096;

	private static readonly OpenApiJsonReader _jsonReader = new OpenApiJsonReader();

	public async Task<ReadResult> ReadAsync(Stream input, Uri location, OpenApiReaderSettings settings, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (input is MemoryStream input2)
		{
			return UpdateFormat(Read(input2, location, settings));
		}
		using MemoryStream preparedStream = new MemoryStream();
		await input.CopyToAsync(preparedStream, 4096, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		preparedStream.Position = 0L;
		return UpdateFormat(Read(preparedStream, location, settings));
	}

	public ReadResult Read(MemoryStream input, Uri location, OpenApiReaderSettings settings)
	{
		//IL_004b: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (settings == null)
		{
			throw new ArgumentNullException("settings");
		}
		JsonNode jsonNode;
		try
		{
			using StreamReader input2 = new StreamReader(input, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 4096, settings.LeaveStreamOpen);
			jsonNode = LoadJsonNodesFromYamlDocument(input2);
		}
		catch (JsonException ex)
		{
			JsonException ex2 = ex;
			OpenApiDiagnostic val = new OpenApiDiagnostic();
			val.Errors.Add(new OpenApiError($"#line={ex2.LineNumber}", ((Exception)(object)ex2).Message));
			val.Format = "yaml";
			return new ReadResult
			{
				Document = null,
				Diagnostic = val
			};
		}
		return UpdateFormat(Read(jsonNode, location, settings));
	}

	private static ReadResult UpdateFormat(ReadResult result)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0017: Expected O, but got Unknown
		if (result.Diagnostic == null)
		{
			OpenApiDiagnostic val = new OpenApiDiagnostic();
			OpenApiDiagnostic val2 = val;
			result.Diagnostic = val;
		}
		result.Diagnostic.Format = "yaml";
		return result;
	}

	public static ReadResult Read(JsonNode jsonNode, Uri location, OpenApiReaderSettings settings)
	{
		return UpdateFormat(_jsonReader.Read(jsonNode, location, settings));
	}

	public T? ReadFragment<T>(MemoryStream input, OpenApiSpecVersion version, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
	{
		//IL_002c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		JsonNode input3;
		try
		{
			using StreamReader input2 = new StreamReader(input);
			input3 = LoadJsonNodesFromYamlDocument(input2);
		}
		catch (JsonException ex)
		{
			JsonException ex2 = ex;
			diagnostic = new OpenApiDiagnostic();
			diagnostic.Errors.Add(new OpenApiError($"#line={ex2.LineNumber}", ((Exception)(object)ex2).Message));
			return default(T);
		}
		return ReadFragment<T>(input3, version, openApiDocument, out diagnostic, settings);
	}

	public static T? ReadFragment<T>(JsonNode input, OpenApiSpecVersion version, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _jsonReader.ReadFragment<T>(input, version, openApiDocument, ref diagnostic, settings);
	}

	private static JsonNode LoadJsonNodesFromYamlDocument(TextReader input)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		YamlStream val = new YamlStream();
		val.Load(input);
		if (val.Documents.Any())
		{
			JsonNode val2 = val.Documents[0].ToJsonNode();
			if (val2 != null)
			{
				return val2;
			}
		}
		throw new InvalidOperationException("No documents found in the YAML stream.");
	}
}
