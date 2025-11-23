using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32;

/// <summary>
/// Class containing logic to deserialize Open API V3 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V3 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V3 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V32 document into
/// runtime Open API object model.
/// </summary>
internal static class OpenApiV32Deserializer
{
	private static readonly FixedFieldMap<OpenApiCallback> _callbackFixedFields = new FixedFieldMap<OpenApiCallback>();

	private static readonly PatternFieldMap<OpenApiCallback> _callbackPatternFields = new PatternFieldMap<OpenApiCallback>
	{
		{
			(string s) => !s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
			delegate(OpenApiCallback o, string p, ParseNode n, OpenApiDocument t)
			{
				o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n, t));
			}
		},
		{
			(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
			delegate(OpenApiCallback o, string p, ParseNode n, OpenApiDocument _)
			{
				o.AddExtension(p, LoadExtension(p, n));
			}
		}
	};

	private static readonly FixedFieldMap<OpenApiComponents> _componentsFixedFields = new FixedFieldMap<OpenApiComponents>
	{
		{
			"schemas",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.Schemas = n.CreateMap(LoadSchema, t);
			}
		},
		{
			"responses",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.Responses = n.CreateMap(LoadResponse, t);
			}
		},
		{
			"parameters",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.Parameters = n.CreateMap(LoadParameter, t);
			}
		},
		{
			"examples",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.Examples = n.CreateMap(LoadExample, t);
			}
		},
		{
			"requestBodies",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.RequestBodies = n.CreateMap(LoadRequestBody, t);
			}
		},
		{
			"headers",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.Headers = n.CreateMap(LoadHeader, t);
			}
		},
		{
			"securitySchemes",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.SecuritySchemes = n.CreateMap(LoadSecurityScheme, t);
			}
		},
		{
			"links",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.Links = n.CreateMap(LoadLink, t);
			}
		},
		{
			"callbacks",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.Callbacks = n.CreateMap(LoadCallback, t);
			}
		},
		{
			"pathItems",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.PathItems = n.CreateMap(LoadPathItem, t);
			}
		},
		{
			"mediaTypes",
			delegate(OpenApiComponents o, ParseNode n, OpenApiDocument t)
			{
				o.MediaTypes = n.CreateMap(LoadMediaType, t);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiComponents> _componentsPatternFields = new PatternFieldMap<OpenApiComponents> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiComponents o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiContact> _contactFixedFields = new FixedFieldMap<OpenApiContact>
	{
		{
			"name",
			delegate(OpenApiContact o, ParseNode n, OpenApiDocument _)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"email",
			delegate(OpenApiContact o, ParseNode n, OpenApiDocument _)
			{
				o.Email = n.GetScalarValue();
			}
		},
		{
			"url",
			delegate(OpenApiContact o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Url = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiContact> _contactPatternFields = new PatternFieldMap<OpenApiContact> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiContact o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiDiscriminator> _discriminatorFixedFields = new FixedFieldMap<OpenApiDiscriminator>
	{
		{
			"propertyName",
			delegate(OpenApiDiscriminator o, ParseNode n, OpenApiDocument _)
			{
				o.PropertyName = n.GetScalarValue();
			}
		},
		{
			"mapping",
			delegate(OpenApiDiscriminator o, ParseNode n, OpenApiDocument doc)
			{
				o.Mapping = n.CreateSimpleMap((ValueNode node) => LoadMapping(node, doc));
			}
		},
		{
			"defaultMapping",
			delegate(OpenApiDiscriminator o, ParseNode n, OpenApiDocument doc)
			{
				o.DefaultMapping = LoadMapping(n, doc);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiDiscriminator> _discriminatorPatternFields = new PatternFieldMap<OpenApiDiscriminator> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiDiscriminator o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiDocument> _openApiFixedFields = new FixedFieldMap<OpenApiDocument>
	{
		{
			"openapi",
			delegate
			{
			}
		},
		{
			"info",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.Info = LoadInfo(n, o);
			}
		},
		{
			"jsonSchemaDialect",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null && Uri.TryCreate(scalarValue, UriKind.Absolute, out Uri result))
				{
					o.JsonSchemaDialect = result;
				}
			}
		},
		{
			"$self",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null && Uri.TryCreate(scalarValue, UriKind.Absolute, out Uri result))
				{
					o.Self = result;
				}
			}
		},
		{
			"servers",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.Servers = n.CreateList(LoadServer, o);
			}
		},
		{
			"paths",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.Paths = LoadPaths(n, o);
			}
		},
		{
			"webhooks",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.Webhooks = n.CreateMap(LoadPathItem, o);
			}
		},
		{
			"components",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.Components = LoadComponents(n, o);
			}
		},
		{
			"tags",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				List<OpenApiTag> list = n.CreateList(LoadTag, o);
				if (list != null && list.Count > 0)
				{
					o.Tags = new HashSet<OpenApiTag>(list, OpenApiTagComparer.Instance);
				}
			}
		},
		{
			"externalDocs",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.ExternalDocs = LoadExternalDocs(n, o);
			}
		},
		{
			"security",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.Security = n.CreateList(LoadSecurityRequirement, o);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiDocument> _openApiPatternFields = new PatternFieldMap<OpenApiDocument> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiDocument o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiEncoding> _encodingFixedFields = new FixedFieldMap<OpenApiEncoding>
	{
		{
			"contentType",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument _)
			{
				o.ContentType = n.GetScalarValue();
			}
		},
		{
			"headers",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument t)
			{
				o.Headers = n.CreateMap(LoadHeader, t);
			}
		},
		{
			"encoding",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument t)
			{
				o.Encoding = n.CreateMap(LoadEncoding, t);
			}
		},
		{
			"itemEncoding",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument t)
			{
				o.ItemEncoding = LoadEncoding(n, t);
			}
		},
		{
			"prefixEncoding",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument t)
			{
				o.PrefixEncoding = n.CreateList(LoadEncoding, t);
			}
		},
		{
			"style",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(n.Context, out var result))
				{
					o.Style = result;
				}
			}
		},
		{
			"explode",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Explode = bool.Parse(scalarValue);
				}
			}
		},
		{
			"allowReserved",
			delegate(OpenApiEncoding o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.AllowReserved = bool.Parse(scalarValue);
				}
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiEncoding> _encodingPatternFields = new PatternFieldMap<OpenApiEncoding> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiEncoding o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiExample> _exampleFixedFields = new FixedFieldMap<OpenApiExample>
	{
		{
			"summary",
			delegate(OpenApiExample o, ParseNode n, OpenApiDocument _)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiExample o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"value",
			delegate(OpenApiExample o, ParseNode n, OpenApiDocument _)
			{
				o.Value = n.CreateAny();
			}
		},
		{
			"externalValue",
			delegate(OpenApiExample o, ParseNode n, OpenApiDocument _)
			{
				o.ExternalValue = n.GetScalarValue();
			}
		},
		{
			"dataValue",
			delegate(OpenApiExample o, ParseNode n, OpenApiDocument _)
			{
				o.DataValue = n.CreateAny();
			}
		},
		{
			"serializedValue",
			delegate(OpenApiExample o, ParseNode n, OpenApiDocument _)
			{
				o.SerializedValue = n.GetScalarValue();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiExample> _examplePatternFields = new PatternFieldMap<OpenApiExample> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiExample o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields = new FixedFieldMap<OpenApiExternalDocs>
	{
		{
			"description",
			delegate(OpenApiExternalDocs o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"url",
			delegate(OpenApiExternalDocs o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Url = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiExternalDocs> _externalDocsPatternFields = new PatternFieldMap<OpenApiExternalDocs> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiExternalDocs o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new FixedFieldMap<OpenApiHeader>
	{
		{
			"description",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"required",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Required = bool.Parse(scalarValue);
				}
			}
		},
		{
			"deprecated",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Deprecated = bool.Parse(scalarValue);
				}
			}
		},
		{
			"allowEmptyValue",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.AllowEmptyValue = bool.Parse(scalarValue);
				}
			}
		},
		{
			"allowReserved",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.AllowReserved = bool.Parse(scalarValue);
				}
			}
		},
		{
			"style",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(n.Context, out var result))
				{
					o.Style = result;
				}
			}
		},
		{
			"explode",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Explode = bool.Parse(scalarValue);
				}
			}
		},
		{
			"schema",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument t)
			{
				o.Schema = LoadSchema(n, t);
			}
		},
		{
			"content",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument t)
			{
				o.Content = n.CreateMap(LoadMediaType, t);
			}
		},
		{
			"examples",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument t)
			{
				o.Examples = n.CreateMap(LoadExample, t);
			}
		},
		{
			"example",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				o.Example = n.CreateAny();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new PatternFieldMap<OpenApiHeader> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiHeader o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	public static readonly FixedFieldMap<OpenApiInfo> InfoFixedFields = new FixedFieldMap<OpenApiInfo>
	{
		{
			"title",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument _)
			{
				o.Title = n.GetScalarValue();
			}
		},
		{
			"version",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument _)
			{
				o.Version = n.GetScalarValue();
			}
		},
		{
			"summary",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument _)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"termsOfService",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.TermsOfService = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"contact",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument t)
			{
				o.Contact = LoadContact(n, t);
			}
		},
		{
			"license",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument t)
			{
				o.License = LoadLicense(n, t);
			}
		}
	};

	public static readonly PatternFieldMap<OpenApiInfo> InfoPatternFields = new PatternFieldMap<OpenApiInfo> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiInfo o, string k, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(k, LoadExtension(k, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiLicense> _licenseFixedFields = new FixedFieldMap<OpenApiLicense>
	{
		{
			"name",
			delegate(OpenApiLicense o, ParseNode n, OpenApiDocument _)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"identifier",
			delegate(OpenApiLicense o, ParseNode n, OpenApiDocument _)
			{
				o.Identifier = n.GetScalarValue();
			}
		},
		{
			"url",
			delegate(OpenApiLicense o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Url = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiLicense> _licensePatternFields = new PatternFieldMap<OpenApiLicense> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiLicense o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiLink> _linkFixedFields = new FixedFieldMap<OpenApiLink>
	{
		{
			"operationRef",
			delegate(OpenApiLink o, ParseNode n, OpenApiDocument _)
			{
				o.OperationRef = n.GetScalarValue();
			}
		},
		{
			"operationId",
			delegate(OpenApiLink o, ParseNode n, OpenApiDocument _)
			{
				o.OperationId = n.GetScalarValue();
			}
		},
		{
			"parameters",
			delegate(OpenApiLink o, ParseNode n, OpenApiDocument _)
			{
				o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper);
			}
		},
		{
			"requestBody",
			delegate(OpenApiLink o, ParseNode n, OpenApiDocument _)
			{
				o.RequestBody = LoadRuntimeExpressionAnyWrapper(n);
			}
		},
		{
			"description",
			delegate(OpenApiLink o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"server",
			delegate(OpenApiLink o, ParseNode n, OpenApiDocument t)
			{
				o.Server = LoadServer(n, t);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiLink> _linkPatternFields = new PatternFieldMap<OpenApiLink> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiLink o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiMediaType> _mediaTypeFixedFields = new FixedFieldMap<OpenApiMediaType>
	{
		{
			"schema",
			delegate(OpenApiMediaType o, ParseNode n, OpenApiDocument t)
			{
				o.Schema = LoadSchema(n, t);
			}
		},
		{
			"itemSchema",
			delegate(OpenApiMediaType o, ParseNode n, OpenApiDocument t)
			{
				o.ItemSchema = LoadSchema(n, t);
			}
		},
		{
			"examples",
			delegate(OpenApiMediaType o, ParseNode n, OpenApiDocument t)
			{
				o.Examples = n.CreateMap(LoadExample, t);
			}
		},
		{
			"example",
			delegate(OpenApiMediaType o, ParseNode n, OpenApiDocument _)
			{
				o.Example = n.CreateAny();
			}
		},
		{
			"encoding",
			delegate(OpenApiMediaType o, ParseNode n, OpenApiDocument t)
			{
				o.Encoding = n.CreateMap(LoadEncoding, t);
			}
		},
		{
			"itemEncoding",
			delegate(OpenApiMediaType o, ParseNode n, OpenApiDocument t)
			{
				o.ItemEncoding = LoadEncoding(n, t);
			}
		},
		{
			"prefixEncoding",
			delegate(OpenApiMediaType o, ParseNode n, OpenApiDocument t)
			{
				o.PrefixEncoding = n.CreateList(LoadEncoding, t);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiMediaType> _mediaTypePatternFields = new PatternFieldMap<OpenApiMediaType> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiMediaType o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields = new AnyFieldMap<OpenApiMediaType> { 
	{
		"example",
		new AnyFieldMapParameter<OpenApiMediaType>((OpenApiMediaType s) => s.Example, delegate(OpenApiMediaType s, JsonNode? v)
		{
			s.Example = v;
		}, (OpenApiMediaType s) => s.Schema)
	} };

	private static readonly AnyMapFieldMap<OpenApiMediaType, IOpenApiExample> _mediaTypeAnyMapOpenApiExampleFields = new AnyMapFieldMap<OpenApiMediaType, IOpenApiExample> { 
	{
		"examples",
		new AnyMapFieldMapParameter<OpenApiMediaType, IOpenApiExample>((OpenApiMediaType m) => m.Examples, (IOpenApiExample e) => e.Value, delegate(IOpenApiExample e, JsonNode v)
		{
			if (e is OpenApiExample openApiExample)
			{
				openApiExample.Value = v;
			}
		}, (OpenApiMediaType m) => m.Schema)
	} };

	private static readonly FixedFieldMap<OpenApiOAuthFlow> _oAuthFlowFixedFileds = new FixedFieldMap<OpenApiOAuthFlow>
	{
		{
			"authorizationUrl",
			delegate(OpenApiOAuthFlow o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.AuthorizationUrl = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"tokenUrl",
			delegate(OpenApiOAuthFlow o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.TokenUrl = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"refreshUrl",
			delegate(OpenApiOAuthFlow o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.RefreshUrl = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"deviceAuthorizationUrl",
			delegate(OpenApiOAuthFlow o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.DeviceAuthorizationUrl = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"scopes",
			delegate(OpenApiOAuthFlow o, ParseNode n, OpenApiDocument _)
			{
				o.Scopes = (from kv in n.CreateSimpleMap(LoadString)
					where kv.Value != null
					select kv).ToDictionary((KeyValuePair<string, string> kv) => kv.Key, (KeyValuePair<string, string> kv) => kv.Value);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiOAuthFlow> _oAuthFlowPatternFields = new PatternFieldMap<OpenApiOAuthFlow> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiOAuthFlow o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiOAuthFlows> _oAuthFlowsFixedFields = new FixedFieldMap<OpenApiOAuthFlows>
	{
		{
			"implicit",
			delegate(OpenApiOAuthFlows o, ParseNode n, OpenApiDocument t)
			{
				o.Implicit = LoadOAuthFlow(n, t);
			}
		},
		{
			"password",
			delegate(OpenApiOAuthFlows o, ParseNode n, OpenApiDocument t)
			{
				o.Password = LoadOAuthFlow(n, t);
			}
		},
		{
			"clientCredentials",
			delegate(OpenApiOAuthFlows o, ParseNode n, OpenApiDocument t)
			{
				o.ClientCredentials = LoadOAuthFlow(n, t);
			}
		},
		{
			"authorizationCode",
			delegate(OpenApiOAuthFlows o, ParseNode n, OpenApiDocument t)
			{
				o.AuthorizationCode = LoadOAuthFlow(n, t);
			}
		},
		{
			"deviceAuthorization",
			delegate(OpenApiOAuthFlows o, ParseNode n, OpenApiDocument t)
			{
				o.DeviceAuthorization = LoadOAuthFlow(n, t);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiOAuthFlows> _oAuthFlowsPatternFields = new PatternFieldMap<OpenApiOAuthFlows> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiOAuthFlows o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields = new FixedFieldMap<OpenApiOperation>
	{
		{
			"tags",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument doc)
			{
				List<OpenApiTagReference> list = n.CreateSimpleList(delegate(ValueNode valueNode, OpenApiDocument? hostDocument)
				{
					string scalarValue = valueNode.GetScalarValue();
					return string.IsNullOrEmpty(scalarValue) ? null : LoadTagByReference(scalarValue, hostDocument);
				}, doc).OfType<OpenApiTagReference>().ToList();
				if (list != null && list.Count > 0)
				{
					o.Tags = new HashSet<OpenApiTagReference>(list, OpenApiTagComparer.Instance);
				}
			}
		},
		{
			"summary",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument _)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"externalDocs",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument t)
			{
				o.ExternalDocs = LoadExternalDocs(n, t);
			}
		},
		{
			"operationId",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument _)
			{
				o.OperationId = n.GetScalarValue();
			}
		},
		{
			"parameters",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument t)
			{
				o.Parameters = n.CreateList(LoadParameter, t);
			}
		},
		{
			"requestBody",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument t)
			{
				o.RequestBody = LoadRequestBody(n, t);
			}
		},
		{
			"responses",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument t)
			{
				o.Responses = LoadResponses(n, t);
			}
		},
		{
			"callbacks",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument t)
			{
				o.Callbacks = n.CreateMap(LoadCallback, t);
			}
		},
		{
			"deprecated",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Deprecated = bool.Parse(scalarValue);
				}
			}
		},
		{
			"security",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument t)
			{
				if (n.JsonNode is JsonArray)
				{
					o.Security = n.CreateList(LoadSecurityRequirement, t);
				}
			}
		},
		{
			"servers",
			delegate(OpenApiOperation o, ParseNode n, OpenApiDocument t)
			{
				o.Servers = n.CreateList(LoadServer, t);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields = new PatternFieldMap<OpenApiOperation> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiOperation o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields = new FixedFieldMap<OpenApiParameter>
	{
		{
			"name",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument _)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"in",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(n.Context, out var result))
				{
					o.In = result;
				}
			}
		},
		{
			"description",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"required",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Required = bool.Parse(scalarValue);
				}
			}
		},
		{
			"deprecated",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Deprecated = bool.Parse(scalarValue);
				}
			}
		},
		{
			"allowEmptyValue",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.AllowEmptyValue = bool.Parse(scalarValue);
				}
			}
		},
		{
			"allowReserved",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.AllowReserved = bool.Parse(scalarValue);
				}
			}
		},
		{
			"style",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(n.Context, out var result))
				{
					o.Style = result;
				}
			}
		},
		{
			"explode",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Explode = bool.Parse(scalarValue);
				}
			}
		},
		{
			"schema",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				o.Schema = LoadSchema(n, t);
			}
		},
		{
			"content",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				o.Content = n.CreateMap(LoadMediaType, t);
			}
		},
		{
			"examples",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				o.Examples = n.CreateMap(LoadExample, t);
			}
		},
		{
			"example",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument _)
			{
				o.Example = n.CreateAny();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields = new PatternFieldMap<OpenApiParameter> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiParameter o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields = new AnyFieldMap<OpenApiParameter> { 
	{
		"example",
		new AnyFieldMapParameter<OpenApiParameter>((OpenApiParameter s) => s.Example, delegate(OpenApiParameter s, JsonNode? v)
		{
			s.Example = v;
		}, (OpenApiParameter s) => s.Schema)
	} };

	private static readonly AnyMapFieldMap<OpenApiParameter, IOpenApiExample> _parameterAnyMapOpenApiExampleFields = new AnyMapFieldMap<OpenApiParameter, IOpenApiExample> { 
	{
		"examples",
		new AnyMapFieldMapParameter<OpenApiParameter, IOpenApiExample>((OpenApiParameter m) => m.Examples, (IOpenApiExample e) => e.Value, delegate(IOpenApiExample e, JsonNode v)
		{
			if (e is OpenApiExample openApiExample)
			{
				openApiExample.Value = v;
			}
		}, (OpenApiParameter m) => m.Schema)
	} };

	private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new FixedFieldMap<OpenApiPathItem>
	{
		{
			"summary",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument _)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"get",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(HttpMethod.Get, LoadOperation(n, t));
			}
		},
		{
			"put",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(HttpMethod.Put, LoadOperation(n, t));
			}
		},
		{
			"post",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(HttpMethod.Post, LoadOperation(n, t));
			}
		},
		{
			"delete",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(HttpMethod.Delete, LoadOperation(n, t));
			}
		},
		{
			"options",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(HttpMethod.Options, LoadOperation(n, t));
			}
		},
		{
			"head",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(HttpMethod.Head, LoadOperation(n, t));
			}
		},
		{
			"patch",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(new HttpMethod("PATCH"), LoadOperation(n, t));
			}
		},
		{
			"query",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(new HttpMethod("QUERY"), LoadOperation(n, t));
			}
		},
		{
			"trace",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.AddOperation(HttpMethod.Trace, LoadOperation(n, t));
			}
		},
		{
			"servers",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.Servers = n.CreateList(LoadServer, t);
			}
		},
		{
			"parameters",
			delegate(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
			{
				o.Parameters = n.CreateList(LoadParameter, t);
			}
		},
		{ "additionalOperations", LoadAdditionalOperations }
	};

	private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields = new PatternFieldMap<OpenApiPathItem> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiPathItem o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiPaths> _pathsFixedFields = new FixedFieldMap<OpenApiPaths>();

	private static readonly PatternFieldMap<OpenApiPaths> _pathsPatternFields = new PatternFieldMap<OpenApiPaths>
	{
		{
			(string s) => s.StartsWith("/", StringComparison.OrdinalIgnoreCase),
			delegate(OpenApiPaths o, string k, ParseNode n, OpenApiDocument t)
			{
				o.Add(k, LoadPathItem(n, t));
			}
		},
		{
			(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
			delegate(OpenApiPaths o, string p, ParseNode n, OpenApiDocument _)
			{
				o.AddExtension(p, LoadExtension(p, n));
			}
		}
	};

	private static readonly FixedFieldMap<OpenApiRequestBody> _requestBodyFixedFields = new FixedFieldMap<OpenApiRequestBody>
	{
		{
			"description",
			delegate(OpenApiRequestBody o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"content",
			delegate(OpenApiRequestBody o, ParseNode n, OpenApiDocument t)
			{
				o.Content = n.CreateMap(LoadMediaType, t);
			}
		},
		{
			"required",
			delegate(OpenApiRequestBody o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Required = bool.Parse(scalarValue);
				}
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiRequestBody> _requestBodyPatternFields = new PatternFieldMap<OpenApiRequestBody> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiRequestBody o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new FixedFieldMap<OpenApiResponse>
	{
		{
			"summary",
			delegate(OpenApiResponse o, ParseNode n, OpenApiDocument _)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiResponse o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"headers",
			delegate(OpenApiResponse o, ParseNode n, OpenApiDocument t)
			{
				o.Headers = n.CreateMap(LoadHeader, t);
			}
		},
		{
			"content",
			delegate(OpenApiResponse o, ParseNode n, OpenApiDocument t)
			{
				o.Content = n.CreateMap(LoadMediaType, t);
			}
		},
		{
			"links",
			delegate(OpenApiResponse o, ParseNode n, OpenApiDocument t)
			{
				o.Links = n.CreateMap(LoadLink, t);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields = new PatternFieldMap<OpenApiResponse> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiResponse o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	public static readonly FixedFieldMap<OpenApiResponses> ResponsesFixedFields = new FixedFieldMap<OpenApiResponses>();

	public static readonly PatternFieldMap<OpenApiResponses> ResponsesPatternFields = new PatternFieldMap<OpenApiResponses>
	{
		{
			(string s) => !s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
			delegate(OpenApiResponses o, string p, ParseNode n, OpenApiDocument t)
			{
				o.Add(p, LoadResponse(n, t));
			}
		},
		{
			(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
			delegate(OpenApiResponses o, string p, ParseNode n, OpenApiDocument _)
			{
				o.AddExtension(p, LoadExtension(p, n));
			}
		}
	};

	private static readonly FixedFieldMap<OpenApiSchema> _openApiSchemaFixedFields = new FixedFieldMap<OpenApiSchema>
	{
		{
			"title",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Title = n.GetScalarValue();
			}
		},
		{
			"$schema",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null && Uri.TryCreate(scalarValue, UriKind.Absolute, out Uri result))
				{
					o.Schema = result;
				}
			}
		},
		{
			"$id",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Id = n.GetScalarValue();
			}
		},
		{
			"$comment",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Comment = n.GetScalarValue();
			}
		},
		{
			"$vocabulary",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Vocabulary = n.CreateSimpleMap(LoadBool).ToDictionary<KeyValuePair<string, bool?>, string, bool>((KeyValuePair<string, bool?> kvp) => kvp.Key, (KeyValuePair<string, bool?> kvp) => kvp.Value == true);
			}
		},
		{
			"$dynamicRef",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.DynamicRef = n.GetScalarValue();
			}
		},
		{
			"$dynamicAnchor",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.DynamicAnchor = n.GetScalarValue();
			}
		},
		{
			"$defs",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument t)
			{
				o.Definitions = n.CreateMap(LoadSchema, t);
			}
		},
		{
			"multipleOf",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.MultipleOf = decimal.Parse(scalarValue, NumberStyles.Float, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"maximum",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (!string.IsNullOrEmpty(scalarValue))
				{
					o.Maximum = scalarValue;
				}
			}
		},
		{
			"exclusiveMaximum",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.ExclusiveMaximum = n.GetScalarValue();
			}
		},
		{
			"minimum",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (!string.IsNullOrEmpty(scalarValue))
				{
					o.Minimum = scalarValue;
				}
			}
		},
		{
			"exclusiveMinimum",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.ExclusiveMinimum = n.GetScalarValue();
			}
		},
		{
			"maxLength",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.MaxLength = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"minLength",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.MinLength = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"pattern",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Pattern = n.GetScalarValue();
			}
		},
		{
			"maxItems",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.MaxItems = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"minItems",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.MinItems = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"uniqueItems",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.UniqueItems = bool.Parse(scalarValue);
				}
			}
		},
		{
			"unevaluatedProperties",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.UnevaluatedProperties = bool.Parse(scalarValue);
				}
			}
		},
		{
			"maxProperties",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.MaxProperties = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"minProperties",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.MinProperties = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"required",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				o.Required = new HashSet<string>(from s in n.CreateSimpleList((ValueNode valueNode, OpenApiDocument? p) => valueNode.GetScalarValue(), doc)
					where s != null
					select s);
			}
		},
		{
			"enum",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Enum = n.CreateListOfAny();
			}
		},
		{
			"type",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				if (n is ValueNode)
				{
					o.Type = n.GetScalarValue()?.ToJsonSchemaType();
				}
				else
				{
					List<string> source = n.CreateSimpleList((ValueNode valueNode, OpenApiDocument? p) => valueNode.GetScalarValue(), doc);
					JsonSchemaType jsonSchemaType = (JsonSchemaType)0;
					foreach (JsonSchemaType item in from t in source
						where t != null
						select t.ToJsonSchemaType())
					{
						jsonSchemaType |= item;
					}
					o.Type = jsonSchemaType;
				}
			}
		},
		{
			"const",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Const = n.GetScalarValue();
			}
		},
		{
			"allOf",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument t)
			{
				o.AllOf = n.CreateList(LoadSchema, t);
			}
		},
		{
			"oneOf",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument t)
			{
				o.OneOf = n.CreateList(LoadSchema, t);
			}
		},
		{
			"anyOf",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument t)
			{
				o.AnyOf = n.CreateList(LoadSchema, t);
			}
		},
		{
			"not",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				o.Not = LoadSchema(n, doc);
			}
		},
		{
			"items",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				o.Items = LoadSchema(n, doc);
			}
		},
		{
			"properties",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument t)
			{
				o.Properties = n.CreateMap(LoadSchema, t);
			}
		},
		{
			"patternProperties",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument t)
			{
				o.PatternProperties = n.CreateMap(LoadSchema, t);
			}
		},
		{
			"additionalProperties",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				if (n is ValueNode)
				{
					string scalarValue = n.GetScalarValue();
					if (scalarValue != null)
					{
						o.AdditionalPropertiesAllowed = bool.Parse(scalarValue);
					}
				}
				else
				{
					o.AdditionalProperties = LoadSchema(n, doc);
				}
			}
		},
		{
			"description",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"format",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Format = n.GetScalarValue();
			}
		},
		{
			"default",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Default = n.CreateAny();
			}
		},
		{
			"nullable",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null && bool.Parse(scalarValue))
				{
					if (o.Type.HasValue)
					{
						o.Type |= JsonSchemaType.Null;
					}
					else
					{
						o.Type = JsonSchemaType.Null;
					}
				}
			}
		},
		{
			"discriminator",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				o.Discriminator = LoadDiscriminator(n, doc);
			}
		},
		{
			"readOnly",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.ReadOnly = bool.Parse(scalarValue);
				}
			}
		},
		{
			"writeOnly",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.WriteOnly = bool.Parse(scalarValue);
				}
			}
		},
		{
			"xml",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				o.Xml = LoadXml(n, doc);
			}
		},
		{
			"externalDocs",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				o.ExternalDocs = LoadExternalDocs(n, doc);
			}
		},
		{
			"example",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Example = n.CreateAny();
			}
		},
		{
			"examples",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Examples = n.CreateListOfAny();
			}
		},
		{
			"deprecated",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Deprecated = bool.Parse(scalarValue);
				}
			}
		},
		{
			"dependentRequired",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument doc)
			{
				o.DependentRequired = n.CreateArrayMap((ValueNode valueNode, OpenApiDocument? p) => valueNode.GetScalarValue(), doc);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiSchema> _openApiSchemaPatternFields = new PatternFieldMap<OpenApiSchema> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiSchema o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields = new FixedFieldMap<OpenApiSecurityScheme>
	{
		{
			"type",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue().TryGetEnumFromDisplayName<SecuritySchemeType>(n.Context, out var result))
				{
					o.Type = result;
				}
			}
		},
		{
			"description",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"name",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"in",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(n.Context, out var result))
				{
					o.In = result;
				}
			}
		},
		{
			"scheme",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				o.Scheme = n.GetScalarValue();
			}
		},
		{
			"bearerFormat",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				o.BearerFormat = n.GetScalarValue();
			}
		},
		{
			"openIdConnectUrl",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.OpenIdConnectUrl = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"flows",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument t)
			{
				o.Flows = LoadOAuthFlows(n, t);
			}
		},
		{
			"deprecated",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Deprecated = bool.Parse(scalarValue);
				}
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields = new PatternFieldMap<OpenApiSecurityScheme> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiSecurityScheme o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiServer> _serverFixedFields = new FixedFieldMap<OpenApiServer>
	{
		{
			"url",
			delegate(OpenApiServer o, ParseNode n, OpenApiDocument _)
			{
				o.Url = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiServer o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"name",
			delegate(OpenApiServer o, ParseNode n, OpenApiDocument _)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"variables",
			delegate(OpenApiServer o, ParseNode n, OpenApiDocument t)
			{
				o.Variables = n.CreateMap(LoadServerVariable, t);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiServer> _serverPatternFields = new PatternFieldMap<OpenApiServer> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiServer o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiServerVariable> _serverVariableFixedFields = new FixedFieldMap<OpenApiServerVariable>
	{
		{
			"enum",
			delegate(OpenApiServerVariable o, ParseNode n, OpenApiDocument doc)
			{
				o.Enum = n.CreateSimpleList((ValueNode s, OpenApiDocument? p) => s.GetScalarValue(), doc).OfType<string>().ToList();
			}
		},
		{
			"default",
			delegate(OpenApiServerVariable o, ParseNode n, OpenApiDocument _)
			{
				o.Default = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiServerVariable o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiServerVariable> _serverVariablePatternFields = new PatternFieldMap<OpenApiServerVariable> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiServerVariable o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiTag> _tagFixedFields = new FixedFieldMap<OpenApiTag>
	{
		{
			"name",
			delegate(OpenApiTag o, ParseNode n, OpenApiDocument _)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiTag o, ParseNode n, OpenApiDocument _)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"externalDocs",
			delegate(OpenApiTag o, ParseNode n, OpenApiDocument t)
			{
				o.ExternalDocs = LoadExternalDocs(n, t);
			}
		},
		{
			"summary",
			delegate(OpenApiTag o, ParseNode n, OpenApiDocument _)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"parent",
			delegate(OpenApiTag o, ParseNode n, OpenApiDocument doc)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Parent = LoadTagByReference(scalarValue, doc);
				}
			}
		},
		{
			"kind",
			delegate(OpenApiTag o, ParseNode n, OpenApiDocument _)
			{
				o.Kind = n.GetScalarValue();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiTag> _tagPatternFields = new PatternFieldMap<OpenApiTag> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiTag o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiXml> _xmlFixedFields = new FixedFieldMap<OpenApiXml>
	{
		{
			"name",
			delegate(OpenApiXml o, ParseNode n, OpenApiDocument _)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"namespace",
			delegate(OpenApiXml o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Namespace = new Uri(scalarValue, UriKind.Absolute);
				}
			}
		},
		{
			"prefix",
			delegate(OpenApiXml o, ParseNode n, OpenApiDocument _)
			{
				o.Prefix = n.GetScalarValue();
			}
		},
		{
			"nodeType",
			delegate(OpenApiXml o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue().TryGetEnumFromDisplayName<OpenApiXmlNodeType>(n.Context, out var result))
				{
					o.NodeType = result;
				}
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiXml> _xmlPatternFields = new PatternFieldMap<OpenApiXml> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiXml o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	public static IOpenApiCallback LoadCallback(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("callback");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiCallbackReference openApiCallbackReference = new OpenApiCallbackReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiCallbackReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiCallbackReference;
		}
		OpenApiCallback openApiCallback = new OpenApiCallback();
		ParseMap(mapNode, openApiCallback, _callbackFixedFields, _callbackPatternFields, hostDocument);
		return openApiCallback;
	}

	public static OpenApiComponents LoadComponents(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("components");
		OpenApiComponents openApiComponents = new OpenApiComponents();
		ParseMap(mapNode, openApiComponents, _componentsFixedFields, _componentsPatternFields, hostDocument);
		return openApiComponents;
	}

	public static OpenApiContact LoadContact(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node as MapNode;
		OpenApiContact openApiContact = new OpenApiContact();
		ParseMap(mapNode, openApiContact, _contactFixedFields, _contactPatternFields, hostDocument);
		return openApiContact;
	}

	public static OpenApiDiscriminator LoadDiscriminator(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("discriminator");
		OpenApiDiscriminator openApiDiscriminator = new OpenApiDiscriminator();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiDiscriminator, _discriminatorFixedFields, _discriminatorPatternFields, hostDocument);
		}
		return openApiDiscriminator;
	}

	public static OpenApiSchemaReference LoadMapping(ParseNode node, OpenApiDocument hostDocument)
	{
		(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(node.GetScalarValue() ?? throw new InvalidOperationException("Could not get a pointer reference"));
		return new OpenApiSchemaReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
	}

	public static OpenApiDocument LoadOpenApi(RootNode rootNode, Uri location)
	{
		OpenApiDocument openApiDocument = new OpenApiDocument
		{
			BaseUri = location
		};
		ParseMap(rootNode.GetMap(), openApiDocument, _openApiFixedFields, _openApiPatternFields, openApiDocument);
		openApiDocument.Workspace?.RegisterComponents(openApiDocument);
		return openApiDocument;
	}

	public static OpenApiEncoding LoadEncoding(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("encoding");
		OpenApiEncoding openApiEncoding = new OpenApiEncoding();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiEncoding, _encodingFixedFields, _encodingPatternFields, hostDocument);
		}
		return openApiEncoding;
	}

	public static IOpenApiExample LoadExample(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("example");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiExampleReference openApiExampleReference = new OpenApiExampleReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiExampleReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiExampleReference;
		}
		OpenApiExample openApiExample = new OpenApiExample();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiExample, _exampleFixedFields, _examplePatternFields, hostDocument);
		}
		return openApiExample;
	}

	public static OpenApiExternalDocs LoadExternalDocs(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("externalDocs");
		OpenApiExternalDocs openApiExternalDocs = new OpenApiExternalDocs();
		ParseMap(mapNode, openApiExternalDocs, _externalDocsFixedFields, _externalDocsPatternFields, hostDocument);
		return openApiExternalDocs;
	}

	public static IOpenApiHeader LoadHeader(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("header");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiHeaderReference openApiHeaderReference = new OpenApiHeaderReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiHeaderReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiHeaderReference;
		}
		OpenApiHeader openApiHeader = new OpenApiHeader();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiHeader, _headerFixedFields, _headerPatternFields, hostDocument);
		}
		return openApiHeader;
	}

	public static OpenApiInfo LoadInfo(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("Info");
		OpenApiInfo openApiInfo = new OpenApiInfo();
		ParseMap(mapNode, openApiInfo, InfoFixedFields, InfoPatternFields, hostDocument);
		return openApiInfo;
	}

	internal static OpenApiLicense LoadLicense(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("License");
		OpenApiLicense openApiLicense = new OpenApiLicense();
		ParseMap(mapNode, openApiLicense, _licenseFixedFields, _licensePatternFields, hostDocument);
		return openApiLicense;
	}

	public static IOpenApiLink LoadLink(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("link");
		OpenApiLink openApiLink = new OpenApiLink();
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiLinkReference openApiLinkReference = new OpenApiLinkReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiLinkReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiLinkReference;
		}
		ParseMap(mapNode, openApiLink, _linkFixedFields, _linkPatternFields, hostDocument);
		return openApiLink;
	}

	public static IOpenApiMediaType LoadMediaType(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("content");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiMediaTypeReference openApiMediaTypeReference = new OpenApiMediaTypeReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiMediaTypeReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiMediaTypeReference;
		}
		OpenApiMediaType openApiMediaType = new OpenApiMediaType();
		ParseMap(mapNode, openApiMediaType, _mediaTypeFixedFields, _mediaTypePatternFields, hostDocument);
		ProcessAnyFields(mapNode, openApiMediaType, _mediaTypeAnyFields);
		ProcessAnyMapFields(mapNode, openApiMediaType, _mediaTypeAnyMapOpenApiExampleFields);
		return openApiMediaType;
	}

	public static OpenApiOAuthFlow LoadOAuthFlow(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("OAuthFlow");
		OpenApiOAuthFlow openApiOAuthFlow = new OpenApiOAuthFlow();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiOAuthFlow, _oAuthFlowFixedFileds, _oAuthFlowPatternFields, hostDocument);
		}
		return openApiOAuthFlow;
	}

	public static OpenApiOAuthFlows LoadOAuthFlows(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("OAuthFlows");
		OpenApiOAuthFlows openApiOAuthFlows = new OpenApiOAuthFlows();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiOAuthFlows, _oAuthFlowsFixedFields, _oAuthFlowsPatternFields, hostDocument);
		}
		return openApiOAuthFlows;
	}

	internal static OpenApiOperation LoadOperation(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("Operation");
		OpenApiOperation openApiOperation = new OpenApiOperation();
		ParseMap(mapNode, openApiOperation, _operationFixedFields, _operationPatternFields, hostDocument);
		return openApiOperation;
	}

	private static OpenApiTagReference LoadTagByReference(string tagName, OpenApiDocument? hostDocument)
	{
		return new OpenApiTagReference(tagName, hostDocument);
	}

	public static IOpenApiParameter LoadParameter(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("parameter");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiParameterReference openApiParameterReference = new OpenApiParameterReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiParameterReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiParameterReference;
		}
		OpenApiParameter openApiParameter = new OpenApiParameter();
		ParseMap(mapNode, openApiParameter, _parameterFixedFields, _parameterPatternFields, hostDocument);
		ProcessAnyFields(mapNode, openApiParameter, _parameterAnyFields);
		ProcessAnyMapFields(mapNode, openApiParameter, _parameterAnyMapOpenApiExampleFields);
		return openApiParameter;
	}

	private static void LoadAdditionalOperations(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
	{
		if (n == null)
		{
			return;
		}
		foreach (PropertyNode item in from p in n.CheckMapNode("additionalOperations")
			where !OpenApiPathItem._standardHttp32MethodsNames.Contains(p.Name)
			select p)
		{
			HttpMethod operationType = new HttpMethod(item.Name);
			o.AddOperation(operationType, LoadOperation(item.Value, t));
		}
	}

	public static IOpenApiPathItem LoadPathItem(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("PathItem");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiPathItemReference openApiPathItemReference = new OpenApiPathItemReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiPathItemReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiPathItemReference;
		}
		OpenApiPathItem openApiPathItem = new OpenApiPathItem();
		ParseMap(mapNode, openApiPathItem, _pathItemFixedFields, _pathItemPatternFields, hostDocument);
		return openApiPathItem;
	}

	public static OpenApiPaths LoadPaths(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("Paths");
		OpenApiPaths openApiPaths = new OpenApiPaths();
		ParseMap(mapNode, openApiPaths, _pathsFixedFields, _pathsPatternFields, hostDocument);
		return openApiPaths;
	}

	public static IOpenApiRequestBody LoadRequestBody(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("requestBody");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiRequestBodyReference openApiRequestBodyReference = new OpenApiRequestBodyReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiRequestBodyReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiRequestBodyReference;
		}
		OpenApiRequestBody openApiRequestBody = new OpenApiRequestBody();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiRequestBody, _requestBodyFixedFields, _requestBodyPatternFields, hostDocument);
		}
		return openApiRequestBody;
	}

	public static IOpenApiResponse LoadResponse(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("response");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiResponseReference openApiResponseReference = new OpenApiResponseReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiResponseReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiResponseReference;
		}
		OpenApiResponse openApiResponse = new OpenApiResponse();
		ParseMap(mapNode, openApiResponse, _responseFixedFields, _responsePatternFields, hostDocument);
		return openApiResponse;
	}

	public static OpenApiResponses LoadResponses(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("Responses");
		OpenApiResponses openApiResponses = new OpenApiResponses();
		ParseMap(mapNode, openApiResponses, ResponsesFixedFields, ResponsesPatternFields, hostDocument);
		return openApiResponses;
	}

	public static IOpenApiSchema LoadSchema(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("schema");
		string referencePointer = mapNode.GetReferencePointer();
		string jsonSchemaIdentifier = mapNode.GetJsonSchemaIdentifier();
		string location = node.Context.GetLocation();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiSchemaReference openApiSchemaReference = new OpenApiSchemaReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiSchemaReference.Reference.SetMetadataFromMapNode(mapNode);
			openApiSchemaReference.Reference.SetJsonPointerPath(referencePointer, location);
			return openApiSchemaReference;
		}
		OpenApiSchema openApiSchema = new OpenApiSchema();
		foreach (PropertyNode propertyNode in mapNode)
		{
			if (_openApiSchemaFixedFields.ContainsKey(propertyNode.Name) || _openApiSchemaPatternFields.Any<KeyValuePair<Func<string, bool>, Action<OpenApiSchema, string, ParseNode, OpenApiDocument>>>((KeyValuePair<Func<string, bool>, Action<OpenApiSchema, string, ParseNode, OpenApiDocument>> p) => p.Key(propertyNode.Name)))
			{
				propertyNode.ParseField(openApiSchema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument);
			}
			else if (propertyNode.JsonNode != null)
			{
				OpenApiSchema openApiSchema2 = openApiSchema;
				if (openApiSchema2.UnrecognizedKeywords == null)
				{
					IDictionary<string, JsonNode> dictionary = (openApiSchema2.UnrecognizedKeywords = new Dictionary<string, JsonNode>(StringComparer.Ordinal));
				}
				openApiSchema.UnrecognizedKeywords[propertyNode.Name] = propertyNode.JsonNode;
			}
		}
		if (openApiSchema.Extensions != null && openApiSchema.Extensions.ContainsKey("x-nullable"))
		{
			if (openApiSchema.Type.HasValue)
			{
				openApiSchema.Type |= JsonSchemaType.Null;
			}
			else
			{
				openApiSchema.Type = JsonSchemaType.Null;
			}
			openApiSchema.Extensions.Remove("x-nullable");
		}
		if (!string.IsNullOrEmpty(jsonSchemaIdentifier) && hostDocument.Workspace != null)
		{
			hostDocument.Workspace.RegisterComponentForDocument(hostDocument, openApiSchema, jsonSchemaIdentifier);
		}
		return openApiSchema;
	}

	public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("security");
		OpenApiSecurityRequirement openApiSecurityRequirement = new OpenApiSecurityRequirement();
		foreach (PropertyNode item in mapNode)
		{
			OpenApiSecuritySchemeReference openApiSecuritySchemeReference = LoadSecuritySchemeByReference(item.Name, hostDocument);
			List<string> value = item.Value.CreateSimpleList((ValueNode n2, OpenApiDocument? p) => n2.GetScalarValue(), hostDocument).OfType<string>().ToList();
			if (openApiSecuritySchemeReference != null)
			{
				openApiSecurityRequirement.Add(openApiSecuritySchemeReference, value);
			}
			else
			{
				mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(node.Context.GetLocation(), "Scheme " + item.Name + " is not found"));
			}
		}
		return openApiSecurityRequirement;
	}

	private static OpenApiSecuritySchemeReference LoadSecuritySchemeByReference(string schemeName, OpenApiDocument? hostDocument)
	{
		return new OpenApiSecuritySchemeReference(schemeName, hostDocument);
	}

	public static IOpenApiSecurityScheme LoadSecurityScheme(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("securityScheme");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			OpenApiSecuritySchemeReference openApiSecuritySchemeReference = new OpenApiSecuritySchemeReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
			openApiSecuritySchemeReference.Reference.SetMetadataFromMapNode(mapNode);
			return openApiSecuritySchemeReference;
		}
		OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiSecurityScheme, _securitySchemeFixedFields, _securitySchemePatternFields, hostDocument);
		}
		return openApiSecurityScheme;
	}

	public static OpenApiServer LoadServer(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("server");
		OpenApiServer openApiServer = new OpenApiServer();
		ParseMap(mapNode, openApiServer, _serverFixedFields, _serverPatternFields, hostDocument);
		return openApiServer;
	}

	public static OpenApiServerVariable LoadServerVariable(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("serverVariable");
		OpenApiServerVariable openApiServerVariable = new OpenApiServerVariable();
		ParseMap(mapNode, openApiServerVariable, _serverVariableFixedFields, _serverVariablePatternFields, hostDocument);
		return openApiServerVariable;
	}

	public static OpenApiTag LoadTag(ParseNode n, OpenApiDocument hostDocument)
	{
		MapNode mapNode = n.CheckMapNode("tag");
		OpenApiTag openApiTag = new OpenApiTag();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiTag, _tagFixedFields, _tagPatternFields, hostDocument);
		}
		return openApiTag;
	}

	private static void ParseMap<T>(MapNode? mapNode, T domainObject, FixedFieldMap<T> fixedFieldMap, PatternFieldMap<T> patternFieldMap, OpenApiDocument doc)
	{
		if (mapNode == null)
		{
			return;
		}
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(domainObject, fixedFieldMap, patternFieldMap, doc);
		}
	}

	private static void ProcessAnyFields<T>(MapNode mapNode, T domainObject, AnyFieldMap<T> anyFieldMap)
	{
		foreach (string item in anyFieldMap.Keys.ToList())
		{
			try
			{
				mapNode.Context.StartObject(item);
				JsonNode jsonNode = anyFieldMap[item].PropertyGetter(domainObject);
				if (jsonNode == null)
				{
					anyFieldMap[item].PropertySetter(domainObject, null);
				}
				else
				{
					anyFieldMap[item].PropertySetter(domainObject, jsonNode);
				}
			}
			catch (OpenApiException ex)
			{
				ex.Pointer = mapNode.Context.GetLocation();
				mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(ex));
			}
			finally
			{
				mapNode.Context.EndObject();
			}
		}
	}

	private static void ProcessAnyMapFields<T, U>(MapNode mapNode, T domainObject, AnyMapFieldMap<T, U> anyMapFieldMap)
	{
		foreach (string item in anyMapFieldMap.Keys.ToList())
		{
			try
			{
				mapNode.Context.StartObject(item);
				IDictionary<string, U> dictionary = anyMapFieldMap[item].PropertyMapGetter(domainObject);
				if (dictionary == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, U> item2 in dictionary)
				{
					mapNode.Context.StartObject(item2.Key);
					if (item2.Value != null)
					{
						JsonNode jsonNode = anyMapFieldMap[item].PropertyGetter(item2.Value);
						if (jsonNode != null)
						{
							anyMapFieldMap[item].PropertySetter(item2.Value, jsonNode);
						}
					}
				}
			}
			catch (OpenApiException ex)
			{
				ex.Pointer = mapNode.Context.GetLocation();
				mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(ex));
			}
			finally
			{
				mapNode.Context.EndObject();
			}
		}
	}

	private static RuntimeExpressionAnyWrapper LoadRuntimeExpressionAnyWrapper(ParseNode node)
	{
		string scalarValue = node.GetScalarValue();
		if (scalarValue != null && scalarValue.StartsWith("$", StringComparison.OrdinalIgnoreCase))
		{
			return new RuntimeExpressionAnyWrapper
			{
				Expression = RuntimeExpression.Build(scalarValue)
			};
		}
		return new RuntimeExpressionAnyWrapper
		{
			Any = node.CreateAny()
		};
	}

	public static JsonNode LoadAny(ParseNode node, OpenApiDocument hostDocument)
	{
		return node.CreateAny();
	}

	private static IOpenApiExtension LoadExtension(string name, ParseNode node)
	{
		if (node.Context.ExtensionParsers == null || !node.Context.ExtensionParsers.TryGetValue(name, out Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension> value))
		{
			return new JsonNodeExtension(node.CreateAny());
		}
		return value(node.CreateAny(), OpenApiSpecVersion.OpenApi3_2);
	}

	private static string? LoadString(ParseNode node)
	{
		return node.GetScalarValue();
	}

	private static bool? LoadBool(ParseNode node)
	{
		string scalarValue = node.GetScalarValue();
		if (scalarValue == null)
		{
			return null;
		}
		return bool.Parse(scalarValue);
	}

	private static (string, string?) GetReferenceIdAndExternalResource(string pointer)
	{
		string[] array = pointer.Split('/');
		string item = ((!pointer.Contains('#')) ? pointer : array[array.Count() - 1]);
		bool num = !array[0].StartsWith("#", StringComparison.OrdinalIgnoreCase);
		string item2 = null;
		if (num && pointer.Contains('#'))
		{
			item2 = pointer.Split('#')[0].TrimEnd('#');
		}
		return (item, item2);
	}

	public static OpenApiXml LoadXml(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("xml");
		OpenApiXml openApiXml = new OpenApiXml();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiXml, _xmlFixedFields, _xmlPatternFields, hostDocument);
		}
		return openApiXml;
	}
}
