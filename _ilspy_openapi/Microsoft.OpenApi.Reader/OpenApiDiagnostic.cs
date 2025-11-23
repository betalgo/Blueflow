using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Object containing all diagnostic information related to Open API parsing.
/// </summary>
public class OpenApiDiagnostic
{
	/// <summary>
	/// List of all errors.
	/// </summary>
	public IList<OpenApiError> Errors { get; set; } = new List<OpenApiError>();

	/// <summary>
	/// List of all warnings
	/// </summary>
	public IList<OpenApiError> Warnings { get; set; } = new List<OpenApiError>();

	/// <summary>
	/// Open API specification version of the document parsed.
	/// </summary>
	public OpenApiSpecVersion SpecificationVersion { get; set; }

	/// <summary>
	/// The format of the OpenAPI document (e.g., "json", "yaml").
	/// </summary>
	public string? Format { get; set; }

	/// <summary>
	/// Append another set of diagnostic Errors and Warnings to this one, this may be appended from another external
	/// document's parsing and we want to indicate which file it originated from.
	/// </summary>
	/// <param name="diagnosticToAdd">The diagnostic instance of which the errors and warnings are to be appended to this diagnostic's</param>
	/// <param name="fileNameToAdd">The originating file of the diagnostic to be appended, this is prefixed to each error and warning to indicate the originating file</param>
	public void AppendDiagnostic(OpenApiDiagnostic diagnosticToAdd, string? fileNameToAdd = null)
	{
		bool flag = !string.IsNullOrEmpty(fileNameToAdd);
		foreach (OpenApiError error in diagnosticToAdd.Errors)
		{
			string message = (flag ? ("[File: " + fileNameToAdd + "] " + error.Message) : error.Message);
			Errors.Add(new OpenApiError(error.Pointer, message));
		}
		foreach (OpenApiError warning in diagnosticToAdd.Warnings)
		{
			string message2 = (flag ? ("[File: " + fileNameToAdd + "] " + warning.Message) : warning.Message);
			Warnings.Add(new OpenApiError(warning.Pointer, message2));
		}
	}
}
