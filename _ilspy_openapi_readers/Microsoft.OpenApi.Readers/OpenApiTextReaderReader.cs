using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers;

public class OpenApiTextReaderReader : IOpenApiReader<TextReader, OpenApiDiagnostic>
{
	private readonly OpenApiReaderSettings _settings;

	public OpenApiTextReaderReader(OpenApiReaderSettings settings = null)
	{
		_settings = settings ?? new OpenApiReaderSettings();
	}

	public OpenApiDocument Read(TextReader input, out OpenApiDiagnostic diagnostic)
	{
		//IL_000a: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		YamlDocument input2;
		try
		{
			input2 = LoadYamlDocument(input);
		}
		catch (YamlException ex)
		{
			YamlException ex2 = ex;
			diagnostic = new OpenApiDiagnostic();
			IList<OpenApiError> errors = diagnostic.Errors;
			Mark start = ex2.Start;
			errors.Add(new OpenApiError($"#line={((Mark)(ref start)).Line}", ((Exception)(object)ex2).Message));
			return new OpenApiDocument();
		}
		return new OpenApiYamlDocumentReader(_settings).Read(input2, out diagnostic);
	}

	public async Task<ReadResult> ReadAsync(TextReader input, CancellationToken cancellationToken = default(CancellationToken))
	{
		YamlDocument input2;
		try
		{
			input2 = LoadYamlDocument(input);
		}
		catch (YamlException ex)
		{
			YamlException ex2 = ex;
			OpenApiDiagnostic openApiDiagnostic = new OpenApiDiagnostic();
			IList<OpenApiError> errors = openApiDiagnostic.Errors;
			Mark start = ex2.Start;
			errors.Add(new OpenApiError($"#line={((Mark)(ref start)).Line}", ((Exception)(object)ex2).Message));
			return new ReadResult
			{
				OpenApiDocument = null,
				OpenApiDiagnostic = openApiDiagnostic
			};
		}
		return await new OpenApiYamlDocumentReader(_settings).ReadAsync(input2, cancellationToken);
	}

	public T ReadFragment<T>(TextReader input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
	{
		//IL_000a: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		YamlDocument input2;
		try
		{
			input2 = LoadYamlDocument(input);
		}
		catch (YamlException ex)
		{
			YamlException ex2 = ex;
			diagnostic = new OpenApiDiagnostic();
			IList<OpenApiError> errors = diagnostic.Errors;
			Mark start = ex2.Start;
			errors.Add(new OpenApiError($"#line={((Mark)(ref start)).Line}", ((Exception)(object)ex2).Message));
			return default(T);
		}
		return new OpenApiYamlDocumentReader(_settings).ReadFragment<T>(input2, version, out diagnostic);
	}

	private static YamlDocument LoadYamlDocument(TextReader input)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		YamlStream val = new YamlStream();
		val.Load(input);
		return val.Documents.First();
	}
}
