namespace Microsoft.OpenApi;

/// <summary>
/// Defines the color and shape of a node in a Mermaid graph diagram
/// </summary>
internal class MermaidNodeStyle
{
	/// <summary>
	/// The CSS color name of the diagram element
	/// </summary>
	public string Color { get; }

	/// <summary>
	/// The shape of the diagram element
	/// </summary>
	public MermaidNodeShape Shape { get; }

	/// <summary>
	/// Create a style that defines the color and shape of a diagram element
	/// </summary>
	/// <param name="color"></param>
	/// <param name="shape"></param>
	internal MermaidNodeStyle(string color, MermaidNodeShape shape)
	{
		Color = color;
		Shape = shape;
	}
}
