using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers.Services;

internal class OpenApiRemoteReferenceCollector : OpenApiVisitorBase
{
	private Dictionary<string, OpenApiReference> _references = new Dictionary<string, OpenApiReference>();

	public IEnumerable<OpenApiReference> References => _references.Values;

	public override void Visit(IOpenApiReferenceable referenceable)
	{
		AddReference(referenceable.Reference);
	}

	private void AddReference(OpenApiReference reference)
	{
		if (reference != null && reference.IsExternal && !_references.ContainsKey(reference.ExternalResource))
		{
			_references.Add(reference.ExternalResource, reference);
		}
	}
}
