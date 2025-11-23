using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi;

/// <summary>
/// A service that slices an OpenApiDocument into a subset document
/// </summary>
public static class OpenApiFilterService
{
	/// <summary>
	/// Create predicate function based on passed query parameters
	/// </summary>
	/// <param name="operationIds">Comma delimited list of operationIds or * for all operations.</param>
	/// <param name="tags">Comma delimited list of tags or a single regex.</param>
	/// <param name="requestUrls">A dictionary of requests from a postman collection.</param>
	/// <param name="source">The input OpenAPI document.</param>
	/// <returns>A predicate.</returns>
	public static Func<string, HttpMethod, OpenApiOperation, bool> CreatePredicate(string? operationIds = null, string? tags = null, Dictionary<string, List<string>>? requestUrls = null, OpenApiDocument? source = null)
	{
		ValidateFilters(requestUrls, operationIds, tags);
		if (operationIds != null)
		{
			return GetOperationIdsPredicate(operationIds);
		}
		if (tags != null)
		{
			return GetTagsPredicate(tags);
		}
		if (requestUrls != null && source != null)
		{
			return GetRequestUrlsPredicate(requestUrls, source);
		}
		throw new InvalidOperationException("Either operationId(s),tag(s) or Postman collection need to be specified.");
	}

	/// <summary>
	/// Create partial OpenAPI document based on the provided predicate.
	/// </summary>
	/// <param name="source">The target <see cref="T:Microsoft.OpenApi.OpenApiDocument" />.</param>
	/// <param name="predicate">A predicate function.</param>
	/// <returns>A partial OpenAPI document.</returns>
	public static OpenApiDocument CreateFilteredDocument(OpenApiDocument source, Func<string, HttpMethod, OpenApiOperation, bool> predicate)
	{
		OpenApiComponents components = ((source.Components == null) ? null : new OpenApiComponents
		{
			SecuritySchemes = source.Components.SecuritySchemes
		});
		OpenApiDocument openApiDocument = new OpenApiDocument
		{
			Info = new OpenApiInfo
			{
				Title = source.Info.Title + " - Subset",
				Description = source.Info.Description,
				TermsOfService = source.Info.TermsOfService,
				Contact = source.Info.Contact,
				License = source.Info.License,
				Version = source.Info.Version,
				Extensions = source.Info.Extensions
			},
			Components = components,
			Security = source.Security,
			Servers = source.Servers
		};
		foreach (SearchResult item in FindOperations(source, predicate))
		{
			IOpenApiPathItem value = null;
			string text = item.CurrentKeys?.Path;
			if (openApiDocument.Paths == null)
			{
				openApiDocument.Paths = new OpenApiPaths();
				value = new OpenApiPathItem();
				if (text != null)
				{
					openApiDocument.Paths.Add(text, value);
				}
			}
			else if (text != null && !openApiDocument.Paths.TryGetValue(text, out value))
			{
				value = new OpenApiPathItem();
				openApiDocument.Paths.Add(text, value);
			}
			if (!(item.CurrentKeys?.Operation != null) || item.Operation == null || !(value is OpenApiPathItem openApiPathItem))
			{
				continue;
			}
			OpenApiPathItem openApiPathItem2 = openApiPathItem;
			if (openApiPathItem2.Operations == null)
			{
				Dictionary<HttpMethod, OpenApiOperation> dictionary = (openApiPathItem2.Operations = new Dictionary<HttpMethod, OpenApiOperation>());
			}
			openApiPathItem.Operations?.Add(item.CurrentKeys.Operation, item.Operation);
			IList<IOpenApiParameter>? parameters = item.Parameters;
			if (parameters == null || !parameters.Any())
			{
				continue;
			}
			openApiPathItem2 = openApiPathItem;
			if (openApiPathItem2.Parameters == null)
			{
				IList<IOpenApiParameter> list = (openApiPathItem2.Parameters = new List<IOpenApiParameter>());
			}
			foreach (IOpenApiParameter parameter in item.Parameters)
			{
				if (openApiPathItem?.Parameters != null && !openApiPathItem.Parameters.Contains(parameter))
				{
					openApiPathItem.Parameters.Add(parameter);
				}
			}
		}
		if (openApiDocument.Paths == null)
		{
			throw new ArgumentException("No paths found for the supplied parameters.");
		}
		CopyReferences(openApiDocument);
		return openApiDocument;
	}

	/// <summary>
	/// Creates an <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> from a collection of <see cref="T:Microsoft.OpenApi.OpenApiDocument" />.
	/// </summary>
	/// <param name="sources">Dictionary of labels and their corresponding <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> objects.</param>
	/// <returns>The created <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" />.</returns>
	public static OpenApiUrlTreeNode CreateOpenApiUrlTreeNode(Dictionary<string, OpenApiDocument> sources)
	{
		OpenApiUrlTreeNode openApiUrlTreeNode = OpenApiUrlTreeNode.Create();
		foreach (KeyValuePair<string, OpenApiDocument> source in sources)
		{
			openApiUrlTreeNode.Attach(source.Value, source.Key);
		}
		return openApiUrlTreeNode;
	}

	private static Dictionary<HttpMethod, OpenApiOperation>? GetOpenApiOperations(OpenApiUrlTreeNode rootNode, string relativeUrl, string label)
	{
		if (relativeUrl.Equals("/", StringComparison.Ordinal) && rootNode.HasOperations(label))
		{
			return rootNode.PathItems[label].Operations;
		}
		string[] urlSegments = relativeUrl.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
		Dictionary<HttpMethod, OpenApiOperation> result = null;
		OpenApiUrlTreeNode openApiUrlTreeNode = rootNode;
		int num = 0;
		int i;
		for (i = 0; i < urlSegments?.Length; i++)
		{
			OpenApiUrlTreeNode openApiUrlTreeNode2 = openApiUrlTreeNode?.Children?.FirstOrDefault((KeyValuePair<string, OpenApiUrlTreeNode> x) => x.Key.Equals(urlSegments[i], StringComparison.OrdinalIgnoreCase)).Value;
			if (openApiUrlTreeNode2 == null)
			{
				if (i == 0)
				{
					break;
				}
				openApiUrlTreeNode2 = openApiUrlTreeNode?.Children?.FirstOrDefault((KeyValuePair<string, OpenApiUrlTreeNode> x) => x.Value.IsParameter).Value;
				if (openApiUrlTreeNode2 == null || num > 0)
				{
					break;
				}
				num++;
			}
			else
			{
				num = 0;
			}
			openApiUrlTreeNode = openApiUrlTreeNode2;
			if (i == urlSegments.Length - 1 && openApiUrlTreeNode.HasOperations(label))
			{
				result = openApiUrlTreeNode.PathItems[label].Operations;
			}
		}
		return result;
	}

	private static IList<SearchResult> FindOperations(OpenApiDocument sourceDocument, Func<string, HttpMethod, OpenApiOperation, bool> predicate)
	{
		OperationSearch operationSearch = new OperationSearch(predicate);
		new OpenApiWalker(operationSearch).Walk(sourceDocument);
		return operationSearch.SearchResults;
	}

	private static void CopyReferences(OpenApiDocument target)
	{
		CopyReferences copyReferences;
		do
		{
			copyReferences = new CopyReferences(target);
			new OpenApiWalker(copyReferences).Walk(target);
		}
		while (AddReferences(copyReferences.Components, target.Components));
	}

	private static bool AddReferences(OpenApiComponents newComponents, OpenApiComponents? target)
	{
		bool result = false;
		if (newComponents.Schemas != null)
		{
			foreach (KeyValuePair<string, IOpenApiSchema> schema in newComponents.Schemas)
			{
				if (target?.Schemas != null && !target.Schemas.ContainsKey(schema.Key))
				{
					result = true;
					target.Schemas.Add(schema.Key, schema.Value);
				}
			}
		}
		if (newComponents.Parameters != null)
		{
			foreach (KeyValuePair<string, IOpenApiParameter> parameter in newComponents.Parameters)
			{
				if (target?.Parameters != null && !target.Parameters.ContainsKey(parameter.Key))
				{
					result = true;
					target.Parameters.Add(parameter.Key, parameter.Value);
				}
			}
		}
		if (newComponents.Responses != null)
		{
			foreach (KeyValuePair<string, IOpenApiResponse> response in newComponents.Responses)
			{
				if (target?.Responses != null && !target.Responses.ContainsKey(response.Key))
				{
					result = true;
					target.Responses.Add(response.Key, response.Value);
				}
			}
		}
		if (newComponents.RequestBodies != null)
		{
			foreach (KeyValuePair<string, IOpenApiRequestBody> item in newComponents.RequestBodies.Where<KeyValuePair<string, IOpenApiRequestBody>>((KeyValuePair<string, IOpenApiRequestBody> item) => target?.RequestBodies != null && !target.RequestBodies.ContainsKey(item.Key)))
			{
				result = true;
				target?.RequestBodies?.Add(item.Key, item.Value);
			}
		}
		if (newComponents.Headers != null)
		{
			foreach (KeyValuePair<string, IOpenApiHeader> item2 in newComponents.Headers.Where<KeyValuePair<string, IOpenApiHeader>>((KeyValuePair<string, IOpenApiHeader> item) => target?.Headers != null && !target.Headers.ContainsKey(item.Key)))
			{
				result = true;
				target?.Headers?.Add(item2.Key, item2.Value);
			}
		}
		if (newComponents.Links != null)
		{
			foreach (KeyValuePair<string, IOpenApiLink> item3 in newComponents.Links.Where<KeyValuePair<string, IOpenApiLink>>((KeyValuePair<string, IOpenApiLink> item) => target?.Links != null && !target.Links.ContainsKey(item.Key)))
			{
				result = true;
				target?.Links?.Add(item3.Key, item3.Value);
			}
		}
		if (newComponents.Callbacks != null)
		{
			foreach (KeyValuePair<string, IOpenApiCallback> item4 in newComponents.Callbacks.Where<KeyValuePair<string, IOpenApiCallback>>((KeyValuePair<string, IOpenApiCallback> item) => target?.Callbacks != null && !target.Callbacks.ContainsKey(item.Key)))
			{
				result = true;
				target?.Callbacks?.Add(item4.Key, item4.Value);
			}
		}
		if (newComponents.Examples != null)
		{
			foreach (KeyValuePair<string, IOpenApiExample> item5 in newComponents.Examples.Where<KeyValuePair<string, IOpenApiExample>>((KeyValuePair<string, IOpenApiExample> item) => target?.Examples != null && !target.Examples.ContainsKey(item.Key)))
			{
				result = true;
				target?.Examples?.Add(item5.Key, item5.Value);
			}
		}
		if (newComponents.SecuritySchemes != null)
		{
			foreach (KeyValuePair<string, IOpenApiSecurityScheme> item6 in newComponents.SecuritySchemes.Where<KeyValuePair<string, IOpenApiSecurityScheme>>((KeyValuePair<string, IOpenApiSecurityScheme> item) => target?.SecuritySchemes != null && !target.SecuritySchemes.ContainsKey(item.Key)))
			{
				result = true;
				target?.SecuritySchemes?.Add(item6.Key, item6.Value);
			}
		}
		return result;
	}

	private static string ExtractPath(string url, IList<OpenApiServer>? serverList)
	{
		string text = serverList?.Select((OpenApiServer s) => s.Url?.TrimEnd('/')).FirstOrDefault((string c) => c != null && url.Contains(c));
		if (text != null)
		{
			return url.Split(new string[1] { text }, StringSplitOptions.None)[1];
		}
		return new Uri(new Uri(SRResource.DefaultBaseUri), url).GetComponents(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.Unescaped);
	}

	private static void ValidateFilters(Dictionary<string, List<string>>? requestUrls, string? operationIds, string? tags)
	{
		if (requestUrls != null && (operationIds != null || tags != null))
		{
			throw new InvalidOperationException("Cannot filter by Postman collection and either operationIds and tags at the same time.");
		}
		if (!string.IsNullOrEmpty(operationIds) && !string.IsNullOrEmpty(tags))
		{
			throw new InvalidOperationException("Cannot specify both operationIds and tags at the same time.");
		}
	}

	private static Func<string, HttpMethod, OpenApiOperation, bool> GetOperationIdsPredicate(string operationIds)
	{
		if (operationIds == "*")
		{
			return (string _, HttpMethod _, OpenApiOperation _) => true;
		}
		string[] operationIdsArray = operationIds.Split(',');
		return (string _, HttpMethod _, OpenApiOperation operation) => operationIdsArray.Contains<string>(operation.OperationId);
	}

	private static Func<string, HttpMethod, OpenApiOperation, bool> GetTagsPredicate(string tags)
	{
		string[] tagsArray = tags.Split(',');
		if (tagsArray.Length == 1)
		{
			Regex regex = new Regex(tagsArray[0]);
			return (string _, HttpMethod _, OpenApiOperation operation) => operation.Tags?.Any((OpenApiTagReference tag) => tag.Name != null && regex.IsMatch(tag.Name)) ?? false;
		}
		return (string _, HttpMethod _, OpenApiOperation operation) => operation.Tags?.Any((OpenApiTagReference tag) => tagsArray.Contains<string>(tag.Name)) ?? false;
	}

	private static Func<string, HttpMethod, OpenApiOperation, bool> GetRequestUrlsPredicate(Dictionary<string, List<string>> requestUrls, OpenApiDocument source)
	{
		List<string> operationTypes = new List<string>();
		if (source != null)
		{
			string version = source.Info.Version;
			if (version != null)
			{
				OpenApiUrlTreeNode rootNode = CreateOpenApiUrlTreeNode(new Dictionary<string, OpenApiDocument> { { version, source } });
				foreach (KeyValuePair<string, List<string>> requestUrl in requestUrls)
				{
					IList<OpenApiServer> servers = source.Servers;
					string text = ExtractPath(requestUrl.Key, servers);
					Dictionary<HttpMethod, OpenApiOperation> openApiOperations = GetOpenApiOperations(rootNode, text, version);
					if (openApiOperations != null)
					{
						operationTypes.AddRange(GetOperationTypes(openApiOperations, requestUrl.Value, text));
					}
				}
			}
		}
		if (!operationTypes.Any())
		{
			throw new ArgumentException("The urls in the Postman collection supplied could not be found.");
		}
		return (string path, HttpMethod operationType, OpenApiOperation _) => operationTypes.Contains(operationType?.ToString() + path);
	}

	private static List<string> GetOperationTypes(Dictionary<HttpMethod, OpenApiOperation> openApiOperations, List<string> url, string path)
	{
		return (from ops in openApiOperations
			where url.Contains(ops.Key.ToString().ToUpper())
			select ops.Key?.ToString() + path).ToList();
	}
}
