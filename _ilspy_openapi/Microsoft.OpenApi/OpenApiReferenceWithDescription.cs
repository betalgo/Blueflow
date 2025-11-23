using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// OpenApiReferenceWithSummary is a reference to an OpenAPI component that includes a description.
/// </summary>
public class OpenApiReferenceWithDescription : BaseOpenApiReference, IOpenApiDescribedElement, IOpenApiElement
{
	/// <summary>
	/// A description which by default SHOULD override that of the referenced component.
	/// CommonMark syntax MAY be used for rich text representation.
	/// If the referenced object-type does not allow a description field, then this field has no effect.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiReferenceWithDescription()
	{
	}

	/// <summary>
	/// Initializes a copy instance of the <see cref="T:Microsoft.OpenApi.OpenApiReferenceWithDescription" /> object
	/// </summary>
	public OpenApiReferenceWithDescription(OpenApiReferenceWithDescription reference)
		: base(reference)
	{
		Utils.CheckArgumentNull(reference, "reference");
		Description = reference.Description;
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
		baseSerializer(writer);
		writer.WriteProperty("description", Description);
	}

	/// <inheritdoc />
	protected override void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
	{
		base.SetAdditional31MetadataFromMapNode(jsonObject);
		string propertyValueFromNode = BaseOpenApiReference.GetPropertyValueFromNode(jsonObject, "description");
		if (!string.IsNullOrEmpty(propertyValueFromNode))
		{
			Description = propertyValueFromNode;
		}
	}
}
