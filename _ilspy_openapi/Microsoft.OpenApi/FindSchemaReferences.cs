using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi;

internal class FindSchemaReferences : OpenApiVisitorBase
{
	private Dictionary<string, IOpenApiSchema> Schemas = new Dictionary<string, IOpenApiSchema>(StringComparer.Ordinal);

	public static void ResolveSchemas(OpenApiComponents? components, Dictionary<string, IOpenApiSchema> schemas)
	{
		FindSchemaReferences findSchemaReferences = new FindSchemaReferences();
		findSchemaReferences.Schemas = schemas;
		new OpenApiWalker(findSchemaReferences).Walk(components);
	}

	/// <inheritdoc />
	public override void Visit(IOpenApiReferenceHolder referenceHolder)
	{
		if (referenceHolder is OpenApiSchemaReference openApiSchemaReference)
		{
			string text = openApiSchemaReference.Reference?.Id;
			if (text != null && Schemas != null && !Schemas.ContainsKey(text))
			{
				Schemas.Add(text, openApiSchemaReference);
			}
		}
		base.Visit(referenceHolder);
	}

	public override void Visit(IOpenApiSchema schema)
	{
		if (schema is OpenApiSchemaReference { Reference: not null } openApiSchemaReference)
		{
			string text = openApiSchemaReference.Reference?.Id;
			if (text != null && Schemas != null && !Schemas.ContainsKey(text))
			{
				Schemas.Add(text, schema);
			}
		}
		base.Visit(schema);
	}
}
