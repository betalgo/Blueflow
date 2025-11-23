using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class AnyFieldMapParameter<T>
{
	public Func<T, IOpenApiAny> PropertyGetter { get; }

	public Action<T, IOpenApiAny> PropertySetter { get; }

	public Func<T, OpenApiSchema> SchemaGetter { get; }

	public AnyFieldMapParameter(Func<T, IOpenApiAny> propertyGetter, Action<T, IOpenApiAny> propertySetter, Func<T, OpenApiSchema> schemaGetter)
	{
		PropertyGetter = propertyGetter;
		PropertySetter = propertySetter;
		SchemaGetter = schemaGetter;
	}
}
