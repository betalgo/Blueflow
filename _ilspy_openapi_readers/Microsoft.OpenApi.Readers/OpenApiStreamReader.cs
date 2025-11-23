using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers;

public class OpenApiStreamReader : IOpenApiReader<Stream, OpenApiDiagnostic>
{
	private readonly OpenApiReaderSettings _settings;

	public OpenApiStreamReader(OpenApiReaderSettings settings = null)
	{
		_settings = settings ?? new OpenApiReaderSettings();
		if ((_settings.ReferenceResolution == ReferenceResolutionSetting.ResolveAllReferences || _settings.LoadExternalRefs) && _settings.BaseUrl == null)
		{
			throw new ArgumentException("BaseUrl must be provided to resolve external references.");
		}
	}

	public OpenApiDocument Read(Stream input, out OpenApiDiagnostic diagnostic)
	{
		using StreamReader input2 = new StreamReader(input, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 4096, _settings.LeaveStreamOpen);
		return new OpenApiTextReaderReader(_settings).Read(input2, out diagnostic);
	}

	public async Task<ReadResult> ReadAsync(Stream input, CancellationToken cancellationToken = default(CancellationToken))
	{
		int bufferSize = 4096;
		MemoryStream bufferedStream;
		if (input is MemoryStream memoryStream)
		{
			bufferedStream = memoryStream;
		}
		else
		{
			bufferedStream = new MemoryStream();
			bufferSize = 81920;
			await input.CopyToAsync(bufferedStream, bufferSize, cancellationToken);
			bufferedStream.Position = 0L;
		}
		using StreamReader reader = new StreamReader(bufferedStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize, _settings.LeaveStreamOpen);
		return await new OpenApiTextReaderReader(_settings).ReadAsync(reader, cancellationToken);
	}

	public T ReadFragment<T>(Stream input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiReferenceable
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		using StreamReader input2 = new StreamReader(input, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 4096, _settings.LeaveStreamOpen);
		return new OpenApiTextReaderReader(_settings).ReadFragment<T>((TextReader)input2, version, out diagnostic);
	}
}
