using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Reader.V2;

internal class RequestBodyReferenceFixer : OpenApiVisitorBase
{
	private readonly IDictionary<string, IOpenApiRequestBody> _requestBodies;

	public RequestBodyReferenceFixer(IDictionary<string, IOpenApiRequestBody> requestBodies)
	{
		_requestBodies = requestBodies;
	}

	public override void Visit(OpenApiOperation operation)
	{
		OpenApiParameterReference openApiParameterReference = operation.Parameters?.OfType<OpenApiParameterReference>().FirstOrDefault((OpenApiParameterReference p) => p.UnresolvedReference && p.Reference?.Id != null && _requestBodies.ContainsKey(p.Reference.Id));
		string text = openApiParameterReference?.Reference?.Id;
		if (openApiParameterReference != null && !string.IsNullOrEmpty(text) && text != null)
		{
			operation.Parameters?.Remove(openApiParameterReference);
			operation.RequestBody = new OpenApiRequestBodyReference(text, openApiParameterReference.Reference?.HostDocument);
		}
	}
}
