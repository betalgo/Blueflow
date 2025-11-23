using System;

namespace Microsoft.OpenApi;

/// <summary>
/// Represents the type of a JSON schema.
/// </summary>
[Flags]
public enum JsonSchemaType
{
	/// <summary>
	/// Represents a null type.
	/// </summary>
	Null = 1,
	/// <summary>
	/// Represents a boolean type.
	/// </summary>
	Boolean = 2,
	/// <summary>
	/// Represents an integer type.
	/// </summary>
	Integer = 4,
	/// <summary>
	/// Represents a number type.
	/// </summary>
	Number = 8,
	/// <summary>
	/// Represents a string type.
	/// </summary>
	String = 0x10,
	/// <summary>
	/// Represents an object type.
	/// </summary>
	Object = 0x20,
	/// <summary>
	/// Represents an array type.
	/// </summary>
	Array = 0x40
}
