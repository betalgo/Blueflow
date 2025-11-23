namespace Microsoft.OpenApi;

/// <summary>
/// This class is used to walk an OpenApiDocument and sets the host document of IOpenApiReferenceable objects
/// </summary>
internal class ReferenceHostDocumentSetter : OpenApiVisitorBase
{
	private readonly OpenApiDocument _currentDocument;

	public ReferenceHostDocumentSetter(OpenApiDocument currentDocument)
	{
		_currentDocument = currentDocument;
	}

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
		baseOpenApiReference.EnsureHostDocumentIsSet(_currentDocument);
	}
}
