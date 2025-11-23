using System.IO;

namespace Microsoft.OpenApi;

/// <summary>
/// YAML writer.
/// </summary>
public class OpenApiYamlWriter : OpenApiWriterBase
{
	/// <summary>
	/// Allow rendering of multi-line strings using YAML | syntax
	/// </summary>
	public bool UseLiteralStyle { get; set; }

	/// <summary>
	/// Base Indentation Level.
	/// This denotes how many indentations are needed for the property in the base object.
	/// </summary>
	protected override int BaseIndentation => 0;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiYamlWriter" /> class.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	public OpenApiYamlWriter(TextWriter textWriter)
		: this(textWriter, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.OpenApiYamlWriter" /> class.
	/// </summary>
	/// <param name="textWriter">The text writer.</param>
	/// <param name="settings"></param>
	public OpenApiYamlWriter(TextWriter textWriter, OpenApiWriterSettings? settings)
		: base(textWriter, settings)
	{
	}

	/// <summary>
	/// Write YAML start object.
	/// </summary>
	public override void WriteStartObject()
	{
		Scope scope = CurrentScope();
		Scope scope2 = StartScope(ScopeType.Object);
		if (scope != null && scope.Type == ScopeType.Array)
		{
			scope2.IsInArray = true;
			base.Writer.WriteLine();
			WriteIndentation();
			base.Writer.Write("- ");
		}
		IncreaseIndentation();
	}

	/// <summary>
	/// Write YAML end object.
	/// </summary>
	public override void WriteEndObject()
	{
		Scope scope = EndScope(ScopeType.Object);
		DecreaseIndentation();
		Scope scope2 = CurrentScope();
		if (scope.ObjectCount == 0)
		{
			if (scope2 != null && scope2.Type == ScopeType.Object)
			{
				base.Writer.Write(" ");
			}
			base.Writer.Write("{ }");
		}
	}

	/// <summary>
	/// Write YAML start array.
	/// </summary>
	public override void WriteStartArray()
	{
		Scope scope = CurrentScope();
		Scope scope2 = StartScope(ScopeType.Array);
		if (scope != null && scope.Type == ScopeType.Array)
		{
			scope2.IsInArray = true;
			base.Writer.WriteLine();
			WriteIndentation();
			base.Writer.Write("- ");
		}
		IncreaseIndentation();
	}

	/// <summary>
	/// Write YAML end array.
	/// </summary>
	public override void WriteEndArray()
	{
		Scope scope = EndScope(ScopeType.Array);
		DecreaseIndentation();
		Scope scope2 = CurrentScope();
		if (scope.ObjectCount == 0)
		{
			if (scope2 != null && scope2.Type == ScopeType.Object)
			{
				base.Writer.Write(" ");
			}
			base.Writer.Write("[ ]");
		}
	}

	/// <summary>
	/// Write the property name and the delimiter.
	/// </summary>
	public override void WritePropertyName(string name)
	{
		VerifyCanWritePropertyName(name);
		Scope scope = CurrentScope();
		if (scope == null || scope.ObjectCount != 0)
		{
			base.Writer.WriteLine();
			WriteIndentation();
		}
		else if (!IsTopLevelScope() && !scope.IsInArray)
		{
			base.Writer.WriteLine();
			WriteIndentation();
		}
		name = name.GetYamlCompatibleString();
		base.Writer.Write(name);
		base.Writer.Write(":");
		scope.ObjectCount++;
	}

	/// <summary>
	/// Write string value.
	/// </summary>
	/// <param name="value">The string value.</param>
	public override void WriteValue(string value)
	{
		if (!UseLiteralStyle || value.IndexOfAny(new char[2] { '\n', '\r' }) == -1)
		{
			WriteValueSeparator();
			value = value.GetYamlCompatibleString();
			base.Writer.Write(value);
			return;
		}
		if (CurrentScope() != null)
		{
			WriteValueSeparator();
		}
		base.Writer.Write("|");
		WriteChompingIndicator(value);
		if (value[0] == ' ')
		{
			base.Writer.Write("  ".Length);
		}
		base.Writer.WriteLine();
		IncreaseIndentation();
		using (StringReader stringReader = new StringReader(value))
		{
			bool flag = true;
			while (true)
			{
				string text = stringReader.ReadLine();
				if (text != null)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						base.Writer.WriteLine();
					}
					if (text.Length > 0)
					{
						WriteIndentation();
					}
					base.Writer.Write(text);
					continue;
				}
				break;
			}
		}
		DecreaseIndentation();
	}

	private void WriteChompingIndicator(string? value)
	{
		int num = 0;
		int num2 = value.Length - 1;
		while (num2 >= 0 && num < 2)
		{
			int num3 = value.LastIndexOfAny(new char[2] { '\n', '\r' }, num2, 2);
			if (num3 == -1 || num3 != num2)
			{
				break;
			}
			num2 = ((value[num2] != '\r') ? ((num2 <= 0 || value[num2 - 1] != '\r') ? (num2 - 1) : (num2 - 2)) : (num2 - 1));
			num++;
		}
		switch (num)
		{
		case 0:
			base.Writer.Write("-");
			break;
		default:
			base.Writer.Write("+");
			break;
		case 1:
			break;
		}
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
	/// Write value separator.
	/// </summary>
	protected override void WriteValueSeparator()
	{
		if (IsArrayScope())
		{
			int objectCount = CurrentScope().ObjectCount;
			if (!IsTopLevelScope() || objectCount != 0)
			{
				base.Writer.WriteLine();
			}
			WriteIndentation();
			base.Writer.Write("- ");
			CurrentScope().ObjectCount++;
		}
		else
		{
			base.Writer.Write(" ");
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
}
