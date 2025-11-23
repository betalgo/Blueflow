using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.OpenApiSchema" />.
/// </summary>
[OpenApiRule]
public static class OpenApiSchemaRules
{
	/// <summary>
	/// Validates Schema Property has value
	/// </summary>
	public static ValidationRule<IOpenApiSchema> ValidateSchemaPropertyHasValue => new ValidationRule<IOpenApiSchema>("ValidateSchemaPropertyHasValue", delegate(IValidationContext context, IOpenApiSchema schema)
	{
		if (schema.Properties != null)
		{
			foreach (KeyValuePair<string, IOpenApiSchema> item in schema.Properties.Where<KeyValuePair<string, IOpenApiSchema>>((KeyValuePair<string, IOpenApiSchema> entry) => entry.Value == null))
			{
				context.Enter(item.Key);
				context.CreateError("ValidateSchemaPropertyHasValue", string.Format(SRResource.Validation_SchemaPropertyObjectRequired, (!(schema is OpenApiSchemaReference { Reference: not null } openApiSchemaReference)) ? string.Empty : openApiSchemaReference.Reference.Id, item.Key));
				context.Exit();
			}
		}
	});

	/// <summary>
	/// Validates Schema Discriminator
	/// </summary>
	public static ValidationRule<IOpenApiSchema> ValidateSchemaDiscriminator => new ValidationRule<IOpenApiSchema>("ValidateSchemaDiscriminator", delegate(IValidationContext context, IOpenApiSchema schema)
	{
		if (schema != null && schema.Discriminator != null)
		{
			string text = schema.Discriminator?.PropertyName;
			if (!ValidateChildSchemaAgainstDiscriminator(schema, text))
			{
				context.Enter("discriminator");
				context.CreateError("ValidateSchemaDiscriminator", string.Format(SRResource.Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator, (schema is OpenApiSchemaReference { Reference: not null } openApiSchemaReference) ? openApiSchemaReference.Reference.Id : string.Empty, text));
				context.Exit();
			}
		}
	});

	/// <summary>
	/// Validates the property name in the discriminator against the ones present in the children schema
	/// </summary>
	/// <param name="schema">The parent schema.</param>
	/// <param name="discriminatorName">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
	/// between other schemas which may satisfy the payload description.</param>
	public static bool ValidateChildSchemaAgainstDiscriminator(IOpenApiSchema schema, string? discriminatorName)
	{
		if (discriminatorName != null)
		{
			if (schema.Required == null || !schema.Required.Contains(discriminatorName))
			{
				IList<IOpenApiSchema>? oneOf = schema.OneOf;
				if (oneOf == null || oneOf.Count != 0)
				{
					return TraverseSchemaElements(discriminatorName, schema.OneOf);
				}
				IList<IOpenApiSchema>? anyOf = schema.AnyOf;
				if (anyOf == null || anyOf.Count != 0)
				{
					return TraverseSchemaElements(discriminatorName, schema.AnyOf);
				}
				IList<IOpenApiSchema>? allOf = schema.AllOf;
				if (allOf == null || allOf.Count != 0)
				{
					return TraverseSchemaElements(discriminatorName, schema.AllOf);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Traverses the schema elements and checks whether the schema contains the discriminator.
	/// </summary>
	/// <param name="discriminatorName">Adds support for polymorphism. The discriminator is an object name that is used to differentiate
	/// between other schemas which may satisfy the payload description.</param>
	/// <param name="childSchema">The child schema.</param>
	/// <returns></returns>
	public static bool TraverseSchemaElements(string discriminatorName, IList<IOpenApiSchema>? childSchema)
	{
		if (childSchema != null)
		{
			using (IEnumerator<IOpenApiSchema> enumerator = childSchema.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					IOpenApiSchema current = enumerator.Current;
					IDictionary<string, IOpenApiSchema>? properties = current.Properties;
					if (properties != null && !properties.ContainsKey(discriminatorName))
					{
						ISet<string>? required = current.Required;
						if (required != null && !required.Contains(discriminatorName))
						{
							return ValidateChildSchemaAgainstDiscriminator(current, discriminatorName);
						}
					}
					return true;
				}
			}
			return false;
		}
		return false;
	}
}
