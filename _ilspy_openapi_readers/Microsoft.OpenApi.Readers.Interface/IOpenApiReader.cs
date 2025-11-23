using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.Interface;

public interface IOpenApiReader<TInput, TDiagnostic> where TDiagnostic : IDiagnostic
{
	OpenApiDocument Read(TInput input, out TDiagnostic diagnostic);
}
