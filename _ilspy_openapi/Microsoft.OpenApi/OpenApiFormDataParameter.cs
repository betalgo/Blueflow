namespace Microsoft.OpenApi;

/// <summary>
/// Form parameter class to propagate information needed for <see cref="M:Microsoft.OpenApi.OpenApiParameter.SerializeAsV2(Microsoft.OpenApi.IOpenApiWriter)" />
/// </summary>
internal class OpenApiFormDataParameter : OpenApiParameter
{
	internal override void WriteInPropertyForV2(IOpenApiWriter writer)
	{
		writer.WriteProperty("in", "formData");
	}
}
