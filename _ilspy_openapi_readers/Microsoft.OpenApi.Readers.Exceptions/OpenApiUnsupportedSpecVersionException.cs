using System;
using System.Globalization;

namespace Microsoft.OpenApi.Readers.Exceptions;

[Serializable]
public class OpenApiUnsupportedSpecVersionException : Exception
{
	private const string messagePattern = "OpenAPI specification version '{0}' is not supported.";

	public string SpecificationVersion { get; }

	public OpenApiUnsupportedSpecVersionException(string specificationVersion)
		: base(string.Format(CultureInfo.InvariantCulture, "OpenAPI specification version '{0}' is not supported.", specificationVersion))
	{
		SpecificationVersion = specificationVersion;
	}

	public OpenApiUnsupportedSpecVersionException(string specificationVersion, Exception innerException)
		: base(string.Format(CultureInfo.InvariantCulture, "OpenAPI specification version '{0}' is not supported.", specificationVersion), innerException)
	{
		SpecificationVersion = specificationVersion;
	}
}
