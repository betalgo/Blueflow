using System;

namespace Microsoft.OpenApi;

/// <summary>
/// Represents the Open Api Data type metadata attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DisplayAttribute : Attribute
{
	/// <summary>
	/// The display Name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.DisplayAttribute" /> class.
	/// </summary>
	/// <param name="name">The display name.</param>
	public DisplayAttribute(string name)
	{
		Name = Utils.CheckArgumentNullOrEmpty(name, "name");
	}
}
