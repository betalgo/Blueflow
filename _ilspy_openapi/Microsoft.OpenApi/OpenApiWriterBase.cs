using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi;

/// <summary>
/// Base class for Open API writer.
/// </summary>
public abstract class OpenApiWriterBase : IOpenApiWriter
{
	/// <summary>
	/// The indentation string to prepend to each line for each indentation level.
	/// </summary>
	protected const string IndentationString = "  ";

	/// <summary>
	/// Scope of the Open API element - object, array, property.
	/// </summary>
	protected readonly Stack<Scope> Scopes;

	/// <summary>
	/// Number which specifies the level of indentation.
	/// </summary>
	private int _indentLevel;

	/// <summary>
	/// Settings for controlling how the OpenAPI document will be written out.
	/// </summary>
	public OpenApiWriterSettings Settings { get; set; }

	/// <summary>
	/// Base Indentation Level.
	/// This denotes how many indentations are needed for the property in the base object.
	/// </summary>
	protected abstract int BaseIndentation { get; }

	/// <summary>
	/// The text writer.
	/// </summary>
	protected TextWriter Writer { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiWriterBase" /> class.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	protected OpenApiWriterBase(TextWriter textWriter)
		: this(textWriter, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiWriterBase" /> class.
	/// </summary>
	/// <param name="textWriter"></param>
	/// <param name="settings"></param>
	protected OpenApiWriterBase(TextWriter textWriter, OpenApiWriterSettings? settings)
	{
		Writer = textWriter;
		Writer.NewLine = "\n";
		Scopes = new Stack<Scope>();
		if (settings == null)
		{
			settings = new OpenApiWriterSettings();
		}
		Settings = settings;
	}

	/// <summary>
	/// Write start object.
	/// </summary>
	public abstract void WriteStartObject();

	/// <summary>
	/// Write end object.
	/// </summary>
	public abstract void WriteEndObject();

	/// <summary>
	/// Write start array.
	/// </summary>
	public abstract void WriteStartArray();

	/// <summary>
	/// Write end array.
	/// </summary>
	public abstract void WriteEndArray();

	/// <summary>
	/// Write the start property.
	/// </summary>
	public abstract void WritePropertyName(string name);

	/// <summary>
	/// Writes a separator of a value if it's needed for the next value to be written.
	/// </summary>
	protected abstract void WriteValueSeparator();

	/// <summary>
	/// Write null value.
	/// </summary>
	public abstract void WriteNull();

	/// <summary>
	/// Write content raw value.
	/// </summary>
	public abstract void WriteRaw(string value);

	/// <inheritdoc />
	public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return Writer.FlushAsync();
	}

	/// <summary>
	/// Write string value.
	/// </summary>
	/// <param name="value">The string value.</param>
	public abstract void WriteValue(string value);

	/// <summary>
	/// Write float value.
	/// </summary>
	/// <param name="value">The float value.</param>
	public virtual void WriteValue(float value)
	{
		WriteValueSeparator();
		Writer.Write(value);
	}

	/// <summary>
	/// Write double value.
	/// </summary>
	/// <param name="value">The double value.</param>
	public virtual void WriteValue(double value)
	{
		WriteValueSeparator();
		Writer.Write(value);
	}

	/// <summary>
	/// Write decimal value.
	/// </summary>
	/// <param name="value">The decimal value.</param>
	public virtual void WriteValue(decimal value)
	{
		WriteValueSeparator();
		Writer.Write(value);
	}

	/// <summary>
	/// Write integer value.
	/// </summary>
	/// <param name="value">The integer value.</param>
	public virtual void WriteValue(int value)
	{
		WriteValueSeparator();
		Writer.Write(value);
	}

	/// <summary>
	/// Write long value.
	/// </summary>
	/// <param name="value">The long value.</param>
	public virtual void WriteValue(long value)
	{
		WriteValueSeparator();
		Writer.Write(value);
	}

	/// <summary>
	/// Write DateTime value.
	/// </summary>
	/// <param name="value">The DateTime value.</param>
	public virtual void WriteValue(DateTime value)
	{
		WriteValue(value.ToString("o", CultureInfo.InvariantCulture));
	}

	/// <summary>
	/// Write DateTimeOffset value.
	/// </summary>
	/// <param name="value">The DateTimeOffset value.</param>
	public virtual void WriteValue(DateTimeOffset value)
	{
		WriteValue(value.ToString("o", CultureInfo.InvariantCulture));
	}

	/// <summary>
	/// Write boolean value.
	/// </summary>
	/// <param name="value">The boolean value.</param>
	public virtual void WriteValue(bool value)
	{
		WriteValueSeparator();
		Writer.Write(value.ToString().ToLower());
	}

	/// <summary>
	/// Writes an enumerable collection as an array
	/// </summary>
	/// <param name="collection">The enumerable collection to write.</param>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	public virtual void WriteEnumerable<T>(IEnumerable<T> collection)
	{
		WriteStartArray();
		foreach (T item in collection)
		{
			WriteValue(item);
		}
		WriteEndArray();
	}

	/// <summary>
	/// Write object value.
	/// </summary>
	/// <param name="value">The object value.</param>
	public virtual void WriteValue(object? value)
	{
		if (value == null)
		{
			WriteNull();
			return;
		}
		if (value is string value2)
		{
			WriteValue(value2);
			return;
		}
		if (value is int value3)
		{
			WriteValue(value3);
			return;
		}
		if (value is uint num)
		{
			WriteValue(num);
			return;
		}
		if (value is long value4)
		{
			WriteValue(value4);
			return;
		}
		if (value is bool value5)
		{
			WriteValue(value5);
			return;
		}
		if (value is float value6)
		{
			WriteValue(value6);
			return;
		}
		if (value is double value7)
		{
			WriteValue(value7);
			return;
		}
		if (value is decimal value8)
		{
			WriteValue(value8);
			return;
		}
		if (value is DateTime value9)
		{
			WriteValue(value9);
			return;
		}
		if (value is DateTimeOffset value10)
		{
			WriteValue(value10);
			return;
		}
		if (value is IEnumerable<object> collection)
		{
			WriteEnumerable(collection);
			return;
		}
		throw new OpenApiWriterException(string.Format(SRResource.OpenApiUnsupportedValueType, value.GetType().FullName));
	}

	/// <summary>
	/// Increases the level of indentation applied to the output.
	/// </summary>
	public virtual void IncreaseIndentation()
	{
		_indentLevel++;
	}

	/// <summary>
	/// Decreases the level of indentation applied to the output.
	/// </summary>
	public virtual void DecreaseIndentation()
	{
		if (_indentLevel == 0)
		{
			throw new OpenApiWriterException(SRResource.IndentationLevelInvalid);
		}
		if (_indentLevel < 1)
		{
			_indentLevel = 0;
		}
		else
		{
			_indentLevel--;
		}
	}

	/// <summary>
	/// Write the indentation.
	/// </summary>
	public virtual void WriteIndentation()
	{
		for (int i = 0; i < BaseIndentation + _indentLevel - 1; i++)
		{
			Writer.Write("  ");
		}
	}

	/// <summary>
	/// Get current scope.
	/// </summary>
	/// <returns></returns>
	protected Scope? CurrentScope()
	{
		if (Scopes.Count != 0)
		{
			return Scopes.Peek();
		}
		return null;
	}

	/// <summary>
	/// Start the scope given the scope type.
	/// </summary>
	/// <param name="type">The scope type to start.</param>
	protected Scope StartScope(ScopeType type)
	{
		if (Scopes.Count != 0)
		{
			Scopes.Peek().ObjectCount++;
		}
		Scope scope = new Scope(type);
		Scopes.Push(scope);
		return scope;
	}

	/// <summary>
	/// End the scope of the given scope type.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	protected Scope EndScope(ScopeType type)
	{
		if (Scopes.Count == 0)
		{
			throw new OpenApiWriterException(SRResource.ScopeMustBePresentToEnd);
		}
		if (Scopes.Peek().Type != type)
		{
			throw new OpenApiWriterException(string.Format(SRResource.ScopeToEndHasIncorrectType, type, Scopes.Peek().Type));
		}
		return Scopes.Pop();
	}

	/// <summary>
	/// Whether the current scope is the top level (outermost) scope.
	/// </summary>
	protected bool IsTopLevelScope()
	{
		return Scopes.Count == 1;
	}

	/// <summary>
	/// Whether the current scope is an object scope.
	/// </summary>
	protected bool IsObjectScope()
	{
		return IsScopeType(ScopeType.Object);
	}

	/// <summary>
	/// Whether the current scope is an array scope.
	/// </summary>
	/// <returns></returns>
	protected bool IsArrayScope()
	{
		return IsScopeType(ScopeType.Array);
	}

	private bool IsScopeType(ScopeType type)
	{
		if (Scopes.Count == 0)
		{
			return false;
		}
		return Scopes.Peek().Type == type;
	}

	/// <summary>
	/// Verifies whether a property name can be written based on whether
	/// the property name is a valid string and whether the current scope is an object scope.
	/// </summary>
	/// <param name="name">property name</param>
	protected void VerifyCanWritePropertyName(string name)
	{
		Utils.CheckArgumentNull(name, "name");
		if (Scopes.Count == 0)
		{
			throw new OpenApiWriterException(string.Format(SRResource.ActiveScopeNeededForPropertyNameWriting, name));
		}
		if (Scopes.Peek().Type != ScopeType.Object)
		{
			throw new OpenApiWriterException(string.Format(SRResource.ObjectScopeNeededForPropertyNameWriting, name));
		}
	}

	/// <inheritdoc />
	public static void WriteV2Examples(IOpenApiWriter writer, OpenApiExample example, OpenApiSpecVersion version)
	{
		writer.WriteStartObject();
		writer.WriteProperty("summary", example.Summary);
		writer.WriteProperty("description", example.Description);
		writer.WriteOptionalObject("value", example.Value, delegate(IOpenApiWriter w, JsonNode v)
		{
			w.WriteAny(v);
		});
		writer.WriteProperty("externalValue", example.ExternalValue);
		writer.WriteExtensions(example.Extensions, version);
		writer.WriteEndObject();
	}
}
