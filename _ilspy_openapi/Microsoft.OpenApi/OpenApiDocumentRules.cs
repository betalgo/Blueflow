using System;

namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiDocument" />.
/// </summary>
[OpenApiRule]
public static class OpenApiDocumentRules
{
	private sealed class OpenApiSchemaReferenceVisitor(string ruleName, IValidationContext context) : OpenApiVisitorBase()
	{
		public override void Visit(IOpenApiReferenceHolder referenceHolder)
		{
			if (referenceHolder is OpenApiSchemaReference reference)
			{
				ValidateSchemaReference(reference);
			}
		}

		public override void Visit(IOpenApiSchema schema)
		{
			if (schema is OpenApiSchemaReference reference)
			{
				ValidateSchemaReference(reference);
			}
		}

		private void ValidateSchemaReference(OpenApiSchemaReference reference)
		{
			if (!reference.Reference.IsLocal)
			{
				return;
			}
			try
			{
				if (reference.RecursiveTarget == null)
				{
					context.Enter(GetSegment());
					context.CreateWarning(ruleName, string.Format(SRResource.Validation_SchemaReferenceDoesNotExist, reference.Reference.ReferenceV3));
					context.Exit();
				}
			}
			catch (InvalidOperationException ex)
			{
				context.Enter(GetSegment());
				context.CreateWarning(ruleName, ex.Message);
				context.Exit();
			}
			string GetSegment()
			{
				string pathString = base.PathString;
				return pathString.Substring(2, pathString.Length - 2) + "/$ref";
			}
		}
	}

	/// <summary>
	/// The Info field is required.
	/// </summary>
	public static ValidationRule<OpenApiDocument> OpenApiDocumentFieldIsMissing => new ValidationRule<OpenApiDocument>("OpenApiDocumentFieldIsMissing", delegate(IValidationContext context, OpenApiDocument item)
	{
		if (item.Info == null)
		{
			context.Enter("info");
			context.CreateError("OpenApiDocumentFieldIsMissing", string.Format(SRResource.Validation_FieldIsRequired, "info", "document"));
			context.Exit();
		}
	});

	/// <summary>
	/// All references in the OpenAPI document must be valid.
	/// </summary>
	public static ValidationRule<OpenApiDocument> OpenApiDocumentReferencesAreValid => new ValidationRule<OpenApiDocument>("OpenApiDocumentReferencesAreValid", delegate(IValidationContext context, OpenApiDocument item)
	{
		new OpenApiWalker(new OpenApiSchemaReferenceVisitor("OpenApiDocumentReferencesAreValid", context)).Walk(item);
	});
}
