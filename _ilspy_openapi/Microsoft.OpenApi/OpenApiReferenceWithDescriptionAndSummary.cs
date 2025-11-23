using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// OpenApiReferenceWithSummary is a reference to an OpenAPI component that includes a summary.
/// </summary>
public class OpenApiReferenceWithDescriptionAndSummary : OpenApiReferenceWithDescription, IOpenApiSummarizedElement, IOpenApiElement
{
	/// <summary>
	/// A short summary which by default SHOULD override that of the referenced component.
	/// If the referenced object-type does not allow a summary field, then this field has no effect.
	/// </summary>
	public string? Summary { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiReferenceWithDescriptionAndSummary()
	{
	}

	/// <summary>
	/// Initializes a copy instance of the <see cref="T:Microsoft.OpenApi.OpenApiReferenceWithDescriptionAndSummary" /> object
	/// </summary>
	public OpenApiReferenceWithDescriptionAndSummary(OpenApiReferenceWithDescriptionAndSummary reference)
		: base(reference)
	{
		Utils.CheckArgumentNull(reference, "reference");
		Summary = reference.Summary;
	}

	/// <inheritdoc />
	protected override void SerializeAdditionalV31Properties(IOpenApiWriter writer)
	{
		SerializeAdditionalV3XProperties(writer, base.SerializeAdditionalV31Properties);
	}

	/// <inheritdoc />
	protected override void SerializeAdditionalV32Properties(IOpenApiWriter writer)
	{
		SerializeAdditionalV3XProperties(writer, base.SerializeAdditionalV32Properties);
	}

	private void SerializeAdditionalV3XProperties(IOpenApiWriter writer, Action<IOpenApiWriter> baseSerializer)
	{
		writer.WriteProperty("summary", Summary);
		baseSerializer(writer);
	}

	/// <inheritdoc />
	protected override void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
	{
		base.SetAdditional31MetadataFromMapNode(jsonObject);
		string propertyValueFromNode = BaseOpenApiReference.GetPropertyValueFromNode(jsonObject, "summary");
		if (!string.IsNullOrEmpty(propertyValueFromNode))
		{
			Summary = propertyValueFromNode;
		}
	}
}
