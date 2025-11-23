using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Body parameter class to propagate information needed for <see cref="M:Microsoft.OpenApi.OpenApiParameter.SerializeAsV2(Microsoft.OpenApi.IOpenApiWriter)" />
/// </summary>
internal class OpenApiBodyParameter : OpenApiParameter
{
	internal override void WriteRequestBodySchemaForV2(IOpenApiWriter writer, Dictionary<string, IOpenApiExtension>? extensionsClone)
	{
		writer.WriteOptionalObject("schema", base.Schema, delegate(IOpenApiWriter w, IOpenApiSchema s)
		{
			s.SerializeAsV2(w);
		});
	}

	internal override void WriteInPropertyForV2(IOpenApiWriter writer)
	{
		writer.WriteProperty("in", "body");
	}
}
