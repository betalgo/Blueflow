using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines a non-default set of rules for validating examples in header, media type and parameter objects against the schema
/// </summary>
public static class OpenApiNonDefaultRules
{
	/// <summary>
	/// Validate the data matches with the given data type.
	/// </summary>
	public static ValidationRule<IOpenApiHeader> HeaderMismatchedDataType => new ValidationRule<IOpenApiHeader>("HeaderMismatchedDataType", delegate(IValidationContext context, IOpenApiHeader header)
	{
		ValidateMismatchedDataType(context, "HeaderMismatchedDataType", header.Example, header.Examples, header.Schema);
	});

	/// <summary>
	/// Validate the data matches with the given data type.
	/// </summary>
	public static ValidationRule<OpenApiMediaType> MediaTypeMismatchedDataType => new ValidationRule<OpenApiMediaType>("MediaTypeMismatchedDataType", delegate(IValidationContext context, OpenApiMediaType mediaType)
	{
		ValidateMismatchedDataType(context, "MediaTypeMismatchedDataType", mediaType.Example, mediaType.Examples, mediaType.Schema);
	});

	/// <summary>
	/// Validate the data matches with the given data type.
	/// </summary>
	public static ValidationRule<OpenApiParameter> ParameterMismatchedDataType => new ValidationRule<OpenApiParameter>("ParameterMismatchedDataType", delegate(IValidationContext context, OpenApiParameter parameter)
	{
		ValidateMismatchedDataType(context, "ParameterMismatchedDataType", parameter.Example, parameter.Examples, parameter.Schema);
	});

	/// <summary>
	/// Validate the data matches with the given data type.
	/// </summary>
	public static ValidationRule<IOpenApiSchema> SchemaMismatchedDataType => new ValidationRule<IOpenApiSchema>("SchemaMismatchedDataType", delegate(IValidationContext context, IOpenApiSchema schema)
	{
		if (schema.Default != null)
		{
			context.Enter("default");
			RuleHelpers.ValidateDataTypeMismatch(context, "SchemaMismatchedDataType", schema.Default, schema);
			context.Exit();
		}
		if (schema.Example != null)
		{
			context.Enter("example");
			RuleHelpers.ValidateDataTypeMismatch(context, "SchemaMismatchedDataType", schema.Example, schema);
			context.Exit();
		}
		if (schema.Enum != null)
		{
			context.Enter("enum");
			for (int i = 0; i < schema.Enum.Count; i++)
			{
				context.Enter(i.ToString());
				RuleHelpers.ValidateDataTypeMismatch(context, "SchemaMismatchedDataType", schema.Enum[i], schema);
				context.Exit();
			}
			context.Exit();
		}
	});

	private static void ValidateMismatchedDataType(IValidationContext context, string ruleName, JsonNode? example, IDictionary<string, IOpenApiExample>? examples, IOpenApiSchema? schema)
	{
		if (example != null)
		{
			context.Enter("example");
			RuleHelpers.ValidateDataTypeMismatch(context, ruleName, example, schema);
			context.Exit();
		}
		if (examples == null)
		{
			return;
		}
		context.Enter("examples");
		foreach (string item in examples.Keys.Where((string k) => examples[k] != null))
		{
			context.Enter(item);
			context.Enter("value");
			RuleHelpers.ValidateDataTypeMismatch(context, ruleName, examples[item]?.Value, schema);
			context.Exit();
			context.Exit();
		}
		context.Exit();
	}
}
