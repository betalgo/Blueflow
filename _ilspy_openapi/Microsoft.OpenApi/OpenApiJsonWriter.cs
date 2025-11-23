using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.OpenApi;

/// <summary>
/// JSON Writer.
/// </summary>
public class OpenApiJsonWriter : OpenApiWriterBase
{
	/// <summary>
	/// Indicates whether or not the produced document will be written in a compact or pretty fashion.
	/// </summary>
	private readonly bool _produceTerseOutput;

	/// <summary>
	/// Base Indentation Level.
	/// This denotes how many indentations are needed for the property in the base object.
	/// </summary>
	protected override int BaseIndentation => 1;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiJsonWriter" /> class.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	public OpenApiJsonWriter(TextWriter textWriter)
		: this(textWriter, (OpenApiWriterSettings?)null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiJsonWriter" /> class.
	/// </summary>
	/// <param name="settings">Settings for controlling how the OpenAPI document will be written out.</param>
	/// <param name="textWriter">The text writer.</param>
	public OpenApiJsonWriter(TextWriter textWriter, OpenApiWriterSettings? settings)
		: base(textWriter, settings ?? new OpenApiJsonWriterSettings())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiJsonWriter" /> class.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	/// <param name="settings">Settings for controlling how the OpenAPI document will be written out.</param>
	public OpenApiJsonWriter(TextWriter textWriter, OpenApiJsonWriterSettings settings)
		: base(textWriter, settings)
	{
		_produceTerseOutput = settings.Terse;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiJsonWriter" /> class.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	/// <param name="settings">Settings for controlling how the OpenAPI document will be written out.</param>
	/// <param name="terseOutput"> Setting for allowing the JSON emitted to be in terse format.</param>
	[Obsolete("Use OpenApiJsonWriter(TextWriter textWriter, OpenApiJsonWriterSettings settings) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public OpenApiJsonWriter(TextWriter textWriter, OpenApiWriterSettings? settings, bool terseOutput)
		: base(textWriter, settings)
	{
		_produceTerseOutput = terseOutput;
	}

	/// <summary>
	/// Write JSON start object.
	/// </summary>
	public override void WriteStartObject()
	{
		Scope scope = CurrentScope();
		Scope scope2 = StartScope(ScopeType.Object);
		if (scope != null && scope.Type == ScopeType.Array)
		{
			scope2.IsInArray = true;
			if (scope.ObjectCount != 1)
			{
				base.Writer.Write(",");
			}
			WriteLine();
			WriteIndentation();
		}
		base.Writer.Write("{");
		IncreaseIndentation();
	}

	/// <summary>
	/// Write JSON end object.
	/// </summary>
	public override void WriteEndObject()
	{
		if (EndScope(ScopeType.Object).ObjectCount != 0)
		{
			WriteLine();
			DecreaseIndentation();
			WriteIndentation();
		}
		else
		{
			if (!_produceTerseOutput)
			{
				base.Writer.Write(" ");
			}
			DecreaseIndentation();
		}
		base.Writer.Write("}");
	}

	/// <summary>
	/// Write JSON start array.
	/// </summary>
	public override void WriteStartArray()
	{
		Scope scope = CurrentScope();
		Scope scope2 = StartScope(ScopeType.Array);
		if (scope != null && scope.Type == ScopeType.Array)
		{
			scope2.IsInArray = true;
			if (scope.ObjectCount != 1)
			{
				base.Writer.Write(",");
			}
			WriteLine();
			WriteIndentation();
		}
		base.Writer.Write("[");
		IncreaseIndentation();
	}

	/// <summary>
	/// Write JSON end array.
	/// </summary>
	public override void WriteEndArray()
	{
		if (EndScope(ScopeType.Array).ObjectCount != 0)
		{
			WriteLine();
			DecreaseIndentation();
			WriteIndentation();
		}
		else
		{
			base.Writer.Write(" ");
			DecreaseIndentation();
		}
		base.Writer.Write("]");
	}

	/// <summary>
	/// Write property name.
	/// </summary>
	/// <param name="name">The property name.</param>
	/// public override void WritePropertyName(string name)
	public override void WritePropertyName(string name)
	{
		VerifyCanWritePropertyName(name);
		Scope? scope = CurrentScope();
		if (scope == null || scope.ObjectCount != 0)
		{
			base.Writer.Write(",");
		}
		WriteLine();
		scope.ObjectCount++;
		WriteIndentation();
		name = name.GetJsonCompatibleString();
		base.Writer.Write(name);
		base.Writer.Write(":");
		if (!_produceTerseOutput)
		{
			base.Writer.Write(" ");
		}
	}

	/// <summary>
	/// Write string value.
	/// </summary>
	/// <param name="value">The string value.</param>
	public override void WriteValue(string value)
	{
		WriteValueSeparator();
		value = value.GetJsonCompatibleString();
		base.Writer.Write(value);
	}

	/// <summary>
	/// Write null value.
	/// </summary>
	public override void WriteNull()
	{
		WriteValueSeparator();
		base.Writer.Write("null");
	}

	/// <summary>
	/// Writes a separator of a value if it's needed for the next value to be written.
	/// </summary>
	protected override void WriteValueSeparator()
	{
		if (Scopes.Count == 0)
		{
			return;
		}
		Scope scope = Scopes.Peek();
		if (scope.Type == ScopeType.Array)
		{
			if (scope.ObjectCount != 0)
			{
				base.Writer.Write(",");
			}
			WriteLine();
			WriteIndentation();
			scope.ObjectCount++;
		}
	}

	/// <summary>
	/// Writes the content raw value.
	/// </summary>
	public override void WriteRaw(string value)
	{
		WriteValueSeparator();
		base.Writer.Write(value);
	}

	/// <summary>
	/// Write the indentation.
	/// </summary>
	public override void WriteIndentation()
	{
		if (!_produceTerseOutput)
		{
			base.WriteIndentation();
		}
	}

	/// <summary>
	/// Writes a line terminator to the text string or stream.
	/// </summary>
	private void WriteLine()
	{
		if (!_produceTerseOutput)
		{
			base.Writer.WriteLine();
		}
	}
}
