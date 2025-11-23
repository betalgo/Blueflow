using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.Interface;

internal interface IOpenApiVersionService
{
	OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type);

	T LoadElement<T>(ParseNode node) where T : IOpenApiElement;

	OpenApiDocument LoadDocument(RootNode rootNode);
}
