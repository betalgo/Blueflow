namespace Microsoft.OpenApi;

/// <summary>
/// Configuration settings to control how OpenAPI documents are written
/// </summary>
public class OpenApiWriterSettings
{
	internal LoopDetector LoopDetector { get; } = new LoopDetector();

	/// <summary>
	/// Indicates if local references should be rendered as an inline object
	/// </summary>
	public bool InlineLocalReferences { get; set; }

	/// <summary>
	/// Indicates if external references should be rendered as an inline object
	/// </summary>
	public bool InlineExternalReferences { get; set; }

	internal bool ShouldInlineReference(BaseOpenApiReference reference)
	{
		if (!reference.IsLocal || !InlineLocalReferences)
		{
			if (reference.IsExternal)
			{
				return InlineExternalReferences;
			}
			return false;
		}
		return true;
	}
}
