using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Microsoft.OpenApi;

/// <summary>
/// Path Item Object: to describe the operations available on a single path.
/// </summary>
public class OpenApiPathItem : IOpenApiExtensible, IOpenApiElement, IOpenApiPathItem, IOpenApiDescribedElement, IOpenApiSummarizedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiPathItem>, IOpenApiReferenceable, IOpenApiSerializable
{
	internal static readonly HashSet<string> _standardHttp2MethodsNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "get", "put", "post", "delete", "options", "head", "patch" };

	internal static readonly HashSet<string> _standardHttp30MethodsNames = new HashSet<string>(_standardHttp2MethodsNames, StringComparer.OrdinalIgnoreCase) { "trace" };

	internal static readonly HashSet<string> _standardHttp31MethodsNames = new HashSet<string>(_standardHttp30MethodsNames, StringComparer.OrdinalIgnoreCase);

	internal static readonly HashSet<string> _standardHttp32MethodsNames = new HashSet<string>(_standardHttp31MethodsNames, StringComparer.OrdinalIgnoreCase) { "query" };

	/// <inheritdoc />
	public string? Summary { get; set; }

	/// <inheritdoc />
	public string? Description { get; set; }

	/// <inheritdoc />
	public Dictionary<HttpMethod, OpenApiOperation>? Operations { get; set; }

	/// <inheritdoc />
	public IList<OpenApiServer>? Servers { get; set; }

	/// <inheritdoc />
	public IList<IOpenApiParameter>? Parameters { get; set; }

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Add one operation into this path item.
	/// </summary>
	/// <param name="operationType">The operation type kind.</param>
	/// <param name="operation">The operation item.</param>
	public void AddOperation(HttpMethod operationType, OpenApiOperation operation)
	{
		if (Operations == null)
		{
			Dictionary<HttpMethod, OpenApiOperation> dictionary = (Operations = new Dictionary<HttpMethod, OpenApiOperation>());
		}
		Operations[operationType] = operation;
	}

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiPathItem()
	{
	}

	/// <summary>
	/// Initializes a clone of an <see cref="T:Microsoft.OpenApi.OpenApiPathItem" /> object
	/// </summary>
	internal OpenApiPathItem(IOpenApiPathItem pathItem)
	{
		Utils.CheckArgumentNull(pathItem, "pathItem");
		Summary = pathItem.Summary ?? Summary;
		Description = pathItem.Description ?? Description;
		Operations = ((pathItem.Operations != null) ? new Dictionary<HttpMethod, OpenApiOperation>(pathItem.Operations) : null);
		IList<OpenApiServer> servers2;
		if (pathItem.Servers != null)
		{
			IList<OpenApiServer>? servers = pathItem.Servers;
			int count = servers.Count;
			List<OpenApiServer> list = new List<OpenApiServer>(count);
			CollectionsMarshal.SetCount(list, count);
			Span<OpenApiServer> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			foreach (OpenApiServer item in servers)
			{
				span[num] = item;
				num++;
			}
			servers2 = list;
		}
		else
		{
			servers2 = null;
		}
		Servers = servers2;
		IList<IOpenApiParameter> parameters2;
		if (pathItem.Parameters != null)
		{
			IList<IOpenApiParameter>? parameters = pathItem.Parameters;
			int num = parameters.Count;
			List<IOpenApiParameter> list2 = new List<IOpenApiParameter>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<IOpenApiParameter> span2 = CollectionsMarshal.AsSpan(list2);
			int count = 0;
			foreach (IOpenApiParameter item2 in parameters)
			{
				span2[count] = item2;
				count++;
			}
			parameters2 = list2;
		}
		else
		{
			parameters2 = null;
		}
		Parameters = parameters2;
		Extensions = ((pathItem.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(pathItem.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiPathItem" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiPathItem" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiPathItem" /> to Open Api v3.0
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	/// <summary>
	/// Serialize inline PathItem in OpenAPI V2
	/// </summary>
	/// <param name="writer"></param>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		if (Operations != null)
		{
			foreach (KeyValuePair<HttpMethod, OpenApiOperation> item in Operations.Where<KeyValuePair<HttpMethod, OpenApiOperation>>((KeyValuePair<HttpMethod, OpenApiOperation> o) => _standardHttp2MethodsNames.Contains<string>(o.Key.Method, StringComparer.OrdinalIgnoreCase)))
			{
				writer.WriteOptionalObject(item.Key.Method.ToLowerInvariant(), item.Value, delegate(IOpenApiWriter w, OpenApiOperation o)
				{
					o.SerializeAsV2(w);
				});
			}
			Dictionary<string, OpenApiOperation> dictionary = Operations.Where<KeyValuePair<HttpMethod, OpenApiOperation>>((KeyValuePair<HttpMethod, OpenApiOperation> o) => !_standardHttp2MethodsNames.Contains<string>(o.Key.Method, StringComparer.OrdinalIgnoreCase)).ToDictionary((KeyValuePair<HttpMethod, OpenApiOperation> o) => o.Key.Method, (KeyValuePair<HttpMethod, OpenApiOperation> o) => o.Value);
			if (dictionary.Count > 0)
			{
				writer.WriteRequiredMap("x-oai-additionalOperations", dictionary, delegate(IOpenApiWriter w, OpenApiOperation o)
				{
					o.SerializeAsV2(w);
				});
			}
		}
		writer.WriteOptionalCollection("parameters", Parameters, delegate(IOpenApiWriter w, IOpenApiParameter p)
		{
			p.SerializeAsV2(w);
		});
		writer.WriteProperty("x-summary", Summary);
		writer.WriteProperty("x-description", Description);
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	internal virtual void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("summary", Summary);
		writer.WriteProperty("description", Description);
		HashSet<string> standardMethodsNames = version switch
		{
			OpenApiSpecVersion.OpenApi3_2 => _standardHttp32MethodsNames, 
			OpenApiSpecVersion.OpenApi3_1 => _standardHttp31MethodsNames, 
			OpenApiSpecVersion.OpenApi3_0 => _standardHttp30MethodsNames, 
			_ => _standardHttp2MethodsNames, 
		};
		if (Operations != null)
		{
			foreach (KeyValuePair<HttpMethod, OpenApiOperation> item in Operations.Where<KeyValuePair<HttpMethod, OpenApiOperation>>((KeyValuePair<HttpMethod, OpenApiOperation> o) => standardMethodsNames.Contains<string>(o.Key.Method, StringComparer.OrdinalIgnoreCase)))
			{
				writer.WriteOptionalObject(item.Key.Method.ToLowerInvariant(), item.Value, callback);
			}
			Dictionary<string, OpenApiOperation> dictionary = Operations.Where<KeyValuePair<HttpMethod, OpenApiOperation>>((KeyValuePair<HttpMethod, OpenApiOperation> o) => !standardMethodsNames.Contains<string>(o.Key.Method, StringComparer.OrdinalIgnoreCase)).ToDictionary((KeyValuePair<HttpMethod, OpenApiOperation> o) => o.Key.Method, (KeyValuePair<HttpMethod, OpenApiOperation> o) => o.Value);
			if (dictionary.Count > 0)
			{
				string text = (((uint)version > 2u) ? "additionalOperations" : "x-oai-additionalOperations");
				string name = text;
				writer.WriteRequiredMap(name, dictionary, delegate(IOpenApiWriter w, OpenApiOperation o)
				{
					o.SerializeAsV32(w);
				});
			}
		}
		writer.WriteOptionalCollection("servers", Servers, callback);
		writer.WriteOptionalCollection("parameters", Parameters, callback);
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public IOpenApiPathItem CreateShallowCopy()
	{
		return new OpenApiPathItem(this);
	}
}
