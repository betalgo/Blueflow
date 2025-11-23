using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class AnyListFieldMapParameter<T>
{
	public Func<T, IList<IOpenApiAny>> PropertyGetter { get; }

	public Action<T, IList<IOpenApiAny>> PropertySetter { get; }

	public Func<T, OpenApiSchema> SchemaGetter { get; }

	public AnyListFieldMapParameter(Func<T, IList<IOpenApiAny>> propertyGetter, Action<T, IList<IOpenApiAny>> propertySetter, Func<T, OpenApiSchema> schemaGetter)
	{
		PropertyGetter = propertyGetter;
		PropertySetter = propertySetter;
		SchemaGetter = schemaGetter;
	}
}
