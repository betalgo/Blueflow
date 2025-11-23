using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.OpenApi;

/// <summary>
/// Visits OpenApi operations and parameters.
/// </summary>
public class OperationSearch : OpenApiVisitorBase
{
	private readonly Func<string, HttpMethod, OpenApiOperation, bool> _predicate;

	private readonly List<SearchResult> _searchResults = new List<SearchResult>();

	/// <summary>
	/// A list of operations from the operation search.
	/// </summary>
	public IList<SearchResult> SearchResults => _searchResults;

	/// <summary>
	/// The OperationSearch constructor.
	/// </summary>
	/// <param name="predicate">A predicate function.</param>
	public OperationSearch(Func<string, HttpMethod, OpenApiOperation, bool> predicate)
	{
		_predicate = predicate ?? throw new ArgumentNullException("predicate");
	}

	/// <inheritdoc />
	public override void Visit(IOpenApiPathItem pathItem)
	{
		if (pathItem.Operations == null)
		{
			return;
		}
		foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in pathItem.Operations)
		{
			OpenApiOperation value = operation.Value;
			HttpMethod key = operation.Key;
			if (base.CurrentKeys.Path != null && _predicate(base.CurrentKeys.Path, key, value))
			{
				_searchResults.Add(new SearchResult
				{
					Operation = value,
					Parameters = pathItem.Parameters,
					CurrentKeys = CopyCurrentKeys(base.CurrentKeys, key)
				});
			}
		}
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiParameter" />.
	/// </summary>
	/// <param name="parameters">The target list of <see cref="T:Microsoft.OpenApi.OpenApiParameter" />.</param>
	public override void Visit(IList<IOpenApiParameter> parameters)
	{
		foreach (OpenApiParameter item in from x in parameters.OfType<OpenApiParameter>()
			where x.Style == ParameterStyle.Form
			select x)
		{
			item.Explode = false;
		}
		base.Visit(parameters);
	}

	private static CurrentKeys CopyCurrentKeys(CurrentKeys currentKeys, HttpMethod operationType)
	{
		return new CurrentKeys
		{
			Path = currentKeys.Path,
			Operation = operationType
		};
	}
}
