using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Contains a set of OpenApi documents and document fragments that reference each other
/// </summary>
public class OpenApiWorkspace
{
	private sealed class UriWithFragmentEqualityComparer : IEqualityComparer<Uri>
	{
		public bool Equals(Uri? x, Uri? y)
		{
			if ((object)x == y)
			{
				return true;
			}
			if ((object)x == null || (object)y == null)
			{
				return false;
			}
			return x.AbsoluteUri == y.AbsoluteUri;
		}

		public int GetHashCode(Uri obj)
		{
			return obj.AbsoluteUri.GetHashCode();
		}
	}

	private readonly Dictionary<string, Uri> _documentsIdRegistry = new Dictionary<string, Uri>();

	private readonly Dictionary<Uri, Stream> _artifactsRegistry = new Dictionary<Uri, Stream>();

	private readonly Dictionary<Uri, IOpenApiReferenceable> _IOpenApiReferenceableRegistry = new Dictionary<Uri, IOpenApiReferenceable>(new UriWithFragmentEqualityComparer());

	private const string ComponentSegmentSeparator = "/";

	/// <summary>
	/// The base location from where all relative references are resolved
	/// </summary>
	public Uri? BaseUrl { get; }

	/// <summary>
	/// Initialize workspace pointing to a base URL to allow resolving relative document locations.  Use a file:// url to point to a folder
	/// </summary>
	/// <param name="baseUrl"></param>
	public OpenApiWorkspace(Uri baseUrl)
	{
		BaseUrl = baseUrl;
	}

	/// <summary>
	/// Initialize workspace using current directory as the default location.
	/// </summary>
	public OpenApiWorkspace()
	{
		BaseUrl = new Uri("https://openapi.net/");
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiWorkspace" /> object
	/// </summary>
	public OpenApiWorkspace(OpenApiWorkspace workspace)
	{
	}

	/// <summary>
	/// Returns the total count of all the components in the workspace registry
	/// </summary>
	/// <returns></returns>
	public int ComponentsCount()
	{
		return _IOpenApiReferenceableRegistry.Count + _artifactsRegistry.Count;
	}

	/// <summary>
	/// Registers a document's components into the workspace
	/// </summary>
	/// <param name="document"></param>
	public void RegisterComponents(OpenApiDocument document)
	{
		if (document?.Components == null)
		{
			return;
		}
		string baseUri = getBaseUri(document);
		if (document.Components.Schemas != null)
		{
			foreach (KeyValuePair<string, IOpenApiSchema> schema in document.Components.Schemas)
			{
				string location = schema.Value.Id ?? (baseUri + ReferenceType.Schema.GetDisplayName() + "/" + schema.Key);
				RegisterComponent(location, schema.Value);
			}
		}
		if (document.Components.Parameters != null)
		{
			foreach (KeyValuePair<string, IOpenApiParameter> parameter in document.Components.Parameters)
			{
				string location = baseUri + ReferenceType.Parameter.GetDisplayName() + "/" + parameter.Key;
				RegisterComponent(location, parameter.Value);
			}
		}
		if (document.Components.Responses != null)
		{
			foreach (KeyValuePair<string, IOpenApiResponse> response in document.Components.Responses)
			{
				string location = baseUri + ReferenceType.Response.GetDisplayName() + "/" + response.Key;
				RegisterComponent(location, response.Value);
			}
		}
		if (document.Components.RequestBodies != null)
		{
			foreach (KeyValuePair<string, IOpenApiRequestBody> requestBody in document.Components.RequestBodies)
			{
				string location = baseUri + ReferenceType.RequestBody.GetDisplayName() + "/" + requestBody.Key;
				RegisterComponent(location, requestBody.Value);
			}
		}
		if (document.Components.Links != null)
		{
			foreach (KeyValuePair<string, IOpenApiLink> link in document.Components.Links)
			{
				string location = baseUri + ReferenceType.Link.GetDisplayName() + "/" + link.Key;
				RegisterComponent(location, link.Value);
			}
		}
		if (document.Components.Callbacks != null)
		{
			foreach (KeyValuePair<string, IOpenApiCallback> callback in document.Components.Callbacks)
			{
				string location = baseUri + ReferenceType.Callback.GetDisplayName() + "/" + callback.Key;
				RegisterComponent(location, callback.Value);
			}
		}
		if (document.Components.PathItems != null)
		{
			foreach (KeyValuePair<string, IOpenApiPathItem> pathItem in document.Components.PathItems)
			{
				string location = baseUri + ReferenceType.PathItem.GetDisplayName() + "/" + pathItem.Key;
				RegisterComponent(location, pathItem.Value);
			}
		}
		if (document.Components.Examples != null)
		{
			foreach (KeyValuePair<string, IOpenApiExample> example in document.Components.Examples)
			{
				string location = baseUri + ReferenceType.Example.GetDisplayName() + "/" + example.Key;
				RegisterComponent(location, example.Value);
			}
		}
		if (document.Components.Headers != null)
		{
			foreach (KeyValuePair<string, IOpenApiHeader> header in document.Components.Headers)
			{
				string location = baseUri + ReferenceType.Header.GetDisplayName() + "/" + header.Key;
				RegisterComponent(location, header.Value);
			}
		}
		if (document.Components.SecuritySchemes != null)
		{
			foreach (KeyValuePair<string, IOpenApiSecurityScheme> securityScheme in document.Components.SecuritySchemes)
			{
				string location = baseUri + ReferenceType.SecurityScheme.GetDisplayName() + "/" + securityScheme.Key;
				RegisterComponent(location, securityScheme.Value);
			}
		}
		if (document.Components.MediaTypes == null)
		{
			return;
		}
		foreach (KeyValuePair<string, IOpenApiMediaType> mediaType in document.Components.MediaTypes)
		{
			string location = baseUri + ReferenceType.MediaType.GetDisplayName() + "/" + mediaType.Key;
			RegisterComponent(location, mediaType.Value);
		}
	}

	private static string getBaseUri(OpenApiDocument openApiDocument)
	{
		return openApiDocument.BaseUri?.ToString() + "#/components/";
	}

	/// <summary>
	/// Registers a component for a document in the workspace
	/// </summary>
	/// <param name="openApiDocument">The document to register the component for.</param>
	/// <param name="componentToRegister">The component to register.</param>
	/// <param name="id">The id of the component.</param>
	/// <typeparam name="T">The type of the component to register.</typeparam>
	/// <returns>true if the component is successfully registered; otherwise false.</returns>
	/// <exception cref="T:System.ArgumentNullException">openApiDocument is null</exception>
	/// <exception cref="T:System.ArgumentNullException">componentToRegister is null</exception>
	/// <exception cref="T:System.ArgumentNullException">id is null or empty</exception>
	public bool RegisterComponentForDocument<T>(OpenApiDocument openApiDocument, T componentToRegister, string id)
	{
		Utils.CheckArgumentNull(openApiDocument, "openApiDocument");
		Utils.CheckArgumentNull(componentToRegister, "componentToRegister");
		Utils.CheckArgumentNullOrEmpty(id, "id");
		string baseUri = getBaseUri(openApiDocument);
		string text;
		if (!(componentToRegister is IOpenApiSchema))
		{
			if (!(componentToRegister is IOpenApiParameter))
			{
				if (!(componentToRegister is IOpenApiResponse))
				{
					if (!(componentToRegister is IOpenApiRequestBody))
					{
						if (!(componentToRegister is IOpenApiLink))
						{
							if (!(componentToRegister is IOpenApiCallback))
							{
								if (!(componentToRegister is IOpenApiPathItem))
								{
									if (!(componentToRegister is IOpenApiExample))
									{
										if (!(componentToRegister is IOpenApiHeader))
										{
											if (!(componentToRegister is IOpenApiSecurityScheme))
											{
												if (!(componentToRegister is IOpenApiMediaType))
												{
													throw new ArgumentException("Invalid component type " + componentToRegister.GetType().Name);
												}
												text = baseUri + ReferenceType.MediaType.GetDisplayName() + "/" + id;
											}
											else
											{
												text = baseUri + ReferenceType.SecurityScheme.GetDisplayName() + "/" + id;
											}
										}
										else
										{
											text = baseUri + ReferenceType.Header.GetDisplayName() + "/" + id;
										}
									}
									else
									{
										text = baseUri + ReferenceType.Example.GetDisplayName() + "/" + id;
									}
								}
								else
								{
									text = baseUri + ReferenceType.PathItem.GetDisplayName() + "/" + id;
								}
							}
							else
							{
								text = baseUri + ReferenceType.Callback.GetDisplayName() + "/" + id;
							}
						}
						else
						{
							text = baseUri + ReferenceType.Link.GetDisplayName() + "/" + id;
						}
					}
					else
					{
						text = baseUri + ReferenceType.RequestBody.GetDisplayName() + "/" + id;
					}
				}
				else
				{
					text = baseUri + ReferenceType.Response.GetDisplayName() + "/" + id;
				}
			}
			else
			{
				text = baseUri + ReferenceType.Parameter.GetDisplayName() + "/" + id;
			}
		}
		else
		{
			text = baseUri + ReferenceType.Schema.GetDisplayName() + "/" + id;
		}
		string location = text;
		return RegisterComponent(location, componentToRegister);
	}

	/// <summary>
	/// Registers a component in the component registry.
	/// </summary>
	/// <param name="location"></param>
	/// <param name="component"></param>
	/// <returns>true if the component is successfully registered; otherwise false.</returns>
	internal bool RegisterComponent<T>(string location, T component)
	{
		Uri uri = ToLocationUrl(location);
		if ((object)uri != null)
		{
			if (component is IOpenApiReferenceable value)
			{
				if (!_IOpenApiReferenceableRegistry.ContainsKey(uri))
				{
					_IOpenApiReferenceableRegistry[uri] = value;
					return true;
				}
			}
			else if (component is Stream value2 && !_artifactsRegistry.ContainsKey(uri))
			{
				_artifactsRegistry[uri] = value2;
				return true;
			}
			return false;
		}
		return false;
	}

	/// <summary>
	/// Adds a document id to the dictionaries of document locations and their ids.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	public void AddDocumentId(string? key, Uri? value)
	{
		if (!string.IsNullOrEmpty(key) && key != null && (object)value != null && !_documentsIdRegistry.ContainsKey(key))
		{
			_documentsIdRegistry[key] = value;
		}
	}

	/// <summary>
	/// Retrieves the document id given a key.
	/// </summary>
	/// <param name="key"></param>
	/// <returns>The document id of the given key.</returns>
	public Uri? GetDocumentId(string? key)
	{
		if (key != null && _documentsIdRegistry.TryGetValue(key, out Uri value))
		{
			return value;
		}
		return null;
	}

	/// <summary>
	/// Verify if workspace contains a component based on its URL.
	/// </summary>
	/// <param name="location">A relative or absolute URL of the file.  Use file:// for folder locations.</param>
	/// <returns>Returns true if a matching document is found.</returns>
	public bool Contains(string location)
	{
		Uri uri = ToLocationUrl(location);
		if ((object)uri == null)
		{
			return false;
		}
		if (!_IOpenApiReferenceableRegistry.ContainsKey(uri))
		{
			return _artifactsRegistry.ContainsKey(uri);
		}
		return true;
	}

	/// <summary>
	/// Resolves a reference given a key.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="location"></param>
	/// <returns>The resolved reference.</returns>
	public T? ResolveReference<T>(string location)
	{
		if (string.IsNullOrEmpty(location))
		{
			return default(T);
		}
		Uri uri = ToLocationUrl(location);
		if ((object)uri != null)
		{
			if (_IOpenApiReferenceableRegistry.TryGetValue(uri, out IOpenApiReferenceable value) && value is T)
			{
				return (T)value;
			}
			if (_artifactsRegistry.TryGetValue(uri, out Stream value2) && value2 is T)
			{
				return (T)(object)((value2 is T) ? value2 : null);
			}
		}
		return default(T);
	}

	/// <summary>
	/// Recursively resolves a schema from a URI fragment.
	/// </summary>
	/// <param name="location"></param>
	/// <param name="parentSchema">The parent schema to resolve against.</param>
	/// <returns></returns>
	internal IOpenApiSchema? ResolveJsonSchemaReference(string location, IOpenApiSchema parentSchema)
	{
		if (!string.IsNullOrEmpty(location))
		{
			Uri uri = ToLocationUrl(location);
			if ((object)uri != null)
			{
				if (!location.Contains("#/components/schemas/", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("Invalid schema reference location: " + location + ". It should contain '#/components/schemas/'");
				}
				string[] array = uri.Fragment.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				string fragment = "/components/" + ReferenceType.Schema.GetDisplayName() + "/" + array[3];
				UriBuilder uriBuilder = new UriBuilder(uri)
				{
					Fragment = fragment
				};
				if (_IOpenApiReferenceableRegistry.TryGetValue(uriBuilder.Uri, out IOpenApiReferenceable value) && value is IOpenApiSchema schema)
				{
					string[] pathSegments = array.Skip(4).ToArray();
					Stack<IOpenApiSchema> stack = new Stack<IOpenApiSchema>();
					stack.Push(parentSchema);
					return ResolveSubSchema(schema, pathSegments, stack);
				}
				return null;
			}
		}
		return null;
	}

	internal static IOpenApiSchema? ResolveSubSchema(IOpenApiSchema schema, string[] pathSegments, Stack<IOpenApiSchema> visitedSchemas)
	{
		if (visitedSchemas.Contains(schema))
		{
			if (schema is OpenApiSchemaReference openApiSchemaReference)
			{
				throw new InvalidOperationException("Circular reference detected while resolving schema: " + openApiSchemaReference.Reference.ReferenceV3);
			}
			throw new InvalidOperationException("Circular reference detected while resolving schema");
		}
		visitedSchemas.Push(schema);
		if (pathSegments.Length == 0)
		{
			return schema;
		}
		string text = pathSegments[0];
		List<string> list = new List<string>();
		list.AddRange(pathSegments.Skip(1));
		pathSegments = list.ToArray();
		switch (text)
		{
		case "properties":
		{
			string key = pathSegments[0];
			if (schema.Properties != null && schema.Properties.TryGetValue(key, out IOpenApiSchema value))
			{
				IOpenApiSchema schema4 = value;
				List<string> list4 = new List<string>();
				list4.AddRange(pathSegments.Skip(1));
				return ResolveSubSchema(schema4, list4.ToArray(), visitedSchemas);
			}
			break;
		}
		case "items":
			if (!(schema.Items is OpenApiSchema schema3))
			{
				return null;
			}
			return ResolveSubSchema(schema3, pathSegments, visitedSchemas);
		case "additionalProperties":
			if (!(schema.AdditionalProperties is OpenApiSchema schema5))
			{
				return null;
			}
			return ResolveSubSchema(schema5, pathSegments, visitedSchemas);
		case "allOf":
		case "anyOf":
		case "oneOf":
		{
			if (!int.TryParse(pathSegments[0], out var result))
			{
				return null;
			}
			IList<IOpenApiSchema> list2 = text switch
			{
				"allOf" => schema.AllOf, 
				"anyOf" => schema.AnyOf, 
				"oneOf" => schema.OneOf, 
				_ => null, 
			};
			if (list2 != null && result < list2.Count)
			{
				IOpenApiSchema schema2 = list2[result];
				List<string> list3 = new List<string>();
				list3.AddRange(pathSegments.Skip(1));
				return ResolveSubSchema(schema2, list3.ToArray(), visitedSchemas);
			}
			break;
		}
		}
		return null;
	}

	private Uri? ToLocationUrl(string location)
	{
		if ((object)BaseUrl != null)
		{
			return new Uri(BaseUrl, location);
		}
		return null;
	}
}
