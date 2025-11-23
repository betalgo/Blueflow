using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal class AnyListFieldMapParameter<T>
{
	/// <summary>
	/// Function to retrieve the value of the property.
	/// </summary>
	public Func<T, List<JsonNode>> PropertyGetter { get; }

	/// <summary>
	/// Function to set the value of the property.
	/// </summary>
	public Action<T, List<JsonNode>> PropertySetter { get; }

	/// <summary>
	/// Function to get the schema to apply to the property.
	/// </summary>
	public Func<T, OpenApiSchema>? SchemaGetter { get; }

	/// <summary>
	/// Constructor
	/// </summary>
	public AnyListFieldMapParameter(Func<T, List<JsonNode>> propertyGetter, Action<T, List<JsonNode>> propertySetter, Func<T, OpenApiSchema>? SchemaGetter = null)
	{
		PropertyGetter = propertyGetter;
		PropertySetter = propertySetter;
		this.SchemaGetter = SchemaGetter;
	}
}
