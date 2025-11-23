using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal class PropertyNode : ParseNode
{
	public string Name { get; set; }

	public ParseNode Value { get; set; }

	public PropertyNode(ParsingContext context, string name, JsonNode node)
		: base(context, node)
	{
		Name = name;
		Value = ParseNode.Create(context, node);
	}

	public void ParseField<T>(T parentInstance, Dictionary<string, Action<T, ParseNode, OpenApiDocument>> fixedFields, Dictionary<Func<string, bool>, Action<T, string, ParseNode, OpenApiDocument>> patternFields, OpenApiDocument hostDocument)
	{
		if (fixedFields.TryGetValue(Name, out Action<T, ParseNode, OpenApiDocument> value))
		{
			try
			{
				base.Context.StartObject(Name);
				value(parentInstance, Value, hostDocument);
				return;
			}
			catch (OpenApiReaderException exception)
			{
				base.Context.Diagnostic.Errors.Add(new OpenApiError(exception));
				return;
			}
			catch (OpenApiException ex)
			{
				ex.Pointer = base.Context.GetLocation();
				base.Context.Diagnostic.Errors.Add(new OpenApiError(ex));
				return;
			}
			finally
			{
				base.Context.EndObject();
			}
		}
		Action<T, string, ParseNode, OpenApiDocument> action = (from p in patternFields
			where p.Key(Name)
			select p.Value).FirstOrDefault();
		if (action != null)
		{
			try
			{
				base.Context.StartObject(Name);
				action(parentInstance, Name, Value, hostDocument);
				return;
			}
			catch (OpenApiReaderException exception2)
			{
				base.Context.Diagnostic.Errors.Add(new OpenApiError(exception2));
				return;
			}
			catch (OpenApiException ex2)
			{
				ex2.Pointer = base.Context.GetLocation();
				base.Context.Diagnostic.Errors.Add(new OpenApiError(ex2));
				return;
			}
			finally
			{
				base.Context.EndObject();
			}
		}
		OpenApiError item = new OpenApiError("", Name + " is not a valid property at " + base.Context.GetLocation());
		if ("$schema".Equals(Name, StringComparison.OrdinalIgnoreCase))
		{
			base.Context.Diagnostic.Warnings.Add(item);
		}
		else
		{
			base.Context.Diagnostic.Errors.Add(item);
		}
	}

	public override JsonNode CreateAny()
	{
		throw new NotImplementedException();
	}
}
