using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal class AnyFieldMapParameter<T>
{
	/// <summary>
	/// Function to retrieve the value of the property.
	/// </summary>
	public Func<T, JsonNode?> PropertyGetter { get; }

	/// <summary>
	/// Function to set the value of the property.
	/// </summary>
	public Action<T, JsonNode?> PropertySetter { get; }

	/// <summary>
	/// Function to get the schema to apply to the property.
	/// </summary>
	public Func<T, IOpenApiSchema?>? SchemaGetter { get; }

	/// <summary>
	/// Constructor.
	/// </summary>
	public AnyFieldMapParameter(Func<T, JsonNode?> propertyGetter, Action<T, JsonNode?> propertySetter, Func<T, IOpenApiSchema?>? SchemaGetter = null)
	{
		PropertyGetter = propertyGetter;
		PropertySetter = propertySetter;
		this.SchemaGetter = SchemaGetter;
	}
}
