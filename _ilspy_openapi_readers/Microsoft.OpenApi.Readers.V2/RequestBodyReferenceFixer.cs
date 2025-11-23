using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers.V2;

internal class RequestBodyReferenceFixer : OpenApiVisitorBase
{
	private IDictionary<string, OpenApiRequestBody> _requestBodies;

	public RequestBodyReferenceFixer(IDictionary<string, OpenApiRequestBody> requestBodies)
	{
		_requestBodies = requestBodies;
	}

	public override void Visit(OpenApiOperation operation)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0062: Expected O, but got Unknown
		OpenApiParameter val = operation.Parameters.FirstOrDefault((OpenApiParameter p) => p.UnresolvedReference && _requestBodies.ContainsKey(p.Reference.Id));
		if (val != null)
		{
			operation.Parameters.Remove(val);
			operation.RequestBody = new OpenApiRequestBody
			{
				UnresolvedReference = true,
				Reference = new OpenApiReference
				{
					Id = val.Reference.Id,
					Type = (ReferenceType)4
				}
			};
		}
	}
}
