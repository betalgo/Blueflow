using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers;

public class ReadResult
{
	public OpenApiDocument OpenApiDocument { get; set; }

	public OpenApiDiagnostic OpenApiDiagnostic { get; set; }
}
