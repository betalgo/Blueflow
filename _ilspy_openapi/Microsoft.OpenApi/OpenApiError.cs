namespace Microsoft.OpenApi;

/// <summary>
/// Error related to the Open API Document.
/// </summary>
public class OpenApiError
{
	/// <summary>
	/// Message explaining the error.
	/// </summary>
	public string Message { get; set; }

	/// <summary>
	/// Pointer to the location of the error.
	/// </summary>
	public string? Pointer { get; set; }

	/// <summary>
	/// Initializes the <see cref="T:Microsoft.OpenApi.OpenApiError" /> class using the message and pointer from the given exception.
	/// </summary>
	public OpenApiError(OpenApiException exception)
		: this(exception.Pointer, exception.Message)
	{
	}

	/// <summary>
	/// Initializes the <see cref="T:Microsoft.OpenApi.OpenApiError" /> class.
	/// </summary>
	public OpenApiError(string? pointer, string message)
	{
		Pointer = pointer;
		Message = message;
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiError" /> object
	/// </summary>
	public OpenApiError(OpenApiError error)
	{
		Pointer = error.Pointer;
		Message = error.Message;
	}

	/// <summary>
	/// Gets the string representation of <see cref="T:Microsoft.OpenApi.OpenApiError" />.
	/// </summary>
	public override string ToString()
	{
		return Message + ((!string.IsNullOrEmpty(Pointer)) ? (" [" + Pointer + "]") : "");
	}
}
