namespace Microsoft.OpenApi;

/// <summary>
/// Describes an element that has a description.
/// </summary>
public interface IOpenApiReadOnlyDescribedElement : IOpenApiElement
{
	/// <summary>
	/// Long description for the example.
	/// CommonMark syntax MAY be used for rich text representation.
	/// </summary>
	string? Description { get; }
}
