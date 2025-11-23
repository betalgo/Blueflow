namespace Microsoft.OpenApi;

/// <summary>
/// Configuration settings to control how OpenAPI Json documents are written
/// </summary>
public class OpenApiJsonWriterSettings : OpenApiWriterSettings
{
	/// <summary>
	/// Indicates whether or not the produced document will be written in a compact or pretty fashion.
	/// </summary>
	public bool Terse { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiJsonWriterSettings" /> class.
	/// </summary>
	public OpenApiJsonWriterSettings()
	{
	}
}
