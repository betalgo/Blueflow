using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi;

/// <summary>
/// Describes an OpenAPI object (OpenAPI document). See: https://spec.openapis.org
/// </summary>
public class OpenApiDocument : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible, IMetadataContainer
{
	private ISet<OpenApiTag>? _tags;

	/// <summary>
	/// Related workspace containing components that are referenced in a document
	/// </summary>
	public OpenApiWorkspace? Workspace { get; set; }

	/// <summary>
	/// REQUIRED. Provides metadata about the API. The metadata MAY be used by tooling as required.
	/// </summary>
	public OpenApiInfo Info { get; set; }

	/// <summary>
	/// The default value for the $schema keyword within Schema Objects contained within this OAS document. This MUST be in the form of a URI.
	/// </summary>
	public Uri? JsonSchemaDialect { get; set; }

	/// <summary>
	/// The URI identifying this document. This MUST be in the form of a URI. (OAI 3.2.0+)
	/// </summary>
	public Uri? Self { get; set; }

	/// <summary>
	/// An array of Server Objects, which provide connectivity information to a target server.
	/// </summary>
	public IList<OpenApiServer>? Servers { get; set; } = new List<OpenApiServer>();

	/// <summary>
	/// REQUIRED. The available paths and operations for the API.
	/// </summary>
	public OpenApiPaths Paths { get; set; }

	/// <summary>
	/// The incoming webhooks that MAY be received as part of this API and that the API consumer MAY choose to implement.
	/// A map of requests initiated other than by an API call, for example by an out of band registration. 
	/// The key name is a unique string to refer to each webhook, while the (optionally referenced) Path Item Object describes a request that may be initiated by the API provider and the expected responses
	/// </summary>
	public IDictionary<string, IOpenApiPathItem>? Webhooks { get; set; }

	/// <summary>
	/// An element to hold various schemas for the specification.
	/// </summary>
	public OpenApiComponents? Components { get; set; }

	/// <summary>
	/// A declaration of which security mechanisms can be used across the API.
	/// </summary>
	public IList<OpenApiSecurityRequirement>? Security { get; set; }

	/// <summary>
	/// A list of tags used by the specification with additional metadata.
	/// </summary>
	public ISet<OpenApiTag>? Tags
	{
		get
		{
			return _tags;
		}
		set
		{
			if (value != null)
			{
				_tags = ((value is HashSet<OpenApiTag> hashSet && hashSet.Comparer is OpenApiTagComparer) ? hashSet : new HashSet<OpenApiTag>(value, OpenApiTagComparer.Instance));
			}
		}
	}

	/// <summary>
	/// Additional external documentation.
	/// </summary>
	public OpenApiExternalDocs? ExternalDocs { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <inheritdoc />
	public IDictionary<string, object>? Metadata { get; set; }

	/// <summary>
	/// Absolute location of the document or a generated placeholder if location is not given
	/// </summary>
	public Uri BaseUri { get; internal set; }

	/// <summary>
	/// Register components in the document to the workspace
	/// </summary>
	public void RegisterComponents()
	{
		Workspace?.RegisterComponents(this);
	}

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiDocument()
	{
		Workspace = new OpenApiWorkspace();
		BaseUri = new Uri("https://openapi.net/" + Guid.NewGuid());
		Info = new OpenApiInfo();
		Paths = new OpenApiPaths();
	}

	/// <summary>
	/// Initializes a copy of an an <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> object
	/// </summary>
	public OpenApiDocument(OpenApiDocument? document)
	{
		Workspace = ((document?.Workspace != null) ? new OpenApiWorkspace(document.Workspace) : null);
		Info = ((document?.Info != null) ? new OpenApiInfo(document.Info) : new OpenApiInfo());
		JsonSchemaDialect = document?.JsonSchemaDialect ?? JsonSchemaDialect;
		Self = document?.Self ?? Self;
		IList<OpenApiServer> servers2;
		if (document?.Servers != null)
		{
			IList<OpenApiServer>? servers = document.Servers;
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
		Paths = ((document?.Paths != null) ? new OpenApiPaths(document.Paths) : new OpenApiPaths());
		Webhooks = ((document?.Webhooks != null) ? new Dictionary<string, IOpenApiPathItem>(document.Webhooks) : null);
		Components = ((document?.Components != null) ? new OpenApiComponents(document?.Components) : null);
		IList<OpenApiSecurityRequirement> security2;
		if (document?.Security != null)
		{
			IList<OpenApiSecurityRequirement>? security = document.Security;
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
		Tags = ((document?.Tags != null) ? new HashSet<OpenApiTag>(document.Tags, OpenApiTagComparer.Instance) : null);
		ExternalDocs = ((document?.ExternalDocs != null) ? new OpenApiExternalDocs(document.ExternalDocs) : null);
		Extensions = ((document?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(document.Extensions) : null);
		Metadata = ((document?.Metadata != null) ? new Dictionary<string, object>(document.Metadata) : null);
		BaseUri = ((document?.BaseUri != null) ? document.BaseUri : new Uri("https://openapi.net/" + Guid.NewGuid()));
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> to an Open API document using the specified version.
	/// </summary>
	/// <param name="version">The Open API specification version to serialize the document as.</param>
	/// <param name="writer">The <see cref="T:Microsoft.OpenApi.IOpenApiWriter" /> to serialize the document to.</param>
	/// <exception cref="T:System.ArgumentOutOfRangeException">
	/// <paramref name="version" /> is not a supported Open API specification version.
	/// </exception>
	public void SerializeAs(OpenApiSpecVersion version, IOpenApiWriter writer)
	{
		switch (version)
		{
		case OpenApiSpecVersion.OpenApi2_0:
			SerializeAsV2(writer);
			break;
		case OpenApiSpecVersion.OpenApi3_0:
			SerializeAsV3(writer);
			break;
		case OpenApiSpecVersion.OpenApi3_1:
			SerializeAsV31(writer);
			break;
		case OpenApiSpecVersion.OpenApi3_2:
			SerializeAsV32(writer);
			break;
		default:
			throw new ArgumentOutOfRangeException("version", version, string.Format(SRResource.OpenApiSpecVersionNotSupported, version));
		}
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> to Open API v3.2 document.
	/// </summary>
	/// <param name="writer"></param>
	public void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeAsV3X(writer, "3.2.0", OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV32(w);
		}, delegate(IOpenApiWriter w, OpenApiPathItemReference referenceElement)
		{
			referenceElement.SerializeAsV32(w);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> to Open API v3.1 document.
	/// </summary>
	/// <param name="writer"></param>
	public void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeAsV3X(writer, "3.1.2", OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV31(w);
		}, delegate(IOpenApiWriter w, OpenApiPathItemReference referenceElement)
		{
			referenceElement.SerializeAsV31(w);
		});
	}

	private void SerializeAsV3X(IOpenApiWriter writer, string versionString, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback, Action<IOpenApiWriter, OpenApiPathItemReference> referenceCallback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("openapi", versionString);
		writer.WriteProperty("jsonSchemaDialect", JsonSchemaDialect?.ToString());
		if (version >= OpenApiSpecVersion.OpenApi3_2)
		{
			writer.WriteProperty("$self", Self?.ToString());
		}
		SerializeInternal(writer, version, callback);
		writer.WriteOptionalMap("webhooks", Webhooks, delegate(IOpenApiWriter w, string key, IOpenApiPathItem component)
		{
			if (component is OpenApiPathItemReference arg)
			{
				referenceCallback(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		if (version < OpenApiSpecVersion.OpenApi3_2 && (object)Self != null)
		{
			writer.WriteProperty("x-oai-$self", Self.ToString());
		}
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> to the latest patch of OpenAPI object V3.0.
	/// </summary>
	public void SerializeAsV3(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("openapi", "3.0.4");
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV3(w);
		});
		if ((object)Self != null)
		{
			writer.WriteProperty("x-oai-$self", Self.ToString());
		}
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiDocument" />
	/// </summary>
	/// <param name="writer"></param>
	/// <param name="version"></param>
	/// <param name="callback"></param>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
	{
		writer.WriteRequiredObject("info", Info, callback);
		writer.WriteOptionalCollection("servers", Servers, callback);
		writer.WriteRequiredObject("paths", Paths, callback);
		writer.WriteOptionalObject("components", Components, callback);
		writer.WriteOptionalCollection("security", Security, callback);
		writer.WriteOptionalCollection("tags", Tags, delegate(IOpenApiWriter w, OpenApiTag t)
		{
			callback(w, t);
		});
		writer.WriteOptionalObject("externalDocs", ExternalDocs, callback);
		writer.WriteExtensions(Extensions, version);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> to OpenAPI object V2.0.
	/// </summary>
	public void SerializeAsV2(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		writer.WriteProperty("swagger", "2.0");
		writer.WriteRequiredObject("info", Info, delegate(IOpenApiWriter w, OpenApiInfo i)
		{
			i.SerializeAsV2(w);
		});
		WriteHostInfoV2(writer, Servers);
		writer.WriteRequiredObject("paths", Paths, delegate(IOpenApiWriter w, OpenApiPaths p)
		{
			p.SerializeAsV2(w);
		});
		if (writer.GetSettings().InlineLocalReferences)
		{
			if (writer.GetSettings().LoopDetector.Loops.TryGetValue(typeof(IOpenApiSchema), out List<object> value))
			{
				Dictionary<string, IOpenApiSchema> dictionary = (from k in value.Cast<IOpenApiSchema>().Distinct().OfType<OpenApiSchemaReference>()
					where k.Reference?.Id != null
					select k).ToDictionary((Func<OpenApiSchemaReference, string>)((OpenApiSchemaReference k) => k.Reference.Id), (Func<OpenApiSchemaReference, IOpenApiSchema>)((OpenApiSchemaReference v) => v));
				FindSchemaReferences.ResolveSchemas(Components, dictionary);
				writer.WriteOptionalMap("definitions", dictionary, delegate(IOpenApiWriter w, string _, IOpenApiSchema component)
				{
					component.SerializeAsV2(w);
				});
			}
			return;
		}
		writer.WriteOptionalMap("definitions", Components?.Schemas, delegate(IOpenApiWriter w, string key, IOpenApiSchema component)
		{
			if (component is OpenApiSchemaReference openApiSchemaReference)
			{
				openApiSchemaReference.SerializeAsV2(w);
			}
			else
			{
				component.SerializeAsV2(w);
			}
		});
		Dictionary<string, IOpenApiParameter> parameters = ((Components?.Parameters != null) ? new Dictionary<string, IOpenApiParameter>(Components.Parameters) : new Dictionary<string, IOpenApiParameter>());
		if (Components?.RequestBodies != null)
		{
			foreach (KeyValuePair<string, IOpenApiRequestBody> item in Components.RequestBodies.Where<KeyValuePair<string, IOpenApiRequestBody>>((KeyValuePair<string, IOpenApiRequestBody> b) => !parameters.ContainsKey(b.Key)))
			{
				IOpenApiParameter openApiParameter = item.Value.ConvertToBodyParameter(writer);
				if (openApiParameter != null)
				{
					parameters.Add(item.Key, openApiParameter);
				}
			}
		}
		writer.WriteOptionalMap("parameters", parameters, delegate(IOpenApiWriter w, string key, IOpenApiParameter component)
		{
			if (component is OpenApiParameterReference openApiParameterReference)
			{
				openApiParameterReference.SerializeAsV2(w);
			}
			else
			{
				component.SerializeAsV2(w);
			}
		});
		writer.WriteOptionalMap("responses", Components?.Responses, delegate(IOpenApiWriter w, string key, IOpenApiResponse component)
		{
			if (component is OpenApiResponseReference openApiResponseReference)
			{
				openApiResponseReference.SerializeAsV2(w);
			}
			else
			{
				component.SerializeAsV2(w);
			}
		});
		writer.WriteOptionalMap("securityDefinitions", Components?.SecuritySchemes, delegate(IOpenApiWriter w, string key, IOpenApiSecurityScheme component)
		{
			if (component is OpenApiSecuritySchemeReference openApiSecuritySchemeReference)
			{
				openApiSecuritySchemeReference.SerializeAsV2(w);
			}
			else
			{
				component.SerializeAsV2(w);
			}
		});
		writer.WriteOptionalCollection("security", Security, delegate(IOpenApiWriter w, OpenApiSecurityRequirement s)
		{
			s.SerializeAsV2(w);
		});
		writer.WriteOptionalCollection("tags", Tags, delegate(IOpenApiWriter w, OpenApiTag t)
		{
			t.SerializeAsV2(w);
		});
		writer.WriteOptionalObject("externalDocs", ExternalDocs, delegate(IOpenApiWriter w, OpenApiExternalDocs e)
		{
			e.SerializeAsV2(w);
		});
		writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);
		writer.WriteEndObject();
	}

	private static string? ParseServerUrl(OpenApiServer server)
	{
		return server.ReplaceServerUrlVariables(new Dictionary<string, string>());
	}

	private static void WriteHostInfoV2(IOpenApiWriter writer, IList<OpenApiServer>? servers)
	{
		if (servers == null || !servers.Any())
		{
			return;
		}
		string text = ParseServerUrl(servers[0]);
		if (text == null)
		{
			return;
		}
		Uri firstServerUrl = new Uri(text, UriKind.RelativeOrAbsolute);
		if (firstServerUrl.IsAbsoluteUri)
		{
			writer.WriteProperty("host", firstServerUrl.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));
			if (firstServerUrl.AbsolutePath != "/")
			{
				writer.WriteProperty("basePath", firstServerUrl.AbsolutePath);
			}
		}
		else
		{
			string text2 = firstServerUrl.OriginalString;
			if (text2.StartsWith("//", StringComparison.OrdinalIgnoreCase))
			{
				int num = text2.IndexOf('/', 3);
				writer.WriteProperty("host", text2.Substring(0, num));
				text2 = text2.Substring(num);
			}
			if (!string.IsNullOrEmpty(text2) && text2 != "/")
			{
				writer.WriteProperty("basePath", text2);
			}
		}
		List<string> elements = (from u in servers.Select(delegate(OpenApiServer s)
			{
				Uri.TryCreate(ParseServerUrl(s), UriKind.RelativeOrAbsolute, out Uri result);
				return result;
			})
			where (object)u != null && Uri.Compare(u, firstServerUrl, UriComponents.Host | UriComponents.Port | UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0 && u.IsAbsoluteUri
			select u.Scheme).Distinct().ToList();
		writer.WriteOptionalCollection("schemes", elements, delegate(IOpenApiWriter w, string? s)
		{
			if (!string.IsNullOrEmpty(s) && s != null)
			{
				w.WriteValue(s);
			}
		});
	}

	/// <summary>
	/// Walks the OpenApiDocument and sets the host document for all IOpenApiReferenceable objects
	/// </summary>
	public void SetReferenceHostDocument()
	{
		new OpenApiWalker(new ReferenceHostDocumentSetter(this)).Walk(this);
	}

	/// <summary>
	/// Load the referenced <see cref="T:Microsoft.OpenApi.IOpenApiReferenceable" /> object from a <see cref="T:Microsoft.OpenApi.BaseOpenApiReference" /> object
	/// </summary>
	internal T? ResolveReferenceTo<T>(BaseOpenApiReference reference, IOpenApiSchema? parentSchema) where T : IOpenApiReferenceable
	{
		IOpenApiReferenceable openApiReferenceable = ResolveReference(reference, reference.IsExternal, parentSchema);
		if (openApiReferenceable is T)
		{
			return (T)openApiReferenceable;
		}
		return default(T);
	}

	/// <summary>
	/// Takes in an OpenApi document instance and generates its hash value
	/// </summary>
	/// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
	/// <returns>The hash value.</returns>
	public async Task<string> GetHashCodeAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		using (HashAlgorithm sha = SHA512.Create())
		{
			using CryptoStream cryptoStream = new CryptoStream(Stream.Null, sha, CryptoStreamMode.Write);
			using StreamWriter streamWriter = new StreamWriter(cryptoStream);
			await WriteDocumentAsync(streamWriter, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			cryptoStream.FlushFinalBlock();
			return ConvertByteArrayToString(sha.Hash ?? Array.Empty<byte>());
		}
		async Task WriteDocumentAsync(TextWriter writer, CancellationToken token)
		{
			OpenApiJsonWriter openApiJsonWriter = new OpenApiJsonWriter(writer, new OpenApiJsonWriterSettings
			{
				Terse = true
			});
			SerializeAsV31(openApiJsonWriter);
			await openApiJsonWriter.FlushAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private static string ConvertByteArrayToString(byte[] hash)
	{
		return Convert.ToHexString(hash);
	}

	/// <summary>
	/// Load the referenced <see cref="T:Microsoft.OpenApi.IOpenApiReferenceable" /> object from a <see cref="T:Microsoft.OpenApi.BaseOpenApiReference" /> object
	/// </summary>
	internal IOpenApiReferenceable? ResolveReference(BaseOpenApiReference? reference, bool useExternal, IOpenApiSchema? parentSchema)
	{
		if (reference == null)
		{
			return null;
		}
		string id = reference.Id;
		string text;
		if (!string.IsNullOrEmpty(id) && id.Contains('/'))
		{
			text = id;
		}
		else
		{
			string text2 = ((!string.IsNullOrEmpty(reference.ReferenceV3)) ? reference.ReferenceV3 : string.Empty);
			string text4;
			if (!string.IsNullOrEmpty(text2) && IsSubComponent(text2))
			{
				if (useExternal)
				{
					string text3 = text2.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries)[1];
					text4 = "#" + text3;
				}
				else
				{
					text4 = text2;
				}
			}
			else
			{
				text4 = $"#{"/components/"}{reference.Type.GetDisplayName()}/{id}";
			}
			Uri uri = ((!useExternal) ? null : Workspace?.GetDocumentId(reference.ExternalResource));
			text = ((useExternal && (object)uri != null) ? (uri.AbsoluteUri + text4) : (BaseUri?.ToString() + text4));
		}
		string text5 = ((!text.StartsWith('#')) ? new Uri(text).AbsoluteUri : new Uri(BaseUri, text).AbsoluteUri);
		string text6 = text5;
		if (reference.Type == ReferenceType.Schema && text6.Contains('#') && parentSchema != null)
		{
			return Workspace?.ResolveJsonSchemaReference(text6, parentSchema);
		}
		return Workspace?.ResolveReference<IOpenApiReferenceable>(text6);
	}

	private static bool IsSubComponent(string reference)
	{
		string[] array = reference.Split('#');
		string text = ((array.Length > 1) ? array[1] : string.Empty);
		if (text.StartsWith("/components/schemas/", StringComparison.OrdinalIgnoreCase))
		{
			return text.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length > 3;
		}
		return false;
	}

	/// <summary>
	/// Reads the stream input and parses it into an Open API document.
	/// </summary>
	/// <param name="stream">Stream containing OpenAPI description to parse.</param>
	/// <param name="format">The OpenAPI format to use during parsing.</param>
	/// <param name="settings">The OpenApi reader settings.</param>
	/// <returns></returns>
	public static ReadResult Load(MemoryStream stream, string? format = null, OpenApiReaderSettings? settings = null)
	{
		return OpenApiModelFactory.Load(stream, format, settings);
	}

	/// <summary>
	/// Parses a local file path or Url into an Open API document.
	/// </summary>
	/// <param name="url"> The path to the OpenAPI file.</param>
	/// <param name="settings">The OpenApi reader settings.</param>
	/// <param name="token">The cancellation token</param>
	/// <returns></returns>
	public static async Task<ReadResult> LoadAsync(string url, OpenApiReaderSettings? settings = null, CancellationToken token = default(CancellationToken))
	{
		return await OpenApiModelFactory.LoadAsync(url, settings, token).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// Reads the stream input and parses it into an Open API document.
	/// </summary>
	/// <param name="stream">Stream containing OpenAPI description to parse.</param>
	/// <param name="format">The OpenAPI format to use during parsing.</param>
	/// <param name="settings">The OpenApi reader settings.</param>
	/// <param name="cancellationToken">Propagates information about operation cancelling.</param>
	/// <returns></returns>
	public static async Task<ReadResult> LoadAsync(Stream stream, string? format = null, OpenApiReaderSettings? settings = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return await OpenApiModelFactory.LoadAsync(stream, format, settings, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// Parses a string into a <see cref="T:Microsoft.OpenApi.OpenApiDocument" /> object.
	/// </summary>
	/// <param name="input"> The string input.</param>
	/// <param name="format"></param>
	/// <param name="settings"></param>
	/// <returns></returns>
	public static ReadResult Parse(string input, string? format = null, OpenApiReaderSettings? settings = null)
	{
		return OpenApiModelFactory.Parse(input, format, settings);
	}

	/// <summary>
	/// Adds a component to the components object of the current document and registers it to the underlying workspace.
	/// </summary>
	/// <param name="componentToRegister">The component to add</param>
	/// <param name="id">The id for the component</param>
	/// <typeparam name="T">The type of the component</typeparam>
	/// <returns>Whether the component was added to the components.</returns>
	/// <exception cref="T:System.ArgumentNullException">Thrown when the component is null.</exception>
	/// <exception cref="T:System.ArgumentException">Thrown when the id is null or empty.</exception>
	public bool AddComponent<T>(string id, T componentToRegister)
	{
		Utils.CheckArgumentNull(componentToRegister, "componentToRegister");
		Utils.CheckArgumentNullOrEmpty(id, "id");
		if (Components == null)
		{
			OpenApiComponents openApiComponents = (Components = new OpenApiComponents());
		}
		bool flag = false;
		if (!(componentToRegister is IOpenApiSchema value))
		{
			if (!(componentToRegister is IOpenApiParameter value2))
			{
				if (!(componentToRegister is IOpenApiResponse value3))
				{
					if (!(componentToRegister is IOpenApiRequestBody value4))
					{
						if (!(componentToRegister is IOpenApiLink value5))
						{
							if (!(componentToRegister is IOpenApiCallback value6))
							{
								if (!(componentToRegister is IOpenApiPathItem value7))
								{
									if (!(componentToRegister is IOpenApiExample value8))
									{
										if (!(componentToRegister is IOpenApiHeader value9))
										{
											if (!(componentToRegister is IOpenApiSecurityScheme value10))
											{
												throw new ArgumentException("Component type " + componentToRegister.GetType().Name + " is not supported.");
											}
											OpenApiComponents openApiComponents = Components;
											if (openApiComponents.SecuritySchemes == null)
											{
												IDictionary<string, IOpenApiSecurityScheme> dictionary = (openApiComponents.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>());
											}
											flag = AddToDictionary<IOpenApiSecurityScheme>(Components.SecuritySchemes, id, value10);
										}
										else
										{
											OpenApiComponents openApiComponents = Components;
											if (openApiComponents.Headers == null)
											{
												IDictionary<string, IOpenApiHeader> dictionary3 = (openApiComponents.Headers = new Dictionary<string, IOpenApiHeader>());
											}
											flag = AddToDictionary<IOpenApiHeader>(Components.Headers, id, value9);
										}
									}
									else
									{
										OpenApiComponents openApiComponents = Components;
										if (openApiComponents.Examples == null)
										{
											IDictionary<string, IOpenApiExample> dictionary5 = (openApiComponents.Examples = new Dictionary<string, IOpenApiExample>());
										}
										flag = AddToDictionary<IOpenApiExample>(Components.Examples, id, value8);
									}
								}
								else
								{
									OpenApiComponents openApiComponents = Components;
									if (openApiComponents.PathItems == null)
									{
										IDictionary<string, IOpenApiPathItem> dictionary7 = (openApiComponents.PathItems = new Dictionary<string, IOpenApiPathItem>());
									}
									flag = AddToDictionary<IOpenApiPathItem>(Components.PathItems, id, value7);
								}
							}
							else
							{
								OpenApiComponents openApiComponents = Components;
								if (openApiComponents.Callbacks == null)
								{
									IDictionary<string, IOpenApiCallback> dictionary9 = (openApiComponents.Callbacks = new Dictionary<string, IOpenApiCallback>());
								}
								flag = AddToDictionary<IOpenApiCallback>(Components.Callbacks, id, value6);
							}
						}
						else
						{
							OpenApiComponents openApiComponents = Components;
							if (openApiComponents.Links == null)
							{
								IDictionary<string, IOpenApiLink> dictionary11 = (openApiComponents.Links = new Dictionary<string, IOpenApiLink>());
							}
							flag = AddToDictionary<IOpenApiLink>(Components.Links, id, value5);
						}
					}
					else
					{
						OpenApiComponents openApiComponents = Components;
						if (openApiComponents.RequestBodies == null)
						{
							IDictionary<string, IOpenApiRequestBody> dictionary13 = (openApiComponents.RequestBodies = new Dictionary<string, IOpenApiRequestBody>());
						}
						flag = AddToDictionary<IOpenApiRequestBody>(Components.RequestBodies, id, value4);
					}
				}
				else
				{
					OpenApiComponents openApiComponents = Components;
					if (openApiComponents.Responses == null)
					{
						IDictionary<string, IOpenApiResponse> dictionary15 = (openApiComponents.Responses = new Dictionary<string, IOpenApiResponse>());
					}
					flag = AddToDictionary<IOpenApiResponse>(Components.Responses, id, value3);
				}
			}
			else
			{
				OpenApiComponents openApiComponents = Components;
				if (openApiComponents.Parameters == null)
				{
					IDictionary<string, IOpenApiParameter> dictionary17 = (openApiComponents.Parameters = new Dictionary<string, IOpenApiParameter>());
				}
				flag = AddToDictionary<IOpenApiParameter>(Components.Parameters, id, value2);
			}
		}
		else
		{
			OpenApiComponents openApiComponents = Components;
			if (openApiComponents.Schemas == null)
			{
				IDictionary<string, IOpenApiSchema> dictionary19 = (openApiComponents.Schemas = new Dictionary<string, IOpenApiSchema>());
			}
			flag = AddToDictionary<IOpenApiSchema>(Components.Schemas, id, value);
		}
		if (flag)
		{
			return Workspace?.RegisterComponentForDocument(this, componentToRegister, id) ?? false;
		}
		return false;
		static bool AddToDictionary<TValue>(IDictionary<string, TValue> dict, string key, TValue value11)
		{
			return dict.TryAdd(key, value11);
		}
	}
}
