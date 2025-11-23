using System;
using Microsoft.OpenApi.Exceptions;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.Exceptions;

[Serializable]
public class OpenApiReaderException : OpenApiException
{
	public OpenApiReaderException()
	{
	}

	public OpenApiReaderException(string message)
		: base(message)
	{
	}

	public OpenApiReaderException(string message, ParsingContext context)
		: base(message)
	{
		((OpenApiException)this).Pointer = context.GetLocation();
	}

	public OpenApiReaderException(string message, YamlNode node)
		: base(message)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Mark start = node.Start;
		((OpenApiException)this).Pointer = $"#line={((Mark)(ref start)).Line}";
	}

	public OpenApiReaderException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
