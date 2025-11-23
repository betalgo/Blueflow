using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class AnyMapFieldMapParameter<T, U>
{
	public Func<T, IDictionary<string, U>> PropertyMapGetter { get; }

	public Func<U, IOpenApiAny> PropertyGetter { get; }

	public Action<U, IOpenApiAny> PropertySetter { get; }

	public Func<T, OpenApiSchema> SchemaGetter { get; }

	public AnyMapFieldMapParameter(Func<T, IDictionary<string, U>> propertyMapGetter, Func<U, IOpenApiAny> propertyGetter, Action<U, IOpenApiAny> propertySetter, Func<T, OpenApiSchema> schemaGetter)
	{
		PropertyMapGetter = propertyMapGetter;
		PropertyGetter = propertyGetter;
		PropertySetter = propertySetter;
		SchemaGetter = schemaGetter;
	}
}
