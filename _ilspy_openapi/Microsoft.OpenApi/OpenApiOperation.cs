using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.OpenApi;

/// <summary>
/// Operation Object.
/// </summary>
public class OpenApiOperation : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible, IMetadataContainer
{
	private ISet<OpenApiTagReference>? _tags;

	/// <summary>
	/// A list of tags for API documentation control.
	/// Tags can be used for logical grouping of operations by resources or any other qualifier.
	/// </summary>
	public ISet<OpenApiTagReference>? Tags
	{
		get
		{
			return _tags;
		}
		set
		{
			if (value != null)
			{
				_tags = ((value is HashSet<OpenApiTagReference> hashSet && hashSet.Comparer is OpenApiTagComparer) ? hashSet : new HashSet<OpenApiTagReference>(value, OpenApiTagComparer.Instance));
			}
		}
	}

	/// <summary>
	/// A short summary of what the operation does.
	/// </summary>
	public string? Summary { get; set; }

	/// <summary>
	/// A verbose explanation of the operation behavior.
	/// CommonMark syntax MAY be used for rich text representation.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Additional external documentation for this operation.
	/// </summary>
	public OpenApiExternalDocs? ExternalDocs { get; set; }

	/// <summary>
	/// Unique string used to identify the operation. The id MUST be unique among all operations described in the API.
	/// Tools and libraries MAY use the operationId to uniquely identify an operation, therefore,
	/// it is RECOMMENDED to follow common programming naming conventions.
	/// </summary>
	public string? OperationId { get; set; }

	/// <summary>
	/// A list of parameters that are applicable for this operation.
	/// If a parameter is already defined at the Path Item, the new definition will override it but can never remove it.
	/// The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a name and location.
	/// The list can use the Reference Object to link to parameters that are defined at the OpenAPI Object's components/parameters.
	/// </summary>
	public IList<IOpenApiParameter>? Parameters { get; set; }

	/// <summary>
	/// The request body applicable for this operation.
	/// The requestBody is only supported in HTTP methods where the HTTP 1.1 specification RFC7231
	/// has explicitly defined semantics for request bodies.
	/// In other cases where the HTTP spec is vague, requestBody SHALL be ignored by consumers.
	/// </summary>
	public IOpenApiRequestBody? RequestBody { get; set; }

	/// <summary>
	/// REQUIRED. The list of possible responses as they are returned from executing this operation.
	/// </summary>
	public OpenApiResponses? Responses { get; set; } = new OpenApiResponses();

	/// <summary>
	/// A map of possible out-of band callbacks related to the parent operation.
	/// The key is a unique identifier for the Callback Object.
	/// Each value in the map is a Callback Object that describes a request
	/// that may be initiated by the API provider and the expected responses.
	/// The key value used to identify the callback object is an expression, evaluated at runtime,
	/// that identifies a URL to use for the callback operation.
	/// </summary>
	public IDictionary<string, IOpenApiCallback>? Callbacks { get; set; }

	/// <summary>
	/// Declares this operation to be deprecated. Consumers SHOULD refrain from usage of the declared operation.
	/// </summary>
	public bool Deprecated { get; set; }

	/// <summary>
	/// A declaration of which security mechanisms can be used for this operation.
	/// The list of values includes alternative security requirement objects that can be used.
	/// Only one of the security requirement objects need to be satisfied to authorize a request.
	/// This definition overrides any declared top-level security.
	/// To remove a top-level security declaration, an empty array can be used.
	/// </summary>
	public IList<OpenApiSecurityRequirement>? Security { get; set; }

	/// <summary>
	/// An alternative server array to service this operation.
	/// If an alternative server object is specified at the Path Item Object or Root level,
	/// it will be overridden by this value.
	/// </summary>
	public IList<OpenApiServer>? Servers { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <inheritdoc />
	public IDictionary<string, object>? Metadata { get; set; }

	/// <summary>
	/// Parameterless constructor
	/// </summary>
	public OpenApiOperation()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiOperation" /> object
	/// </summary>
	public OpenApiOperation(OpenApiOperation operation)
	{
		Utils.CheckArgumentNull(operation, "operation");
		Tags = ((operation.Tags != null) ? new HashSet<OpenApiTagReference>(operation.Tags) : null);
		Summary = operation.Summary ?? Summary;
		Description = operation.Description ?? Description;
		ExternalDocs = ((operation.ExternalDocs != null) ? new OpenApiExternalDocs(operation.ExternalDocs) : null);
		OperationId = operation.OperationId ?? OperationId;
		IList<IOpenApiParameter> parameters2;
		if (operation.Parameters != null)
		{
			IList<IOpenApiParameter>? parameters = operation.Parameters;
			int count = parameters.Count;
			List<IOpenApiParameter> list = new List<IOpenApiParameter>(count);
			CollectionsMarshal.SetCount(list, count);
			Span<IOpenApiParameter> span = CollectionsMarshal.AsSpan(list);
			int num = 0;
			foreach (IOpenApiParameter item in parameters)
			{
				span[num] = item;
				num++;
			}
			parameters2 = list;
		}
		else
		{
			parameters2 = null;
		}
		Parameters = parameters2;
		RequestBody = operation.RequestBody?.CreateShallowCopy();
		Responses = ((operation.Responses != null) ? new OpenApiResponses(operation.Responses) : null);
		Callbacks = ((operation.Callbacks != null) ? new Dictionary<string, IOpenApiCallback>(operation.Callbacks) : null);
		Deprecated = operation.Deprecated;
		IList<OpenApiSecurityRequirement> security2;
		if (operation.Security != null)
		{
			IList<OpenApiSecurityRequirement>? security = operation.Security;
			int num = security.Count;
			List<OpenApiSecurityRequirement> list2 = new List<OpenApiSecurityRequirement>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<OpenApiSecurityRequirement> span2 = CollectionsMarshal.AsSpan(list2);
			int count = 0;
			foreach (OpenApiSecurityRequirement item2 in security)
			{
				span2[count] = item2;
				count++;
			}
			security2 = list2;
		}
		else
		{
			security2 = null;
		}
		Security = security2;
		IList<OpenApiServer> servers2;
		if (operation.Servers != null)
		{
			IList<OpenApiServer>? servers = operation.Servers;
			int count = servers.Count;
			List<OpenApiServer> list3 = new List<OpenApiServer>(count);
			CollectionsMarshal.SetCount(list3, count);
			Span<OpenApiServer> span3 = CollectionsMarshal.AsSpan(list3);
			int num = 0;
			foreach (OpenApiServer item3 in servers)
			{
				span3[num] = item3;
				num++;
			}
			servers2 = list3;
		}
		else
		{
			servers2 = null;
		}
		Servers = servers2;
		Extensions = ((operation.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(operation.Extensions) : null);
		Metadata = ((operation.Metadata != null) ? new Dictionary<string, object>(operation.Metadata) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOperation" /> to Open Api v3.2.
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOperation" /> to Open Api v3.1.
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOperation" /> to Open Api v3.0.
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOperation" /> to Open Api v3.0.
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteOptionalCollection("tags", Tags, callback);
		writer.WriteProperty("summary", Summary);
		writer.WriteProperty("description", Description);
		writer.WriteOptionalObject("externalDocs", ExternalDocs, callback);
		writer.WriteProperty("operationId", OperationId);
		writer.WriteOptionalCollection("parameters", Parameters, callback);
		writer.WriteOptionalObject("requestBody", RequestBody, callback);
		writer.WriteRequiredObject("responses", Responses, callback);
		writer.WriteOptionalMap("callbacks", Callbacks, callback);
		writer.WriteProperty("deprecated", Deprecated);
		writer.WriteOptionalOrEmptyCollection("security", Security, callback);
		writer.WriteOptionalCollection("servers", Servers, callback);
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiOperation" /> to Open Api v2.0.
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteOptionalCollection("tags", Tags, delegate(IOpenApiWriter w, OpenApiTagReference t)
		{
			t.SerializeAsV2(w);
		});
		writer.WriteProperty("summary", Summary);
		writer.WriteProperty("description", Description);
		writer.WriteOptionalObject("externalDocs", ExternalDocs, delegate(IOpenApiWriter w, OpenApiExternalDocs e)
		{
			e.SerializeAsV2(w);
		});
		writer.WriteProperty("operationId", OperationId);
		List<IOpenApiParameter> list = ((Parameters == null) ? new List<IOpenApiParameter>() : new List<IOpenApiParameter>(Parameters));
		HashSet<string> hashSet;
		if (RequestBody != null)
		{
			hashSet = new HashSet<string>(RequestBody.Content?.Keys.Distinct<string>(StringComparer.OrdinalIgnoreCase) ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
			if (hashSet.Count > 0)
			{
				if (hashSet.Contains("application/x-www-form-urlencoded") || hashSet.Contains("multipart/form-data"))
				{
					IEnumerable<IOpenApiParameter> enumerable = RequestBody.ConvertToFormDataParameters(writer);
					if (enumerable != null)
					{
						list.AddRange(enumerable);
						goto IL_0198;
					}
				}
				IOpenApiParameter openApiParameter = RequestBody.ConvertToBodyParameter(writer);
				if (openApiParameter != null)
				{
					list.Add(openApiParameter);
				}
			}
			else if (RequestBody is OpenApiRequestBodyReference openApiRequestBodyReference && openApiRequestBodyReference.Reference.Id != null)
			{
				list.Add(new OpenApiParameterReference(openApiRequestBodyReference.Reference.Id, openApiRequestBodyReference.Reference.HostDocument));
			}
			goto IL_0198;
		}
		goto IL_01ec;
		IL_01ec:
		if (Responses != null)
		{
			string[] array = (from m in Responses.Where<KeyValuePair<string, IOpenApiResponse>>((KeyValuePair<string, IOpenApiResponse> r) => r.Value.Content != null).SelectMany(delegate(KeyValuePair<string, IOpenApiResponse> r)
				{
					IEnumerable<string> enumerable2 = r.Value.Content?.Keys;
					return enumerable2 ?? Enumerable.Empty<string>();
				})
				where !string.IsNullOrEmpty(m)
				select m).Distinct<string>(StringComparer.OrdinalIgnoreCase).ToArray();
			if (array.Length != 0)
			{
				writer.WritePropertyName("produces");
				writer.WriteStartArray();
				string[] array2 = array;
				foreach (string value in array2)
				{
					writer.WriteValue(value);
				}
				writer.WriteEndArray();
			}
		}
		writer.WriteOptionalCollection("parameters", list, delegate(IOpenApiWriter w, IOpenApiParameter p)
		{
			p.SerializeAsV2(w);
		});
		writer.WriteRequiredObject("responses", Responses, delegate(IOpenApiWriter w, OpenApiResponses r)
		{
			r.SerializeAsV2(w);
		});
		if (Servers != null)
		{
			List<string> elements = (from s in Servers.Select(delegate(OpenApiServer s)
				{
					Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out Uri result);
					return result?.Scheme;
				})
				where s != null
				select s).Distinct().ToList();
			writer.WriteOptionalCollection("schemes", elements, delegate(IOpenApiWriter w, string? s)
			{
				if (!string.IsNullOrEmpty(s) && s != null)
				{
					w.WriteValue(s);
				}
			});
		}
		writer.WriteProperty("deprecated", Deprecated);
		writer.WriteOptionalCollection("security", Security, delegate(IOpenApiWriter w, OpenApiSecurityRequirement s)
		{
			s.SerializeAsV2(w);
		});
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
		return;
		IL_0198:
		if (hashSet.Count > 0)
		{
			writer.WritePropertyName("consumes");
			writer.WriteStartArray();
			foreach (string item in hashSet)
			{
				writer.WriteValue(item);
			}
			writer.WriteEndArray();
		}
		goto IL_01ec;
	}
}
