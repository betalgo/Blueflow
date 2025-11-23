using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers;

public class OpenApiDiagnostic : IDiagnostic
{
	public IList<OpenApiError> Errors { get; set; } = new List<OpenApiError>();

	public IList<OpenApiError> Warnings { get; set; } = new List<OpenApiError>();

	public OpenApiSpecVersion SpecificationVersion { get; set; }

	public void AppendDiagnostic(OpenApiDiagnostic diagnosticToAdd, string fileNameToAdd = null)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		bool flag = !string.IsNullOrEmpty(fileNameToAdd);
		foreach (OpenApiError error in diagnosticToAdd.Errors)
		{
			string text = (flag ? ("[File: " + fileNameToAdd + "] " + error.Message) : error.Message);
			Errors.Add(new OpenApiError(error.Pointer, text));
		}
		foreach (OpenApiError warning in diagnosticToAdd.Warnings)
		{
			string text2 = (flag ? ("[File: " + fileNameToAdd + "] " + warning.Message) : warning.Message);
			Warnings.Add(new OpenApiError(warning.Pointer, text2));
		}
	}
}
