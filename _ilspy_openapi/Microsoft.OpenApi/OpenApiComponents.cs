using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Components Object.
/// </summary>
public class OpenApiComponents : IOpenApiSerializable, IOpenApiElement, IOpenApiExtensible
{
	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiSchema" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiSchema>? Schemas { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiResponse" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiResponse>? Responses { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiParameter" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiParameter>? Parameters { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.OpenApiExample" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiExample>? Examples { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiRequestBody" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiRequestBody>? RequestBodies { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiHeader" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiHeader>? Headers { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiSecurityScheme" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiSecurityScheme>? SecuritySchemes { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiLink" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiLink>? Links { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.OpenApiCallback" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiCallback>? Callbacks { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiPathItem" /> Object.
	/// </summary>
	public IDictionary<string, IOpenApiPathItem>? PathItems { get; set; }

	/// <summary>
	/// An object to hold reusable <see cref="T:Microsoft.OpenApi.IOpenApiMediaType" /> Objects.
	/// </summary>
	public IDictionary<string, IOpenApiMediaType>? MediaTypes { get; set; }

	/// <summary>
	/// This object MAY be extended with Specification Extensions.
	/// </summary>
	public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

	/// <summary>
	/// Parameter-less constructor
	/// </summary>
	public OpenApiComponents()
	{
	}

	/// <summary>
	/// Initializes a copy of an <see cref="T:Microsoft.OpenApi.OpenApiComponents" /> object
	/// </summary>
	public OpenApiComponents(OpenApiComponents? components)
	{
		Schemas = ((components?.Schemas != null) ? new Dictionary<string, IOpenApiSchema>(components.Schemas) : null);
		Responses = ((components?.Responses != null) ? new Dictionary<string, IOpenApiResponse>(components.Responses) : null);
		Parameters = ((components?.Parameters != null) ? new Dictionary<string, IOpenApiParameter>(components.Parameters) : null);
		Examples = ((components?.Examples != null) ? new Dictionary<string, IOpenApiExample>(components.Examples) : null);
		RequestBodies = ((components?.RequestBodies != null) ? new Dictionary<string, IOpenApiRequestBody>(components.RequestBodies) : null);
		Headers = ((components?.Headers != null) ? new Dictionary<string, IOpenApiHeader>(components.Headers) : null);
		SecuritySchemes = ((components?.SecuritySchemes != null) ? new Dictionary<string, IOpenApiSecurityScheme>(components.SecuritySchemes) : null);
		Links = ((components?.Links != null) ? new Dictionary<string, IOpenApiLink>(components.Links) : null);
		Callbacks = ((components?.Callbacks != null) ? new Dictionary<string, IOpenApiCallback>(components.Callbacks) : null);
		PathItems = ((components?.PathItems != null) ? new Dictionary<string, IOpenApiPathItem>(components.PathItems) : null);
		MediaTypes = ((components?.MediaTypes != null) ? new Dictionary<string, IOpenApiMediaType>(components.MediaTypes) : null);
		Extensions = ((components?.Extensions != null) ? new Dictionary<string, IOpenApiExtension>(components.Extensions) : null);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiComponents" /> to Open API v3.2.
	/// </summary>
	/// <param name="writer"></param>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeAsV3X(writer, OpenApiSpecVersion.OpenApi3_2, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV32(writer2);
		}, delegate(IOpenApiWriter writer2, IOpenApiReferenceHolder referenceElement)
		{
			referenceElement.SerializeAsV32(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiComponents" /> to Open API v3.1.
	/// </summary>
	/// <param name="writer"></param>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeAsV3X(writer, OpenApiSpecVersion.OpenApi3_1, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV31(writer2);
		}, delegate(IOpenApiWriter writer2, IOpenApiReferenceHolder referenceElement)
		{
			referenceElement.SerializeAsV31(writer2);
		});
	}

	private void SerializeAsV3X(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback, Action<IOpenApiWriter, IOpenApiReferenceHolder> action)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (writer.GetSettings().InlineLocalReferences)
		{
			RenderComponents(writer, callback, version);
			return;
		}
		writer.WriteStartObject();
		writer.WriteOptionalMap("pathItems", PathItems, delegate(IOpenApiWriter w, string key, IOpenApiPathItem component)
		{
			if (component is OpenApiPathItemReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		SerializeInternal(writer, version, callback, action);
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiComponents" /> to v3.0
	/// </summary>
	/// <param name="writer"></param>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		Utils.CheckArgumentNull(writer, "writer");
		if (writer.GetSettings().InlineLocalReferences)
		{
			RenderComponents(writer, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
			{
				element.SerializeAsV3(writer2);
			}, OpenApiSpecVersion.OpenApi3_0);
			return;
		}
		writer.WriteStartObject();
		SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, delegate(IOpenApiWriter writer2, IOpenApiSerializable element)
		{
			element.SerializeAsV3(writer2);
		}, delegate(IOpenApiWriter writer2, IOpenApiReferenceHolder referenceElement)
		{
			referenceElement.SerializeAsV3(writer2);
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiComponents" />.
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback, Action<IOpenApiWriter, IOpenApiReferenceHolder> action)
	{
		writer.WriteOptionalMap("schemas", Schemas, delegate(IOpenApiWriter w, string key, IOpenApiSchema component)
		{
			if (component is OpenApiSchemaReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("responses", Responses, delegate(IOpenApiWriter w, string key, IOpenApiResponse component)
		{
			if (component is OpenApiResponseReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("parameters", Parameters, delegate(IOpenApiWriter w, string key, IOpenApiParameter component)
		{
			if (component is OpenApiParameterReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("examples", Examples, delegate(IOpenApiWriter w, string key, IOpenApiExample component)
		{
			if (component is OpenApiExampleReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("requestBodies", RequestBodies, delegate(IOpenApiWriter w, string key, IOpenApiRequestBody component)
		{
			if (component is OpenApiRequestBodyReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("headers", Headers, delegate(IOpenApiWriter w, string key, IOpenApiHeader component)
		{
			if (component is OpenApiHeaderReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("securitySchemes", SecuritySchemes, delegate(IOpenApiWriter w, string key, IOpenApiSecurityScheme component)
		{
			if (component is OpenApiSecuritySchemeReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("links", Links, delegate(IOpenApiWriter w, string key, IOpenApiLink component)
		{
			if (component is OpenApiLinkReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		writer.WriteOptionalMap("callbacks", Callbacks, delegate(IOpenApiWriter w, string key, IOpenApiCallback component)
		{
			if (component is OpenApiCallbackReference arg)
			{
				action(w, arg);
			}
			else
			{
				callback(w, component);
			}
		});
		if (MediaTypes != null && version >= OpenApiSpecVersion.OpenApi3_2)
		{
			writer.WriteOptionalMap("mediaTypes", MediaTypes, delegate(IOpenApiWriter w, string key, IOpenApiMediaType component)
			{
				if (component is OpenApiMediaTypeReference arg)
				{
					action(w, arg);
				}
				else
				{
					callback(w, component);
				}
			});
		}
		writer.WriteExtensions(Extensions, version);
		writer.WriteEndObject();
	}

	private void RenderComponents(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback, OpenApiSpecVersion version)
	{
		Dictionary<Type, List<object>> loops = writer.GetSettings().LoopDetector.Loops;
		writer.WriteStartObject();
		if (loops.TryGetValue(typeof(OpenApiSchema), out List<object> _))
		{
			writer.WriteOptionalMap("schemas", Schemas, callback);
		}
		if (SecuritySchemes != null && SecuritySchemes.Any())
		{
			writer.WriteOptionalMap("securitySchemes", SecuritySchemes, delegate(IOpenApiWriter w, string key, IOpenApiSecurityScheme component)
			{
				if (version == OpenApiSpecVersion.OpenApi3_1)
				{
					component.SerializeAsV31(writer);
				}
				component.SerializeAsV3(writer);
			});
		}
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiComponents" /> to Open Api v2.0.
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
	}
}
