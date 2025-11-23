using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// The wrapper either for <see cref="T:System.Text.Json.Nodes.JsonNode" /> or <see cref="T:Microsoft.OpenApi.RuntimeExpression" />
/// </summary>
public class RuntimeExpressionAnyWrapper : IOpenApiElement
{
	private JsonNode? _any;

	private RuntimeExpression? _expression;

	/// <summary>
	/// Gets/Sets the <see cref="T:System.Text.Json.Nodes.JsonNode" />
	/// You must use the <see cref="M:Microsoft.OpenApi.JsonNullSentinel.IsJsonNullSentinel(System.Text.Json.Nodes.JsonNode)" /> method to check whether Default was assigned a null value in the document.
	/// Assign <see cref="P:Microsoft.OpenApi.JsonNullSentinel.JsonNull" /> to use get null as a serialized value.
	/// </summary>
	public JsonNode? Any
	{
		get
		{
			return _any;
		}
		set
		{
			_expression = null;
			_any = value;
		}
	}

	/// <summary>
	/// Gets/Set the <see cref="T:Microsoft.OpenApi.RuntimeExpression" />
	/// </summary>
	public RuntimeExpression? Expression
	{
		get
		{
			return _expression;
		}
		set
		{
			_any = null;
			_expression = value;
		}
	}

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public RuntimeExpressionAnyWrapper()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.RuntimeExpressionAnyWrapper" /> object
	/// </summary>
	public RuntimeExpressionAnyWrapper(RuntimeExpressionAnyWrapper runtimeExpressionAnyWrapper)
	{
		Any = JsonNodeCloneHelper.Clone(runtimeExpressionAnyWrapper?.Any);
		Expression = runtimeExpressionAnyWrapper?.Expression;
	}

	/// <summary>
	/// Write <see cref="T:Microsoft.OpenApi.RuntimeExpressionAnyWrapper" />
	/// </summary>
	public void WriteValue(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (_any != null)
		{
			writer.WriteAny(_any);
		}
		else if (_expression != null)
		{
			writer.WriteValue(_expression.Expression);
		}
	}
}
