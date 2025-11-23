using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// The walker to visit multiple Open API elements.
/// </summary>
public class OpenApiWalker
{
	private readonly OpenApiVisitorBase _visitor;

	private readonly Stack<IOpenApiSchema> _schemaLoop = new Stack<IOpenApiSchema>();

	private readonly Stack<IOpenApiPathItem> _pathItemLoop = new Stack<IOpenApiPathItem>();

	/// <summary>
	/// Initializes the <see cref="T:Microsoft.OpenApi.OpenApiWalker" /> class.
	/// </summary>
	public OpenApiWalker(OpenApiVisitorBase visitor)
	{
		_visitor = visitor;
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> and child objects
	/// </summary>
	/// <param name="doc">OpenApiDocument to be walked</param>
	public void Walk(OpenApiDocument? doc)
	{
		if (doc == null)
		{
			return;
		}
		_schemaLoop.Clear();
		_pathItemLoop.Clear();
		_visitor.Visit(doc);
		OpenApiInfo info = doc.Info;
		if (info != null)
		{
			WalkItem("info", info, delegate(OpenApiWalker self, OpenApiInfo item)
			{
				self.Walk(item);
			});
		}
		IList<OpenApiServer> servers = doc.Servers;
		if (servers != null)
		{
			WalkItem("servers", servers, delegate(OpenApiWalker self, IList<OpenApiServer> item)
			{
				self.Walk(item);
			});
		}
		OpenApiPaths paths = doc.Paths;
		if (paths != null)
		{
			WalkItem("paths", paths, delegate(OpenApiWalker self, OpenApiPaths item)
			{
				self.Walk(item);
			});
		}
		WalkDictionary("webhooks", doc.Webhooks, delegate(OpenApiWalker self, IOpenApiPathItem item, bool isComponent)
		{
			self.Walk(item, isComponent);
		});
		OpenApiComponents components = doc.Components;
		if (components != null)
		{
			WalkItem("components", components, delegate(OpenApiWalker self, OpenApiComponents item)
			{
				self.Walk(item);
			});
		}
		IList<OpenApiSecurityRequirement> security = doc.Security;
		if (security != null)
		{
			WalkItem("security", security, delegate(OpenApiWalker self, IList<OpenApiSecurityRequirement> item)
			{
				self.Walk(item);
			});
		}
		OpenApiExternalDocs externalDocs = doc.ExternalDocs;
		if (externalDocs != null)
		{
			WalkItem("externalDocs", externalDocs, delegate(OpenApiWalker self, OpenApiExternalDocs item)
			{
				self.Walk(item);
			});
		}
		ISet<OpenApiTag> tags = doc.Tags;
		if (tags != null)
		{
			WalkItem("tags", tags, delegate(OpenApiWalker self, ISet<OpenApiTag> item)
			{
				self.Walk(item);
			});
		}
		Walk((IOpenApiExtensible?)doc);
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiTag" /> and child objects
	/// </summary>
	internal void Walk(ISet<OpenApiTag>? tags)
	{
		if (tags != null)
		{
			_visitor.Visit(tags);
			WalkTags(tags, delegate(OpenApiWalker self, OpenApiTag tag)
			{
				self.Walk(tag);
			});
		}
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiTagReference" /> and child objects
	/// </summary>
	internal void Walk(ISet<OpenApiTagReference>? tags)
	{
		if (tags != null)
		{
			_visitor.Visit(tags);
			WalkTags(tags, delegate(OpenApiWalker self, OpenApiTagReference tag)
			{
				self.Walk(tag);
			});
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> and child objects
	/// </summary>
	internal void Walk(string externalDocs)
	{
		if (externalDocs != null)
		{
			_visitor.Visit(externalDocs);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiExternalDocs" /> and child objects
	/// </summary>
	internal void Walk(OpenApiExternalDocs? externalDocs)
	{
		if (externalDocs != null)
		{
			_visitor.Visit(externalDocs);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiComponents" /> and child objects
	/// </summary>
	internal void Walk(OpenApiComponents? components)
	{
		if (components != null)
		{
			_visitor.Visit(components);
			bool isComponent = true;
			WalkDictionary("schemas", components.Schemas, delegate(OpenApiWalker self, IOpenApiSchema item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("securitySchemes", components.SecuritySchemes, delegate(OpenApiWalker self, IOpenApiSecurityScheme item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("callbacks", components.Callbacks, delegate(OpenApiWalker self, IOpenApiCallback item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("pathItems", components.PathItems, delegate(OpenApiWalker self, IOpenApiPathItem item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("parameters", components.Parameters, delegate(OpenApiWalker self, IOpenApiParameter item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("examples", components.Examples, delegate(OpenApiWalker self, IOpenApiExample item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("headers", components.Headers, delegate(OpenApiWalker self, IOpenApiHeader item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("links", components.Links, delegate(OpenApiWalker self, IOpenApiLink item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("requestBodies", components.RequestBodies, delegate(OpenApiWalker self, IOpenApiRequestBody item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("responses", components.Responses, delegate(OpenApiWalker self, IOpenApiResponse item, bool flag)
			{
				self.Walk(item);
			}, isComponent);
			WalkDictionary("mediaTypes", components.MediaTypes, delegate(OpenApiWalker self, IOpenApiMediaType item, bool isComponent2)
			{
				self.Walk(item, isComponent2);
			}, isComponent);
			Walk((IOpenApiExtensible?)components);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiPaths" /> and child objects
	/// </summary>
	internal void Walk(OpenApiPaths paths)
	{
		if (paths == null)
		{
			return;
		}
		_visitor.Visit(paths);
		foreach (KeyValuePair<string, IOpenApiPathItem> path in paths)
		{
			if (path.Value != null)
			{
				_visitor.CurrentKeys.Path = path.Key;
				WalkItem(path.Key, path.Value, delegate(OpenApiWalker self, IOpenApiPathItem item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Path = null;
			}
		}
	}

	/// <summary>
	/// Visits Webhooks and child objects
	/// </summary>
	internal void Walk(IDictionary<string, IOpenApiPathItem>? webhooks)
	{
		if (webhooks == null)
		{
			return;
		}
		_visitor.Visit(webhooks);
		foreach (KeyValuePair<string, IOpenApiPathItem> webhook in webhooks)
		{
			if (webhook.Value != null)
			{
				_visitor.CurrentKeys.Path = webhook.Key;
				WalkItem(webhook.Key, webhook.Value, delegate(OpenApiWalker self, IOpenApiPathItem item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Path = null;
			}
		}
	}

	/// <summary>
	/// Visits list of  <see cref="T:Microsoft.OpenApi.OpenApiServer" /> and child objects
	/// </summary>
	internal void Walk(IList<OpenApiServer>? servers)
	{
		if (servers == null)
		{
			return;
		}
		_visitor.Visit(servers);
		for (int i = 0; i < servers.Count; i++)
		{
			OpenApiServer openApiServer = servers[i];
			if (openApiServer != null)
			{
				WalkItem(i.ToString(), openApiServer, delegate(OpenApiWalker self, OpenApiServer item)
				{
					self.Walk(item);
				});
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiInfo" /> and child objects
	/// </summary>
	internal void Walk(OpenApiInfo info)
	{
		if (info == null)
		{
			return;
		}
		_visitor.Visit(info);
		OpenApiContact contact = info.Contact;
		if (contact != null)
		{
			WalkItem("contact", contact, delegate(OpenApiWalker self, OpenApiContact item)
			{
				self.Walk(item);
			});
		}
		OpenApiLicense license = info.License;
		if (license != null)
		{
			WalkItem("license", license, delegate(OpenApiWalker self, OpenApiLicense item)
			{
				self.Walk(item);
			});
		}
		Walk((IOpenApiExtensible?)info);
	}

	/// <summary>
	/// Visits dictionary of extensions
	/// </summary>
	internal void Walk(IOpenApiExtensible? openApiExtensible)
	{
		if (openApiExtensible == null)
		{
			return;
		}
		_visitor.Visit(openApiExtensible);
		if (openApiExtensible.Extensions == null)
		{
			return;
		}
		foreach (KeyValuePair<string, IOpenApiExtension> extension in openApiExtensible.Extensions)
		{
			if (extension.Value != null)
			{
				_visitor.CurrentKeys.Extension = extension.Key;
				WalkItem(extension.Key, extension.Value, delegate(OpenApiWalker self, IOpenApiExtension item)
				{
					self.Walk(item);
				});
				_visitor.CurrentKeys.Extension = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiExtension" />
	/// </summary>
	internal void Walk(IOpenApiExtension extension)
	{
		if (extension != null)
		{
			_visitor.Visit(extension);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiLicense" /> and child objects
	/// </summary>
	internal void Walk(OpenApiLicense? license)
	{
		if (license != null)
		{
			_visitor.Visit(license);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiContact" /> and child objects
	/// </summary>
	internal void Walk(OpenApiContact? contact)
	{
		if (contact != null)
		{
			_visitor.Visit(contact);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiCallback" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiCallback callback, bool isComponent = false)
	{
		if (callback == null)
		{
			return;
		}
		if (callback is IOpenApiReferenceHolder referenceableHolder)
		{
			Walk(referenceableHolder);
			return;
		}
		_visitor.Visit(callback);
		if (callback.PathItems == null)
		{
			return;
		}
		foreach (KeyValuePair<RuntimeExpression, IOpenApiPathItem> pathItem in callback.PathItems)
		{
			if (pathItem.Value != null)
			{
				string text = pathItem.Key.ToString();
				_visitor.CurrentKeys.Callback = text;
				WalkItem(text, pathItem.Value, delegate(OpenApiWalker self, IOpenApiPathItem item)
				{
					self.Walk(item);
				});
				_visitor.CurrentKeys.Callback = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiTag" /> and child objects
	/// </summary>
	internal void Walk(OpenApiTag tag)
	{
		if (tag != null)
		{
			_visitor.Visit(tag);
			OpenApiExternalDocs externalDocs = tag.ExternalDocs;
			if (externalDocs != null)
			{
				_visitor.Visit(externalDocs);
			}
			_visitor.Visit((IOpenApiExtensible)tag);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiTagReference" /> and child objects
	/// </summary>
	internal void Walk(OpenApiTagReference tag)
	{
		if (tag != null)
		{
			Walk((IOpenApiReferenceHolder)tag);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiServer" /> and child objects
	/// </summary>
	internal void Walk(OpenApiServer? server)
	{
		if (server == null)
		{
			return;
		}
		_visitor.Visit(server);
		IDictionary<string, OpenApiServerVariable> variables = server.Variables;
		if (variables != null)
		{
			WalkItem("variables", variables, delegate(OpenApiWalker self, IDictionary<string, OpenApiServerVariable> item)
			{
				self.Walk(item);
			});
		}
		_visitor.Visit((IOpenApiExtensible)server);
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.OpenApiServerVariable" />
	/// </summary>
	internal void Walk(IDictionary<string, OpenApiServerVariable>? serverVariables)
	{
		if (serverVariables == null)
		{
			return;
		}
		_visitor.Visit(serverVariables);
		foreach (KeyValuePair<string, OpenApiServerVariable> serverVariable in serverVariables)
		{
			if (serverVariable.Value != null)
			{
				_visitor.CurrentKeys.ServerVariable = serverVariable.Key;
				WalkItem(serverVariable.Key, serverVariable.Value, delegate(OpenApiWalker self, OpenApiServerVariable item)
				{
					self.Walk(item);
				});
				_visitor.CurrentKeys.ServerVariable = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiServerVariable" /> and child objects
	/// </summary>
	internal void Walk(OpenApiServerVariable serverVariable)
	{
		if (serverVariable != null)
		{
			_visitor.Visit(serverVariable);
			_visitor.Visit((IOpenApiExtensible)serverVariable);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiPathItem" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiPathItem pathItem, bool isComponent = false)
	{
		if (pathItem == null)
		{
			return;
		}
		if (pathItem is IOpenApiReferenceHolder referenceableHolder)
		{
			Walk(referenceableHolder);
		}
		else
		{
			if (_pathItemLoop.Contains(pathItem))
			{
				return;
			}
			_pathItemLoop.Push(pathItem);
			_visitor.Visit(pathItem);
			if (pathItem != null)
			{
				IList<IOpenApiParameter> parameters = pathItem.Parameters;
				if (parameters != null)
				{
					WalkItem("parameters", parameters, delegate(OpenApiWalker self, IList<IOpenApiParameter> item)
					{
						self.Walk(item);
					});
				}
				Walk(pathItem.Operations);
			}
			if (pathItem is IOpenApiExtensible openApiExtensible)
			{
				_visitor.Visit(openApiExtensible);
			}
			_pathItemLoop.Pop();
		}
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.OpenApiOperation" />
	/// </summary>
	internal void Walk(IDictionary<HttpMethod, OpenApiOperation>? operations)
	{
		if (operations == null)
		{
			return;
		}
		_visitor.Visit(operations);
		foreach (KeyValuePair<HttpMethod, OpenApiOperation> operation in operations)
		{
			if (operation.Value != null)
			{
				_visitor.CurrentKeys.Operation = operation.Key;
				WalkItem(operation.Key.Method.ToLowerInvariant(), operation.Value, delegate(OpenApiWalker self, OpenApiOperation item)
				{
					self.Walk(item);
				});
				_visitor.CurrentKeys.Operation = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiOperation" /> and child objects
	/// </summary>
	/// <param name="operation"></param>
	internal void Walk(OpenApiOperation operation)
	{
		if (operation == null)
		{
			return;
		}
		_visitor.Visit(operation);
		IList<IOpenApiParameter> parameters = operation.Parameters;
		if (parameters != null)
		{
			WalkItem("parameters", parameters, delegate(OpenApiWalker self, IList<IOpenApiParameter> item)
			{
				self.Walk(item);
			});
		}
		IOpenApiRequestBody requestBody = operation.RequestBody;
		if (requestBody != null)
		{
			WalkItem("requestBody", requestBody, delegate(OpenApiWalker self, IOpenApiRequestBody item)
			{
				self.Walk(item);
			});
		}
		OpenApiResponses responses = operation.Responses;
		if (responses != null)
		{
			WalkItem("responses", responses, delegate(OpenApiWalker self, OpenApiResponses item)
			{
				self.Walk(item);
			});
		}
		WalkDictionary("callbacks", operation.Callbacks, delegate(OpenApiWalker self, IOpenApiCallback item, bool isComponent)
		{
			self.Walk(item, isComponent);
		});
		ISet<OpenApiTagReference> tags = operation.Tags;
		if (tags != null)
		{
			WalkItem("tags", tags, delegate(OpenApiWalker self, ISet<OpenApiTagReference> item)
			{
				self.Walk(item);
			});
		}
		IList<OpenApiSecurityRequirement> security = operation.Security;
		if (security != null)
		{
			WalkItem("security", security, delegate(OpenApiWalker self, IList<OpenApiSecurityRequirement> item)
			{
				self.Walk(item);
			});
		}
		Walk((IOpenApiExtensible?)operation);
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" />
	/// </summary>
	internal void Walk(IList<OpenApiSecurityRequirement>? securityRequirements)
	{
		if (securityRequirements == null)
		{
			return;
		}
		_visitor.Visit(securityRequirements);
		for (int i = 0; i < securityRequirements.Count; i++)
		{
			OpenApiSecurityRequirement openApiSecurityRequirement = securityRequirements[i];
			if (openApiSecurityRequirement != null)
			{
				WalkItem(i.ToString(), openApiSecurityRequirement, delegate(OpenApiWalker self, OpenApiSecurityRequirement item)
				{
					self.Walk(item);
				});
			}
		}
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiParameter" />
	/// </summary>
	internal void Walk(IList<IOpenApiParameter>? parameters)
	{
		if (parameters == null)
		{
			return;
		}
		_visitor.Visit(parameters);
		for (int i = 0; i < parameters.Count; i++)
		{
			IOpenApiParameter openApiParameter = parameters[i];
			if (openApiParameter != null)
			{
				WalkItem(i.ToString(), openApiParameter, delegate(OpenApiWalker self, IOpenApiParameter item)
				{
					self.Walk(item);
				});
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiParameter" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiParameter parameter, bool isComponent = false)
	{
		if (parameter == null)
		{
			return;
		}
		if (parameter is IOpenApiReferenceHolder referenceableHolder)
		{
			Walk(referenceableHolder);
			return;
		}
		_visitor.Visit(parameter);
		IOpenApiSchema schema = parameter.Schema;
		if (schema != null)
		{
			WalkItem("schema", schema, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent2)
			{
				self.Walk(item, isComponent2);
			}, isComponent: false);
		}
		IDictionary<string, IOpenApiMediaType> content = parameter.Content;
		if (content != null)
		{
			WalkItem("content", content, delegate(OpenApiWalker self, IDictionary<string, IOpenApiMediaType> item)
			{
				self.Walk(item);
			});
		}
		WalkDictionary("examples", parameter.Examples, delegate(OpenApiWalker self, IOpenApiExample item, bool isComponent2)
		{
			self.Walk(item, isComponent2);
		});
		Walk(parameter as IOpenApiExtensible);
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiResponses" /> and child objects
	/// </summary>
	internal void Walk(OpenApiResponses? responses)
	{
		if (responses == null)
		{
			return;
		}
		_visitor.Visit(responses);
		foreach (KeyValuePair<string, IOpenApiResponse> response in responses)
		{
			if (response.Value != null)
			{
				_visitor.CurrentKeys.Response = response.Key;
				WalkItem(response.Key, response.Value, delegate(OpenApiWalker self, IOpenApiResponse item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Response = null;
			}
		}
		Walk((IOpenApiExtensible?)responses);
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiResponse" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiResponse response, bool isComponent = false)
	{
		if (response == null)
		{
			return;
		}
		if (response is IOpenApiReferenceHolder referenceableHolder)
		{
			Walk(referenceableHolder);
			return;
		}
		_visitor.Visit(response);
		IDictionary<string, IOpenApiMediaType> content = response.Content;
		if (content != null)
		{
			WalkItem("content", content, delegate(OpenApiWalker self, IDictionary<string, IOpenApiMediaType> item)
			{
				self.Walk(item);
			});
		}
		WalkDictionary("links", response.Links, delegate(OpenApiWalker self, IOpenApiLink item, bool isComponent2)
		{
			self.Walk(item, isComponent2);
		});
		WalkDictionary("headers", response.Headers, delegate(OpenApiWalker self, IOpenApiHeader item, bool isComponent2)
		{
			self.Walk(item, isComponent2);
		});
		Walk(response as IOpenApiExtensible);
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiRequestBody" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiRequestBody? requestBody, bool isComponent = false)
	{
		if (requestBody == null)
		{
			return;
		}
		if (requestBody is IOpenApiReferenceHolder referenceableHolder)
		{
			Walk(referenceableHolder);
			return;
		}
		_visitor.Visit(requestBody);
		IDictionary<string, IOpenApiMediaType> content = requestBody.Content;
		if (content != null)
		{
			WalkItem("content", content, delegate(OpenApiWalker self, IDictionary<string, IOpenApiMediaType> item)
			{
				self.Walk(item);
			});
		}
		Walk(requestBody as IOpenApiExtensible);
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.OpenApiHeader" />
	/// </summary>
	internal void Walk(IDictionary<string, IOpenApiHeader>? headers)
	{
		if (headers == null)
		{
			return;
		}
		_visitor.Visit(headers);
		foreach (KeyValuePair<string, IOpenApiHeader> header in headers)
		{
			if (header.Value != null)
			{
				_visitor.CurrentKeys.Header = header.Key;
				WalkItem(header.Key, header.Value, delegate(OpenApiWalker self, IOpenApiHeader item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Header = null;
			}
		}
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.IOpenApiCallback" />
	/// </summary>
	internal void Walk(IDictionary<string, IOpenApiCallback>? callbacks)
	{
		if (callbacks == null)
		{
			return;
		}
		_visitor.Visit(callbacks);
		foreach (KeyValuePair<string, IOpenApiCallback> callback in callbacks)
		{
			if (callback.Value != null)
			{
				_visitor.CurrentKeys.Callback = callback.Key;
				WalkItem(callback.Key, callback.Value, delegate(OpenApiWalker self, IOpenApiCallback item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Callback = null;
			}
		}
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.IOpenApiMediaType" />
	/// </summary>
	internal void Walk(IDictionary<string, IOpenApiMediaType>? content)
	{
		if (content == null)
		{
			return;
		}
		_visitor.Visit(content);
		foreach (KeyValuePair<string, IOpenApiMediaType> item in content)
		{
			if (item.Value != null)
			{
				_visitor.CurrentKeys.Content = item.Key;
				WalkItem(item.Key, item.Value, delegate(OpenApiWalker self, IOpenApiMediaType item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Content = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiMediaType" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiMediaType mediaType)
	{
		if (mediaType == null)
		{
			return;
		}
		_visitor.Visit(mediaType);
		WalkDictionary("example", mediaType.Examples, delegate(OpenApiWalker self, IOpenApiExample item, bool isComponent)
		{
			self.Walk(item, isComponent);
		});
		IOpenApiSchema schema = mediaType.Schema;
		if (schema != null)
		{
			WalkItem("schema", schema, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent)
			{
				self.Walk(item, isComponent);
			}, isComponent: false);
		}
		IDictionary<string, OpenApiEncoding> encoding = mediaType.Encoding;
		if (encoding != null)
		{
			WalkItem("encoding", encoding, delegate(OpenApiWalker self, IDictionary<string, OpenApiEncoding> item)
			{
				self.Walk(item);
			});
		}
		Walk(mediaType as IOpenApiExtensible);
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiMediaType" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiMediaType mediaType, bool isComponent = false)
	{
		if (mediaType != null)
		{
			if (mediaType is IOpenApiReferenceHolder referenceableHolder)
			{
				Walk(referenceableHolder);
			}
			else if (mediaType is OpenApiMediaType mediaType2)
			{
				Walk((IOpenApiMediaType)mediaType2);
			}
		}
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.OpenApiEncoding" />
	/// </summary>
	internal void Walk(IDictionary<string, OpenApiEncoding>? encodings)
	{
		if (encodings == null)
		{
			return;
		}
		_visitor.Visit(encodings);
		foreach (KeyValuePair<string, OpenApiEncoding> encoding in encodings)
		{
			if (encoding.Value != null)
			{
				_visitor.CurrentKeys.Encoding = encoding.Key;
				WalkItem(encoding.Key, encoding.Value, delegate(OpenApiWalker self, OpenApiEncoding item)
				{
					self.Walk(item);
				});
				_visitor.CurrentKeys.Encoding = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiEncoding" /> and child objects
	/// </summary>
	internal void Walk(OpenApiEncoding encoding)
	{
		if (encoding != null)
		{
			_visitor.Visit(encoding);
			IDictionary<string, IOpenApiHeader> headers = encoding.Headers;
			if (headers != null)
			{
				Walk(headers);
			}
			Walk((IOpenApiExtensible?)encoding);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiSchema" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiSchema? schema, bool isComponent = false)
	{
		if (schema == null || (schema is IOpenApiReferenceHolder referenceableHolder && ProcessAsReference(referenceableHolder, isComponent)) || _schemaLoop.Contains(schema))
		{
			return;
		}
		_schemaLoop.Push(schema);
		_visitor.Visit(schema);
		IOpenApiSchema items = schema.Items;
		if (items != null)
		{
			WalkItem("items", items, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent2)
			{
				self.Walk(item, isComponent2);
			}, isComponent: false);
		}
		IOpenApiSchema not = schema.Not;
		if (not != null)
		{
			WalkItem("not", not, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent2)
			{
				self.Walk(item, isComponent2);
			}, isComponent: false);
		}
		IList<IOpenApiSchema> allOf = schema.AllOf;
		if (allOf != null && allOf.Count > 0)
		{
			WalkItem("allOf", allOf, delegate(OpenApiWalker self, IList<IOpenApiSchema> item)
			{
				self.Walk(item);
			});
		}
		IList<IOpenApiSchema> anyOf = schema.AnyOf;
		if (anyOf != null && anyOf.Count > 0)
		{
			WalkItem("anyOf", anyOf, delegate(OpenApiWalker self, IList<IOpenApiSchema> item)
			{
				self.Walk(item);
			});
		}
		IList<IOpenApiSchema> oneOf = schema.OneOf;
		if (oneOf != null && oneOf.Count > 0)
		{
			WalkItem("oneOf", oneOf, delegate(OpenApiWalker self, IList<IOpenApiSchema> item)
			{
				self.Walk(item);
			});
		}
		IDictionary<string, IOpenApiSchema> properties = schema.Properties;
		if (properties != null)
		{
			WalkDictionary("properties", properties, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent2)
			{
				self.Walk(item, isComponent2);
			});
		}
		IOpenApiSchema additionalProperties = schema.AdditionalProperties;
		if (additionalProperties != null)
		{
			WalkItem("additionalProperties", additionalProperties, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent2)
			{
				self.Walk(item, isComponent2);
			}, isComponent: false);
		}
		OpenApiDiscriminator discriminator = schema.Discriminator;
		if (discriminator != null)
		{
			WalkItem("discriminator", discriminator, delegate(OpenApiWalker self, OpenApiDiscriminator item)
			{
				self.Walk(item);
			});
		}
		OpenApiExternalDocs externalDocs = schema.ExternalDocs;
		if (externalDocs != null)
		{
			WalkItem("externalDocs", externalDocs, delegate(OpenApiWalker self, OpenApiExternalDocs item)
			{
				self.Walk(item);
			});
		}
		Walk(schema as IOpenApiExtensible);
		_schemaLoop.Pop();
	}

	internal void Walk(OpenApiDiscriminator? openApiDiscriminator)
	{
		if (openApiDiscriminator == null)
		{
			return;
		}
		_visitor.Visit(openApiDiscriminator);
		IDictionary<string, OpenApiSchemaReference> mapping = openApiDiscriminator.Mapping;
		if (mapping != null && mapping.Count > 0)
		{
			WalkDictionary("mapping", mapping, delegate(OpenApiWalker self, OpenApiSchemaReference item, bool _)
			{
				self.Walk(item, isComponent: false);
			});
		}
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.IOpenApiExample" />
	/// </summary>
	internal void Walk(IDictionary<string, IOpenApiExample>? examples)
	{
		if (examples == null)
		{
			return;
		}
		_visitor.Visit(examples);
		foreach (KeyValuePair<string, IOpenApiExample> example in examples)
		{
			if (example.Value != null)
			{
				_visitor.CurrentKeys.Example = example.Key;
				WalkItem(example.Key, example.Value, delegate(OpenApiWalker self, IOpenApiExample item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Example = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.JsonNodeExtension" /> and child objects
	/// </summary>
	internal void Walk(JsonNode? example)
	{
		if (example != null)
		{
			_visitor.Visit(example);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiExample" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiExample example, bool isComponent = false)
	{
		if (example != null)
		{
			if (example is OpenApiExampleReference referenceableHolder)
			{
				Walk((IOpenApiReferenceHolder)referenceableHolder);
				return;
			}
			_visitor.Visit(example);
			Walk(example as IOpenApiExtensible);
		}
	}

	/// <summary>
	/// Visits the list of <see cref="T:Microsoft.OpenApi.IOpenApiExample" /> and child objects
	/// </summary>
	internal void Walk(List<IOpenApiExample> examples)
	{
		if (examples == null)
		{
			return;
		}
		_visitor.Visit(examples);
		for (int i = 0; i < examples.Count; i++)
		{
			IOpenApiExample openApiExample = examples[i];
			if (openApiExample != null)
			{
				WalkItem(i.ToString(), openApiExample, delegate(OpenApiWalker self, IOpenApiExample item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
			}
		}
	}

	/// <summary>
	/// Visits a list of <see cref="T:Microsoft.OpenApi.IOpenApiSchema" /> and child objects
	/// </summary>
	internal void Walk(IList<IOpenApiSchema> schemas)
	{
		if (schemas == null)
		{
			return;
		}
		for (int i = 0; i < schemas.Count; i++)
		{
			IOpenApiSchema openApiSchema = schemas[i];
			if (openApiSchema != null)
			{
				WalkItem(i.ToString(), openApiSchema, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlows" /> and child objects
	/// </summary>
	internal void Walk(OpenApiOAuthFlows flows)
	{
		if (flows != null)
		{
			_visitor.Visit(flows);
			Walk((IOpenApiExtensible?)flows);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlow" /> and child objects
	/// </summary>
	internal void Walk(OpenApiOAuthFlow oAuthFlow)
	{
		if (oAuthFlow != null)
		{
			_visitor.Visit(oAuthFlow);
			Walk((IOpenApiExtensible?)oAuthFlow);
		}
	}

	/// <summary>
	/// Visits dictionary of <see cref="T:Microsoft.OpenApi.IOpenApiLink" /> and child objects
	/// </summary>
	internal void Walk(IDictionary<string, IOpenApiLink>? links)
	{
		if (links == null)
		{
			return;
		}
		_visitor.Visit(links);
		foreach (KeyValuePair<string, IOpenApiLink> link in links)
		{
			if (link.Value != null)
			{
				_visitor.CurrentKeys.Link = link.Key;
				WalkItem(link.Key, link.Value, delegate(OpenApiWalker self, IOpenApiLink item, bool isComponent)
				{
					self.Walk(item, isComponent);
				}, isComponent: false);
				_visitor.CurrentKeys.Link = null;
			}
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiLink" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiLink link, bool isComponent = false)
	{
		if (link == null)
		{
			return;
		}
		if (link is IOpenApiReferenceHolder referenceableHolder)
		{
			Walk(referenceableHolder);
			return;
		}
		_visitor.Visit(link);
		OpenApiServer server = link.Server;
		if (server != null)
		{
			WalkItem("server", server, delegate(OpenApiWalker self, OpenApiServer item)
			{
				self.Walk(item);
			});
		}
		Walk(link as IOpenApiExtensible);
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiHeader" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiHeader header, bool isComponent = false)
	{
		if (header == null)
		{
			return;
		}
		if (header is IOpenApiReferenceHolder referenceableHolder)
		{
			Walk(referenceableHolder);
			return;
		}
		_visitor.Visit(header);
		IDictionary<string, IOpenApiMediaType> content = header.Content;
		if (content != null)
		{
			WalkItem("content", content, delegate(OpenApiWalker self, IDictionary<string, IOpenApiMediaType> item)
			{
				self.Walk(item);
			});
		}
		JsonNode example = header.Example;
		if (example != null)
		{
			WalkItem("example", example, delegate(OpenApiWalker self, JsonNode item)
			{
				self.Walk(item);
			});
		}
		WalkDictionary("examples", header.Examples, delegate(OpenApiWalker self, IOpenApiExample item, bool isComponent2)
		{
			self.Walk(item, isComponent2);
		});
		IOpenApiSchema schema = header.Schema;
		if (schema != null)
		{
			WalkItem("schema", schema, delegate(OpenApiWalker self, IOpenApiSchema item, bool isComponent2)
			{
				self.Walk(item, isComponent2);
			}, isComponent: false);
		}
		Walk(header as IOpenApiExtensible);
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" /> and child objects
	/// </summary>
	internal void Walk(OpenApiSecurityRequirement securityRequirement)
	{
		if (securityRequirement == null)
		{
			return;
		}
		foreach (OpenApiSecuritySchemeReference key in securityRequirement.Keys)
		{
			Walk((IOpenApiReferenceHolder)key);
		}
		_visitor.Visit(securityRequirement);
		Walk(securityRequirement as IOpenApiExtensible);
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiSecurityScheme" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiSecurityScheme securityScheme, bool isComponent = false)
	{
		if (securityScheme != null)
		{
			if (securityScheme is IOpenApiReferenceHolder referenceableHolder)
			{
				Walk(referenceableHolder);
				return;
			}
			_visitor.Visit(securityScheme);
			Walk(securityScheme as IOpenApiExtensible);
		}
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiSecurityScheme" /> and child objects
	/// </summary>
	internal void Walk(IOpenApiReferenceHolder referenceableHolder)
	{
		_visitor.Visit(referenceableHolder);
	}

	/// <summary>
	/// Dispatcher method that enables using a single method to walk the model
	/// starting from any <see cref="T:Microsoft.OpenApi.IOpenApiElement" />
	/// </summary>
	internal void Walk(IOpenApiElement element)
	{
		if (element == null)
		{
			return;
		}
		if (!(element is OpenApiDocument doc))
		{
			if (!(element is OpenApiLicense license))
			{
				if (!(element is OpenApiInfo info))
				{
					if (!(element is OpenApiComponents components))
					{
						if (!(element is OpenApiContact contact))
						{
							if (!(element is IOpenApiCallback callback))
							{
								if (!(element is OpenApiEncoding encoding))
								{
									if (!(element is IOpenApiExample example))
									{
										if (!(element is OpenApiExternalDocs externalDocs))
										{
											if (!(element is IOpenApiHeader header))
											{
												if (!(element is IOpenApiLink link))
												{
													if (!(element is IOpenApiMediaType mediaType))
													{
														if (!(element is OpenApiOAuthFlows flows))
														{
															if (!(element is OpenApiOAuthFlow oAuthFlow))
															{
																if (!(element is OpenApiOperation operation))
																{
																	if (!(element is IOpenApiParameter parameter))
																	{
																		if (!(element is OpenApiPaths paths))
																		{
																			if (!(element is IOpenApiPathItem pathItem))
																			{
																				if (!(element is OpenApiRequestBody openApiExtensible))
																				{
																					if (!(element is IOpenApiResponse response))
																					{
																						if (!(element is IOpenApiSchema schema))
																						{
																							if (!(element is OpenApiDiscriminator openApiDiscriminator))
																							{
																								if (!(element is OpenApiSecurityRequirement securityRequirement))
																								{
																									if (!(element is IOpenApiSecurityScheme securityScheme))
																									{
																										if (!(element is OpenApiServer server))
																										{
																											if (!(element is OpenApiServerVariable serverVariable))
																											{
																												if (!(element is OpenApiTag tag))
																												{
																													if (!(element is IOpenApiExtensible openApiExtensible2))
																													{
																														if (element is IOpenApiExtension extension)
																														{
																															Walk(extension);
																														}
																													}
																													else
																													{
																														Walk(openApiExtensible2);
																													}
																												}
																												else
																												{
																													Walk(tag);
																												}
																											}
																											else
																											{
																												Walk(serverVariable);
																											}
																										}
																										else
																										{
																											Walk(server);
																										}
																									}
																									else
																									{
																										Walk(securityScheme);
																									}
																								}
																								else
																								{
																									Walk(securityRequirement);
																								}
																							}
																							else
																							{
																								Walk(openApiDiscriminator);
																							}
																						}
																						else
																						{
																							Walk(schema);
																						}
																					}
																					else
																					{
																						Walk(response);
																					}
																				}
																				else
																				{
																					Walk((IOpenApiExtensible?)openApiExtensible);
																				}
																			}
																			else
																			{
																				Walk(pathItem);
																			}
																		}
																		else
																		{
																			Walk(paths);
																		}
																	}
																	else
																	{
																		Walk(parameter);
																	}
																}
																else
																{
																	Walk(operation);
																}
															}
															else
															{
																Walk(oAuthFlow);
															}
														}
														else
														{
															Walk(flows);
														}
													}
													else
													{
														Walk(mediaType);
													}
												}
												else
												{
													Walk(link);
												}
											}
											else
											{
												Walk(header);
											}
										}
										else
										{
											Walk(externalDocs);
										}
									}
									else
									{
										Walk(example);
									}
								}
								else
								{
									Walk(encoding);
								}
							}
							else
							{
								Walk(callback);
							}
						}
						else
						{
							Walk(contact);
						}
					}
					else
					{
						Walk(components);
					}
				}
				else
				{
					Walk(info);
				}
			}
			else
			{
				Walk(license);
			}
		}
		else
		{
			Walk(doc);
		}
	}

	private static string ReplaceSlashes(string value)
	{
		return value.Replace("/", "~1", StringComparison.Ordinal);
	}

	/// <summary>
	/// Adds a segment to the context path to enable pointing to the current location in the document
	/// </summary>
	/// <typeparam name="T">The type of the state.</typeparam>
	/// <param name="context">An identifier for the context.</param>
	/// <param name="state">The state to pass to the walk action.</param>
	/// <param name="walk">An action that walks objects within the context.</param>
	private void WalkItem<T>(string context, T state, Action<OpenApiWalker, T> walk)
	{
		_visitor.Enter(ReplaceSlashes(context));
		walk(this, state);
		_visitor.Exit();
	}

	/// <summary>
	/// Adds a segment to the context path to enable pointing to the current location in the document
	/// </summary>
	/// <typeparam name="T">The type of the state.</typeparam>
	/// <param name="context">An identifier for the context.</param>
	/// <param name="state">The state to pass to the walk action.</param>
	/// <param name="isComponent">Whether the state is a component.</param>
	/// <param name="walk">An action that walks objects within the context.</param>
	private void WalkItem<T>(string context, T state, Action<OpenApiWalker, T, bool> walk, bool isComponent)
	{
		_visitor.Enter(ReplaceSlashes(context));
		walk(this, state, isComponent);
		_visitor.Exit();
	}

	/// <summary>
	/// Adds a segment to the context path to enable pointing to the current location in the document
	/// </summary>
	/// <typeparam name="T">The type of the state.</typeparam>
	/// <param name="context">An identifier for the context.</param>
	/// <param name="state">The state to pass to the walk action.</param>
	/// <param name="isComponent">Whether the state is a component.</param>
	/// <param name="walk">An action that walks objects within the context.</param>
	private void WalkDictionary<T>(string context, IDictionary<string, T>? state, Action<OpenApiWalker, T, bool> walk, bool isComponent = false)
	{
		if (state == null || state.Count <= 0)
		{
			return;
		}
		_visitor.Enter(ReplaceSlashes(context));
		foreach (KeyValuePair<string, T> item in state)
		{
			WalkItem(item.Key, (this, item.Value, isComponent, walk), delegate(OpenApiWalker self, (OpenApiWalker, T Value, bool isComponent, Action<OpenApiWalker, T, bool> walk) tuple)
			{
				tuple.walk(self, tuple.Value, tuple.isComponent);
			});
		}
		_visitor.Exit();
	}

	/// <summary>
	/// Identify if an element is just a reference to a component, or an actual component
	/// </summary>
	private bool ProcessAsReference(IOpenApiReferenceHolder referenceableHolder, bool isComponent = false)
	{
		int num;
		if (isComponent)
		{
			num = (referenceableHolder.UnresolvedReference ? 1 : 0);
			if (num == 0)
			{
				goto IL_0016;
			}
		}
		else
		{
			num = 1;
		}
		Walk(referenceableHolder);
		goto IL_0016;
		IL_0016:
		return (byte)num != 0;
	}

	private void WalkTags<T>(ISet<T> tags, Action<OpenApiWalker, T> walk) where T : IOpenApiTag
	{
		if (tags is HashSet<T> { Count: 1 } hashSet)
		{
			T val = hashSet.First();
			if (val != null)
			{
				WalkItem("0", val, walk);
				return;
			}
		}
		int num = 0;
		foreach (T tag in tags)
		{
			if (tag != null)
			{
				string context = num.ToString();
				WalkItem(context, tag, walk);
				num++;
			}
		}
	}
}
