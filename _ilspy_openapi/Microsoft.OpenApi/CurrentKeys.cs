using System.Net.Http;

namespace Microsoft.OpenApi;

/// <summary>
/// Object containing contextual information based on where the walker is currently referencing in an OpenApiDocument
/// </summary>
public class CurrentKeys
{
	/// <summary>
	/// Current Path key
	/// </summary>
	public string? Path { get; internal set; }

	/// <summary>
	/// Current Operation Type
	/// </summary>
	public HttpMethod? Operation { get; internal set; }

	/// <summary>
	/// Current Response Status Code
	/// </summary>
	public string? Response { get; internal set; }

	/// <summary>
	/// Current Content Media Type
	/// </summary>
	public string? Content { get; internal set; }

	/// <summary>
	/// Current Callback Key
	/// </summary>
	public string? Callback { get; internal set; }

	/// <summary>
	/// Current Link Key
	/// </summary>
	public string? Link { get; internal set; }

	/// <summary>
	/// Current Header Key
	/// </summary>
	public string? Header { get; internal set; }

	/// <summary>
	/// Current Encoding Key
	/// </summary>
	public string? Encoding { get; internal set; }

	/// <summary>
	/// Current Example Key
	/// </summary>
	public string? Example { get; internal set; }

	/// <summary>
	/// Current Extension Key
	/// </summary>
	public string? Extension { get; internal set; }

	/// <summary>
	/// Current ServerVariable
	/// </summary>
	public string? ServerVariable { get; internal set; }
}
