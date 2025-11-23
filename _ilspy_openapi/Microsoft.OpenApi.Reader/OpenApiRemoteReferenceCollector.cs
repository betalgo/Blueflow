using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Builds a list of all remote references used in an OpenApi document
/// </summary>
internal class OpenApiRemoteReferenceCollector : OpenApiVisitorBase
{
	private readonly Dictionary<string, BaseOpenApiReference> _references = new Dictionary<string, BaseOpenApiReference>();

	/// <summary>
	/// List of all external references collected from OpenApiDocument
	/// </summary>
	public IEnumerable<BaseOpenApiReference> References => _references.Values;

	/// <inheritdoc />
	public override void Visit(IOpenApiReferenceHolder referenceHolder)
	{
		BaseOpenApiReference baseOpenApiReference;
		if (!(referenceHolder is IOpenApiReferenceHolder<OpenApiReferenceWithDescriptionAndSummary> { Reference: { } reference }))
		{
			if (!(referenceHolder is IOpenApiReferenceHolder<OpenApiReferenceWithDescription> { Reference: { } reference2 }))
			{
				if (!(referenceHolder is IOpenApiReferenceHolder<JsonSchemaReference> { Reference: { } reference3 }))
				{
					if (!(referenceHolder is IOpenApiReferenceHolder<BaseOpenApiReference> { Reference: { } reference4 }))
					{
						throw new OpenApiException("Unsupported reference holder type: " + referenceHolder.GetType().FullName);
					}
					baseOpenApiReference = reference4;
				}
				else
				{
					baseOpenApiReference = reference3;
				}
			}
			else
			{
				baseOpenApiReference = reference2;
			}
		}
		else
		{
			baseOpenApiReference = reference;
		}
		BaseOpenApiReference reference5 = baseOpenApiReference;
		AddExternalReferences(reference5);
	}

	/// <summary>
	/// Collect external references
	/// </summary>
	private void AddExternalReferences(BaseOpenApiReference? reference)
	{
		if (reference != null && reference.IsExternal)
		{
			string externalResource = reference.ExternalResource;
			if (externalResource != null && !_references.ContainsKey(externalResource))
			{
				_references.Add(externalResource, reference);
			}
		}
	}
}
