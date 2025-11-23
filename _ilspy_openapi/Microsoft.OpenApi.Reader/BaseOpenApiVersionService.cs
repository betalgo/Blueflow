using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Reader;

internal abstract class BaseOpenApiVersionService : IOpenApiVersionService
{
	public OpenApiDiagnostic Diagnostic { get; }

	internal abstract Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> Loaders { get; }

	protected BaseOpenApiVersionService(OpenApiDiagnostic diagnostic)
	{
		Diagnostic = diagnostic;
	}

	public abstract OpenApiDocument LoadDocument(RootNode rootNode, Uri location);

	public T? LoadElement<T>(ParseNode node, OpenApiDocument doc) where T : IOpenApiElement
	{
		if (Loaders.TryGetValue(typeof(T), out Func<ParseNode, OpenApiDocument, object> value))
		{
			object obj = value(node, doc);
			if (obj is T)
			{
				return (T)obj;
			}
		}
		return default(T);
	}

	public virtual string? GetReferenceScalarValues(MapNode mapNode, string scalarValue)
	{
		if (mapNode.Any((PropertyNode x) => !"$ref".Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
		{
			ValueNode valueNode = (from x in mapNode
				where x.Name.Equals(scalarValue)
				select x.Value).OfType<ValueNode>().FirstOrDefault();
			if (valueNode != null)
			{
				return valueNode.GetScalarValue();
			}
		}
		return null;
	}
}
