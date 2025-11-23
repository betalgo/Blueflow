using System;
using System.Globalization;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines an exception indicating OpenAPI Reader encountered an unsupported specification version while reading.
/// </summary>
[Serializable]
public class OpenApiUnsupportedSpecVersionException : Exception
{
	private const string messagePattern = "OpenAPI specification version '{0}' is not supported.";

	/// <summary>
	/// The unsupported specification version.
	/// </summary>
	public string SpecificationVersion { get; }

	/// <summary>
	/// Initializes the <see cref="T:Microsoft.OpenApi.OpenApiUnsupportedSpecVersionException" /> class with a specification version.
	/// </summary>
	/// <param name="specificationVersion">Version that caused this exception to be thrown.</param>
	public OpenApiUnsupportedSpecVersionException(string specificationVersion)
		: base(string.Format(CultureInfo.InvariantCulture, "OpenAPI specification version '{0}' is not supported.", specificationVersion))
	{
		SpecificationVersion = specificationVersion;
	}

	/// <summary>
	/// Initializes the <see cref="T:Microsoft.OpenApi.OpenApiUnsupportedSpecVersionException" /> class with a specification version and
	/// inner exception.
	/// </summary>
	/// <param name="specificationVersion">Version that caused this exception to be thrown.</param>
	/// <param name="innerException">Inner exception that caused this exception to be thrown.</param>
	public OpenApiUnsupportedSpecVersionException(string specificationVersion, Exception innerException)
		: base(string.Format(CultureInfo.InvariantCulture, "OpenAPI specification version '{0}' is not supported.", specificationVersion), innerException)
	{
		SpecificationVersion = specificationVersion;
	}
}
