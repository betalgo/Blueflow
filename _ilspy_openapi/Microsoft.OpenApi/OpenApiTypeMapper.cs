using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Extension methods for <see cref="T:System.Type" />.
/// </summary>
public static class OpenApiTypeMapper
{
	private static readonly Dictionary<JsonSchemaType, string> allSchemaTypes = new Dictionary<JsonSchemaType, string>
	{
		{
			JsonSchemaType.Boolean,
			"boolean"
		},
		{
			JsonSchemaType.Integer,
			"integer"
		},
		{
			JsonSchemaType.Number,
			"number"
		},
		{
			JsonSchemaType.String,
			"string"
		},
		{
			JsonSchemaType.Object,
			"object"
		},
		{
			JsonSchemaType.Array,
			"array"
		},
		{
			JsonSchemaType.Null,
			"null"
		}
	};

	private static readonly Dictionary<Type, Func<OpenApiSchema>> _simpleTypeToOpenApiSchema = new Dictionary<Type, Func<OpenApiSchema>>
	{
		[typeof(bool)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Boolean
		},
		[typeof(byte)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Format = "byte"
		},
		[typeof(int)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Integer,
			Format = "int32"
		},
		[typeof(uint)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Integer,
			Format = "int32"
		},
		[typeof(long)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Integer,
			Format = "int64"
		},
		[typeof(ulong)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Integer,
			Format = "int64"
		},
		[typeof(float)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Number,
			Format = "float"
		},
		[typeof(double)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Number,
			Format = "double"
		},
		[typeof(decimal)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Number,
			Format = "double"
		},
		[typeof(DateTime)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Format = "date-time"
		},
		[typeof(DateTimeOffset)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Format = "date-time"
		},
		[typeof(Guid)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Format = "uuid"
		},
		[typeof(char)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.String
		},
		[typeof(bool?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Boolean)
		},
		[typeof(byte?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.String),
			Format = "byte"
		},
		[typeof(int?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Integer),
			Format = "int32"
		},
		[typeof(uint?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Integer),
			Format = "int32"
		},
		[typeof(long?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Integer),
			Format = "int64"
		},
		[typeof(ulong?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Integer),
			Format = "int64"
		},
		[typeof(float?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Number),
			Format = "float"
		},
		[typeof(double?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Number),
			Format = "double"
		},
		[typeof(decimal?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.Number),
			Format = "double"
		},
		[typeof(DateTime?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.String),
			Format = "date-time"
		},
		[typeof(DateTimeOffset?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.String),
			Format = "date-time"
		},
		[typeof(Guid?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.String),
			Format = "uuid"
		},
		[typeof(char?)] = () => new OpenApiSchema
		{
			Type = (JsonSchemaType.Null | JsonSchemaType.String)
		},
		[typeof(Uri)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.String,
			Format = "uri"
		},
		[typeof(string)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.String
		},
		[typeof(object)] = () => new OpenApiSchema
		{
			Type = JsonSchemaType.Object
		}
	};

	/// <summary>
	/// Maps a JsonSchema data type to an identifier.
	/// </summary>
	/// <param name="schemaType"></param>
	/// <returns></returns>
	public static string[]? ToIdentifiers(this JsonSchemaType? schemaType)
	{
		if (!schemaType.HasValue)
		{
			return null;
		}
		return schemaType.Value.ToIdentifiers();
	}

	/// <summary>
	/// Maps a JsonSchema data type to an identifier.
	/// </summary>
	/// <param name="schemaType"></param>
	/// <returns></returns>
	public static string[] ToIdentifiers(this JsonSchemaType schemaType)
	{
		return schemaType.ToIdentifiersInternal().ToArray();
	}

	private static IEnumerable<string> ToIdentifiersInternal(this JsonSchemaType schemaType)
	{
		return from kvp in allSchemaTypes
			where schemaType.HasFlag(kvp.Key)
			select kvp.Value;
	}

	/// <summary>
	/// Returns the first identifier from a string array.
	/// </summary>
	/// <param name="schemaType"></param>
	/// <returns></returns>
	internal static string ToFirstIdentifier(this JsonSchemaType schemaType)
	{
		return schemaType.ToIdentifiersInternal().First();
	}

	/// <summary>
	/// Returns a single identifier from an array with only one item.
	/// </summary>
	/// <param name="schemaType"></param>
	/// <returns></returns>
	internal static string ToSingleIdentifier(this JsonSchemaType schemaType)
	{
		return schemaType.ToIdentifiersInternal().Single();
	}

	/// <summary>
	/// Converts a schema type's identifier into the enum equivalent
	/// </summary>
	/// <param name="identifier"></param>
	/// <returns></returns>
	public static JsonSchemaType ToJsonSchemaType(this string identifier)
	{
		switch (identifier)
		{
		case "null":
			return JsonSchemaType.Null;
		case "boolean":
			return JsonSchemaType.Boolean;
		case "integer":
		case "int":
			return JsonSchemaType.Integer;
		case "decimal":
		case "double":
		case "number":
		case "float":
			return JsonSchemaType.Number;
		case "string":
			return JsonSchemaType.String;
		case "array":
			return JsonSchemaType.Array;
		case "object":
			return JsonSchemaType.Object;
		case "file":
			return JsonSchemaType.String;
		default:
			throw new OpenApiException($"Invalid schema type identifier: {identifier}");
		}
	}

	/// <summary>
	/// Converts a schema type's identifier into the enum equivalent
	/// </summary>
	/// <param name="identifier"></param>
	/// <returns></returns>
	public static JsonSchemaType? ToJsonSchemaType(this string[] identifier)
	{
		if (identifier == null)
		{
			return null;
		}
		JsonSchemaType jsonSchemaType = (JsonSchemaType)0;
		foreach (string identifier2 in identifier)
		{
			jsonSchemaType |= identifier2.ToJsonSchemaType();
		}
		return jsonSchemaType;
	}

	/// <summary>
	/// Maps a simple type to an OpenAPI data type and format.
	/// </summary>
	/// <param name="type">Simple type.</param>
	/// <remarks>
	/// All the following types from http://swagger.io/specification/#data-types-12 are supported.
	/// Other types including nullables and URL are also supported.
	/// Common Name      type    format      Comments
	/// ===========      ======= ======      =========================================
	/// integer          number int32       signed 32 bits
	/// long             number int64       signed 64 bits
	/// float            number  float
	/// double           number  double
	/// string           string  [empty]
	/// byte             string  byte        base64 encoded characters
	/// binary           string  binary      any sequence of octets
	/// boolean          boolean [empty]
	/// date             string  date        As defined by full-date - RFC3339
	/// dateTime         string  date-time   As defined by date-time - RFC3339
	/// password         string  password    Used to hint UIs the input needs to be obscured.
	/// If the type is not recognized as "simple", System.String will be returned.
	/// </remarks>
	public static OpenApiSchema MapTypeToOpenApiPrimitiveType(this Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (!_simpleTypeToOpenApiSchema.TryGetValue(type, out Func<OpenApiSchema> value))
		{
			return new OpenApiSchema
			{
				Type = JsonSchemaType.String
			};
		}
		return value();
	}

	/// <summary>
	/// Maps a JsonSchema data type and format to a simple type.
	/// </summary>
	/// <param name="schema">The OpenApi data type</param>
	/// <returns>The simple type</returns>
	/// <exception cref="T:System.ArgumentNullException"></exception>
	public static Type MapOpenApiPrimitiveTypeToSimpleType(this OpenApiSchema schema)
	{
		if (schema == null)
		{
			throw new ArgumentNullException("schema");
		}
		JsonSchemaType? type = schema.Type;
		string text = schema.Format?.ToLowerInvariant();
		switch (type)
		{
		case JsonSchemaType.Null | JsonSchemaType.Integer:
			switch (text)
			{
			case "int32":
				break;
			case "int64":
				goto IL_0268;
			case null:
				return typeof(long?);
			default:
				goto end_IL_002f;
			}
			goto IL_0258;
		case JsonSchemaType.Null | JsonSchemaType.Number:
			switch (text)
			{
			case "int32":
				break;
			case "int64":
				goto IL_0268;
			case "float":
				return typeof(float?);
			case "double":
				return typeof(double?);
			case null:
				return typeof(double?);
			case "decimal":
				return typeof(decimal?);
			default:
				goto end_IL_002f;
			}
			goto IL_0258;
		case JsonSchemaType.Null | JsonSchemaType.String:
			switch (text)
			{
			case "byte":
				return typeof(byte?);
			case "date-time":
				return typeof(DateTimeOffset?);
			case "uuid":
				return typeof(Guid?);
			case "char":
				return typeof(char?);
			}
			break;
		case JsonSchemaType.Null | JsonSchemaType.Boolean:
			if (text != null)
			{
				break;
			}
			return typeof(bool?);
		case JsonSchemaType.Boolean:
			if (text != null)
			{
				break;
			}
			return typeof(bool);
		case JsonSchemaType.Integer:
			switch (text)
			{
			case "int32":
				break;
			case "int64":
				goto IL_0338;
			case null:
				return typeof(long);
			default:
				goto end_IL_002f;
			}
			goto IL_0328;
		case JsonSchemaType.Number:
			switch (text)
			{
			case "int32":
				break;
			case "int64":
				goto IL_0338;
			case "float":
				return typeof(float);
			case "double":
				return typeof(double);
			case "decimal":
				return typeof(decimal);
			case null:
				return typeof(double);
			default:
				goto end_IL_002f;
			}
			goto IL_0328;
		case JsonSchemaType.String:
			switch (text)
			{
			case "byte":
				return typeof(byte);
			case "date-time":
				return typeof(DateTimeOffset);
			case "uuid":
				return typeof(Guid);
			case "char":
				return typeof(char);
			case null:
				return typeof(string);
			case "uri":
				return typeof(Uri);
			}
			break;
		case JsonSchemaType.Object:
			{
				if (text != null)
				{
					break;
				}
				return typeof(object);
			}
			IL_0338:
			return typeof(long);
			IL_0328:
			return typeof(int);
			IL_0268:
			return typeof(long?);
			IL_0258:
			return typeof(int?);
			end_IL_002f:
			break;
		}
		return typeof(string);
	}
}
