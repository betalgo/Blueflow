using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class PropertyNode : ParseNode
{
	public string Name { get; set; }

	public ParseNode Value { get; set; }

	public PropertyNode(ParsingContext context, string name, YamlNode node)
		: base(context)
	{
		Name = name;
		Value = ParseNode.Create(context, node);
	}

	public void ParseField<T>(T parentInstance, IDictionary<string, Action<T, ParseNode>> fixedFields, IDictionary<Func<string, bool>, Action<T, string, ParseNode>> patternFields)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0058: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0122: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		if (fixedFields.TryGetValue(Name, out var value))
		{
			try
			{
				base.Context.StartObject(Name);
				value(parentInstance, Value);
				return;
			}
			catch (OpenApiReaderException ex)
			{
				base.Context.Diagnostic.Errors.Add(new OpenApiError((OpenApiException)(object)ex));
				return;
			}
			catch (OpenApiException ex2)
			{
				OpenApiException ex3 = ex2;
				ex3.Pointer = base.Context.GetLocation();
				base.Context.Diagnostic.Errors.Add(new OpenApiError(ex3));
				return;
			}
			finally
			{
				base.Context.EndObject();
			}
		}
		Action<T, string, ParseNode> action = (from p in patternFields
			where p.Key(Name)
			select p.Value).FirstOrDefault();
		if (action != null)
		{
			try
			{
				base.Context.StartObject(Name);
				action(parentInstance, Name, Value);
				return;
			}
			catch (OpenApiReaderException ex4)
			{
				base.Context.Diagnostic.Errors.Add(new OpenApiError((OpenApiException)(object)ex4));
				return;
			}
			catch (OpenApiException ex5)
			{
				OpenApiException ex6 = ex5;
				ex6.Pointer = base.Context.GetLocation();
				base.Context.Diagnostic.Errors.Add(new OpenApiError(ex6));
				return;
			}
			finally
			{
				base.Context.EndObject();
			}
		}
		base.Context.Diagnostic.Errors.Add(new OpenApiError("", Name + " is not a valid property at " + base.Context.GetLocation()));
	}

	public override IOpenApiAny CreateAny()
	{
		throw new NotImplementedException();
	}
}
