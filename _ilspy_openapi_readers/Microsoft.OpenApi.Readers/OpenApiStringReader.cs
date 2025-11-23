using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers;

public class OpenApiStringReader : IOpenApiReader<string, OpenApiDiagnostic>
{
	private readonly OpenApiReaderSettings _settings;

	public OpenApiStringReader(OpenApiReaderSettings settings = null)
	{
		_settings = settings ?? new OpenApiReaderSettings();
	}

	public OpenApiDocument Read(string input, out OpenApiDiagnostic diagnostic)
	{
		using StringReader input2 = new StringReader(input);
		return new OpenApiTextReaderReader(_settings).Read(input2, out diagnostic);
	}

	public T ReadFragment<T>(string input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		using StringReader input2 = new StringReader(input);
		return new OpenApiTextReaderReader(_settings).ReadFragment<T>(input2, version, out diagnostic);
	}
}
