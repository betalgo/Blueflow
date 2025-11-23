using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V2;

/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V2 document into
/// runtime Open API object model.
/// </summary>
/// <summary>
/// Class containing logic to deserialize Open API V3 document into
/// runtime Open API object model.
/// </summary>
internal static class OpenApiV2Deserializer
{
	private static readonly FixedFieldMap<OpenApiContact> _contactFixedFields = new FixedFieldMap<OpenApiContact>
	{
		{
			"name",
			delegate(OpenApiContact o, ParseNode n, OpenApiDocument t)
			{
				o.Name = n.GetScalarValue();
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
		},
		{
			"email",
			delegate(OpenApiContact o, ParseNode n, OpenApiDocument t)
			{
				o.Email = n.GetScalarValue();
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

	private static readonly FixedFieldMap<OpenApiDocument> _openApiFixedFields = new FixedFieldMap<OpenApiDocument>
	{
		{
			"swagger",
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
			"host",
			delegate(OpenApiDocument _, ParseNode n, OpenApiDocument _)
			{
				n.Context.SetTempStorage("host", n.GetScalarValue());
			}
		},
		{
			"basePath",
			delegate(OpenApiDocument _, ParseNode n, OpenApiDocument _)
			{
				n.Context.SetTempStorage("basePath", n.GetScalarValue());
			}
		},
		{
			"schemes",
			delegate(OpenApiDocument _, ParseNode n, OpenApiDocument doc)
			{
				n.Context.SetTempStorage("schemes", n.CreateSimpleList((ValueNode s, OpenApiDocument? p) => s.GetScalarValue(), doc));
			}
		},
		{
			"consumes",
			delegate(OpenApiDocument _, ParseNode n, OpenApiDocument doc)
			{
				List<string> list = n.CreateSimpleList((ValueNode s, OpenApiDocument? p) => s.GetScalarValue(), doc);
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("globalConsumes", list);
				}
			}
		},
		{
			"produces",
			delegate(OpenApiDocument _, ParseNode n, OpenApiDocument doc)
			{
				List<string> list = n.CreateSimpleList((ValueNode s, OpenApiDocument? p) => s.GetScalarValue(), doc);
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("globalProduces", list);
				}
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
			"definitions",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				if (o.Components == null)
				{
					OpenApiComponents openApiComponents = (o.Components = new OpenApiComponents());
				}
				o.Components.Schemas = n.CreateMap(LoadSchema, o);
			}
		},
		{
			"parameters",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument doc)
			{
				if (o.Components == null)
				{
					OpenApiComponents openApiComponents = (o.Components = new OpenApiComponents());
				}
				o.Components.Parameters = (from kvp in n.CreateMap(LoadParameter, o)
					where kvp.Value != null
					select kvp).ToDictionary((KeyValuePair<string, IOpenApiParameter> kvp) => kvp.Key, (KeyValuePair<string, IOpenApiParameter> kvp) => kvp.Value);
				o.Components.RequestBodies = (from kvp in n.CreateMap(delegate(MapNode p, OpenApiDocument d)
					{
						IOpenApiParameter openApiParameter = LoadParameter(p, loadRequestBody: true, d);
						return (openApiParameter == null) ? null : CreateRequestBody(p.Context, openApiParameter);
					}, doc)
					where kvp.Value != null
					select kvp).ToDictionary((KeyValuePair<string, IOpenApiRequestBody> kvp) => kvp.Key, (KeyValuePair<string, IOpenApiRequestBody> kvp) => kvp.Value);
			}
		},
		{
			"responses",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				if (o.Components == null)
				{
					o.Components = new OpenApiComponents();
				}
				o.Components.Responses = n.CreateMap(LoadResponse, o);
			}
		},
		{
			"securityDefinitions",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				if (o.Components == null)
				{
					o.Components = new OpenApiComponents();
				}
				o.Components.SecuritySchemes = n.CreateMap(LoadSecurityScheme, o);
			}
		},
		{
			"security",
			delegate(OpenApiDocument o, ParseNode n, OpenApiDocument _)
			{
				o.Security = n.CreateList(LoadSecurityRequirement, o);
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

	private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields = new FixedFieldMap<OpenApiExternalDocs>
	{
		{
			"description",
			delegate(OpenApiExternalDocs o, ParseNode n, OpenApiDocument _)
			{
				if (n.GetScalarValue() != null)
				{
					o.Description = n.GetScalarValue();
				}
			}
		},
		{
			"url",
			delegate(OpenApiExternalDocs o, ParseNode n, OpenApiDocument _)
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
			"type",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).Type = scalarValue.ToJsonSchemaType();
				}
			}
		},
		{
			"format",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				GetOrCreateSchema(o).Format = n.GetScalarValue();
			}
		},
		{
			"items",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument doc)
			{
				GetOrCreateSchema(o).Items = LoadSchema(n, doc);
			}
		},
		{
			"collectionFormat",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					LoadStyle(o, scalarValue);
				}
			}
		},
		{
			"default",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				GetOrCreateSchema(o).Default = n.CreateAny();
			}
		},
		{
			"maximum",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (!string.IsNullOrEmpty(scalarValue))
				{
					GetOrCreateSchema(o).Maximum = scalarValue;
				}
			}
		},
		{
			"exclusiveMaximum",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				GetOrCreateSchema(o).IsExclusiveMaximum = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"minimum",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (!string.IsNullOrEmpty(scalarValue))
				{
					GetOrCreateSchema(o).Minimum = scalarValue;
				}
			}
		},
		{
			"exclusiveMinimum",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				GetOrCreateSchema(o).IsExclusiveMinimum = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"maxLength",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).MaxLength = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"minLength",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).MinLength = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"pattern",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				GetOrCreateSchema(o).Pattern = n.GetScalarValue();
			}
		},
		{
			"maxItems",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).MaxItems = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"minItems",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).MinItems = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"uniqueItems",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).UniqueItems = bool.Parse(scalarValue);
				}
			}
		},
		{
			"multipleOf",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).MultipleOf = decimal.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"enum",
			delegate(OpenApiHeader o, ParseNode n, OpenApiDocument _)
			{
				GetOrCreateSchema(o).Enum = n.CreateListOfAny();
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

	private static readonly FixedFieldMap<OpenApiInfo> _infoFixedFields = new FixedFieldMap<OpenApiInfo>
	{
		{
			"title",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument _)
			{
				o.Title = n.GetScalarValue();
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
		},
		{
			"version",
			delegate(OpenApiInfo o, ParseNode n, OpenApiDocument _)
			{
				o.Version = n.GetScalarValue();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiInfo> _infoPatternFields = new PatternFieldMap<OpenApiInfo> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiInfo o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
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
				o.Parameters = n.CreateList(LoadParameter, t).OfType<IOpenApiParameter>().ToList();
			}
		},
		{
			"consumes",
			delegate(OpenApiOperation _, ParseNode n, OpenApiDocument doc)
			{
				List<string> list = n.CreateSimpleList((ValueNode s, OpenApiDocument? p) => s.GetScalarValue(), doc);
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("operationConsumes", list);
				}
			}
		},
		{
			"produces",
			delegate(OpenApiOperation _, ParseNode n, OpenApiDocument doc)
			{
				List<string> list = n.CreateSimpleList((ValueNode s, OpenApiDocument? p) => s.GetScalarValue(), doc);
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("operationProduces", list);
				}
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

	private static readonly FixedFieldMap<OpenApiResponses> _responsesFixedFields = new FixedFieldMap<OpenApiResponses>();

	private static readonly PatternFieldMap<OpenApiResponses> _responsesPatternFields = new PatternFieldMap<OpenApiResponses>
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

	private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields = new FixedFieldMap<OpenApiParameter>
	{
		{
			"name",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{ "in", ProcessIn },
		{
			"description",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
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
			"type",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					OpenApiSchema orCreateSchema = GetOrCreateSchema(o);
					orCreateSchema.Type = scalarValue.ToJsonSchemaType();
					if ("file".Equals(scalarValue, StringComparison.OrdinalIgnoreCase))
					{
						orCreateSchema.Format = "binary";
					}
				}
			}
		},
		{
			"items",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				GetOrCreateSchema(o).Items = LoadSchema(n, t);
			}
		},
		{
			"collectionFormat",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					LoadStyle(o, scalarValue);
				}
			}
		},
		{
			"format",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				GetOrCreateSchema(o).Format = n.GetScalarValue();
			}
		},
		{
			"minimum",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (!string.IsNullOrEmpty(scalarValue))
				{
					GetOrCreateSchema(o).Minimum = scalarValue;
				}
			}
		},
		{
			"maximum",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (!string.IsNullOrEmpty(scalarValue))
				{
					GetOrCreateSchema(o).Maximum = scalarValue;
				}
			}
		},
		{
			"maxLength",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).MaxLength = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"minLength",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).MinLength = int.Parse(scalarValue, CultureInfo.InvariantCulture);
				}
			}
		},
		{
			"readOnly",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					GetOrCreateSchema(o).ReadOnly = bool.Parse(scalarValue);
				}
			}
		},
		{
			"default",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				GetOrCreateSchema(o).Default = n.CreateAny();
			}
		},
		{
			"pattern",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				GetOrCreateSchema(o).Pattern = n.GetScalarValue();
			}
		},
		{
			"enum",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				GetOrCreateSchema(o).Enum = n.CreateListOfAny();
			}
		},
		{
			"schema",
			delegate(OpenApiParameter o, ParseNode n, OpenApiDocument t)
			{
				o.Schema = LoadSchema(n, t);
			}
		},
		{ "x-examples", LoadParameterExamplesExtension }
	};

	private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields = new PatternFieldMap<OpenApiParameter> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase) && !s.Equals("x-examples", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiParameter o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new FixedFieldMap<OpenApiPathItem>
	{
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
		{ "parameters", LoadPathParameters }
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

	private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new FixedFieldMap<OpenApiResponse>
	{
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
		{ "examples", LoadExamples },
		{ "x-examples", LoadResponseExamplesExtension },
		{
			"schema",
			delegate(OpenApiResponse o, ParseNode n, OpenApiDocument t)
			{
				n.Context.SetTempStorage("responseSchema", LoadSchema(n, t), o);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields = new PatternFieldMap<OpenApiResponse> { 
	{
		(string s) => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase) && !s.Equals("x-examples", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiResponse o, string p, ParseNode n, OpenApiDocument _)
		{
			o.AddExtension(p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields = new AnyFieldMap<OpenApiMediaType> { 
	{
		"example",
		new AnyFieldMapParameter<OpenApiMediaType>((OpenApiMediaType m) => m.Example, delegate(OpenApiMediaType m, JsonNode? v)
		{
			m.Example = v;
		}, (OpenApiMediaType m) => m.Schema)
	} };

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
				o.IsExclusiveMaximum = bool.Parse(n.GetScalarValue());
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
				o.IsExclusiveMinimum = bool.Parse(n.GetScalarValue());
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
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Type = scalarValue.ToJsonSchemaType();
				}
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
			"discriminator",
			delegate(OpenApiSchema o, ParseNode n, OpenApiDocument _)
			{
				o.Discriminator = new OpenApiDiscriminator
				{
					PropertyName = n.GetScalarValue()
				};
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

	private static string? _flowValue;

	private static OpenApiOAuthFlow? _flow;

	private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields = new FixedFieldMap<OpenApiSecurityScheme>
	{
		{
			"type",
			delegate(OpenApiSecurityScheme o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				switch (scalarValue)
				{
				case "basic":
					o.Type = SecuritySchemeType.Http;
					o.Scheme = "basic";
					break;
				case "apiKey":
					o.Type = SecuritySchemeType.ApiKey;
					break;
				case "oauth2":
					o.Type = SecuritySchemeType.OAuth2;
					break;
				default:
					n.Context.Diagnostic.Errors.Add(new OpenApiError(n.Context.GetLocation(), "Security scheme type " + scalarValue + " is not recognized."));
					break;
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
			"flow",
			delegate(OpenApiSecurityScheme _, ParseNode n, OpenApiDocument _)
			{
				_flowValue = n.GetScalarValue();
			}
		},
		{
			"authorizationUrl",
			delegate(OpenApiSecurityScheme _, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (_flow != null && scalarValue != null)
				{
					_flow.AuthorizationUrl = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"tokenUrl",
			delegate(OpenApiSecurityScheme _, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (_flow != null && scalarValue != null)
				{
					_flow.TokenUrl = new Uri(scalarValue, UriKind.RelativeOrAbsolute);
				}
			}
		},
		{
			"scopes",
			delegate(OpenApiSecurityScheme _, ParseNode n, OpenApiDocument _)
			{
				if (_flow != null)
				{
					_flow.Scopes = (from kv in n.CreateSimpleMap(LoadString)
						where kv.Value != null
						select kv).ToDictionary((KeyValuePair<string, string> kv) => kv.Key, (KeyValuePair<string, string> kv) => kv.Value);
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
				if (Uri.IsWellFormedUriString(scalarValue, UriKind.Absolute) && scalarValue != null)
				{
					o.Namespace = new Uri(scalarValue, UriKind.Absolute);
					return;
				}
				throw new OpenApiReaderException("Xml Namespace requires absolute URL. '" + n.GetScalarValue() + "' is not valid.");
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
			"attribute",
			delegate(OpenApiXml o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Attribute = bool.Parse(scalarValue);
				}
			}
		},
		{
			"wrapped",
			delegate(OpenApiXml o, ParseNode n, OpenApiDocument _)
			{
				string scalarValue = n.GetScalarValue();
				if (scalarValue != null)
				{
					o.Wrapped = bool.Parse(scalarValue);
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

	public static OpenApiContact LoadContact(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node as MapNode;
		OpenApiContact openApiContact = new OpenApiContact();
		ParseMap(mapNode, openApiContact, _contactFixedFields, _contactPatternFields, hostDocument);
		return openApiContact;
	}

	private static void MakeServers(IList<OpenApiServer> servers, ParsingContext context, RootNode rootNode)
	{
		string text = context.GetFromTempStorage<string>("host");
		string text2 = context.GetFromTempStorage<string>("basePath");
		List<string> list = context.GetFromTempStorage<List<string>>("schemes");
		Uri baseUrl = rootNode.Context.BaseUrl;
		if (string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text))
		{
			text2 = "/";
		}
		if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2) && (list == null || list.Count == 0) && baseUrl == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(text) && !IsHostValid(text))
		{
			rootNode.Context.Diagnostic.Errors.Add(new OpenApiError(rootNode.Context.GetLocation(), "Invalid host"));
			return;
		}
		if (baseUrl != null)
		{
			text = text ?? baseUrl.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped);
			text2 = text2 ?? baseUrl.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
			List<string> list2 = list;
			if (list2 == null)
			{
				int num = 1;
				list2 = new List<string>(num);
				CollectionsMarshal.SetCount(list2, num);
				Span<string> span = CollectionsMarshal.AsSpan(list2);
				int num2 = 0;
				span[num2] = baseUrl.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped);
				num2++;
			}
			list = list2;
		}
		else if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
		{
			return;
		}
		if (list != null && list.Count > 0)
		{
			foreach (string item3 in list)
			{
				OpenApiServer item = new OpenApiServer
				{
					Url = BuildUrl(item3, text, text2)
				};
				servers.Add(item);
			}
		}
		else
		{
			OpenApiServer item2 = new OpenApiServer
			{
				Url = BuildUrl(null, text, text2)
			};
			servers.Add(item2);
		}
		foreach (OpenApiServer server in servers)
		{
			if (server.Url != null && server.Url.EndsWith("/"))
			{
				server.Url = server.Url.Substring(0, server.Url.Length - 1);
			}
		}
	}

	private static string BuildUrl(string? scheme, string? host, string? basePath)
	{
		if (string.IsNullOrEmpty(scheme) && !string.IsNullOrEmpty(host))
		{
			host = "//" + host;
		}
		int? num = null;
		if (!string.IsNullOrEmpty(host) && host != null && host.Contains(':'))
		{
			string[] array = host.Split(':');
			if (array != null)
			{
				host = array[0];
				num = int.Parse(array[array.Count() - 1], CultureInfo.InvariantCulture);
			}
		}
		UriBuilder uriBuilder = new UriBuilder
		{
			Scheme = scheme,
			Host = host,
			Path = basePath
		};
		if (num.HasValue)
		{
			uriBuilder.Port = num.Value;
		}
		if (("https".Equals(uriBuilder.Scheme, StringComparison.OrdinalIgnoreCase) && uriBuilder.Port == 443) || ("http".Equals(uriBuilder.Scheme, StringComparison.OrdinalIgnoreCase) && uriBuilder.Port == 80))
		{
			uriBuilder.Port = -1;
		}
		return uriBuilder.ToString();
	}

	public static OpenApiDocument LoadOpenApi(RootNode rootNode, Uri location)
	{
		OpenApiDocument openApiDocument = new OpenApiDocument
		{
			BaseUri = location
		};
		MapNode map = rootNode.GetMap();
		ParseMap(map, openApiDocument, _openApiFixedFields, _openApiPatternFields, openApiDocument);
		if (openApiDocument.Paths != null)
		{
			ProcessResponsesMediaTypes(rootNode.GetMap(), openApiDocument.Paths.Values.SelectMany(delegate(IOpenApiPathItem path)
			{
				IEnumerable<OpenApiOperation> enumerable = path.Operations?.Values;
				return enumerable ?? Enumerable.Empty<OpenApiOperation>();
			}).SelectMany(delegate(OpenApiOperation operation)
			{
				IEnumerable<IOpenApiResponse> enumerable = operation.Responses?.Values;
				return enumerable ?? Enumerable.Empty<IOpenApiResponse>();
			}), map.Context);
		}
		ProcessResponsesMediaTypes(rootNode.GetMap(), openApiDocument.Components?.Responses?.Values, map.Context);
		if (openApiDocument.Servers == null)
		{
			openApiDocument.Servers = new List<OpenApiServer>();
		}
		MakeServers(openApiDocument.Servers, map.Context, rootNode);
		FixRequestBodyReferences(openApiDocument);
		openApiDocument.Workspace?.RegisterComponents(openApiDocument);
		return openApiDocument;
	}

	private static void ProcessResponsesMediaTypes(MapNode mapNode, IEnumerable<IOpenApiResponse>? responses, ParsingContext context)
	{
		if (responses == null)
		{
			return;
		}
		foreach (OpenApiResponse item in responses.OfType<OpenApiResponse>())
		{
			ProcessProduces(mapNode, item, context);
			if (item.Content == null)
			{
				continue;
			}
			foreach (OpenApiMediaType item2 in item.Content.Values.OfType<OpenApiMediaType>())
			{
				ProcessAnyFields(mapNode, item2, _mediaTypeAnyFields);
			}
		}
	}

	private static void FixRequestBodyReferences(OpenApiDocument doc)
	{
		IDictionary<string, IOpenApiRequestBody> dictionary = doc.Components?.RequestBodies;
		if (dictionary != null && dictionary.Count > 0)
		{
			new OpenApiWalker(new RequestBodyReferenceFixer(doc.Components.RequestBodies)).Walk(doc);
		}
	}

	private static bool IsHostValid(string host)
	{
		if (host.Contains(Uri.SchemeDelimiter))
		{
			return false;
		}
		return Uri.CheckHostName(host.Split(':').First()) != UriHostNameType.Unknown;
	}

	public static OpenApiExternalDocs LoadExternalDocs(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("externalDocs");
		OpenApiExternalDocs openApiExternalDocs = new OpenApiExternalDocs();
		ParseMap(mapNode, openApiExternalDocs, _externalDocsFixedFields, _externalDocsPatternFields, hostDocument);
		return openApiExternalDocs;
	}

	private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
	{
		if (p.Schema is OpenApiSchema result)
		{
			return result;
		}
		IOpenApiSchema openApiSchema = (p.Schema = new OpenApiSchema());
		return (OpenApiSchema)openApiSchema;
	}

	public static IOpenApiHeader LoadHeader(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("header");
		OpenApiHeader openApiHeader = new OpenApiHeader();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiHeader, _headerFixedFields, _headerPatternFields, hostDocument);
		}
		IOpenApiSchema fromTempStorage = node.Context.GetFromTempStorage<IOpenApiSchema>("schema");
		if (fromTempStorage != null)
		{
			openApiHeader.Schema = fromTempStorage;
			node.Context.SetTempStorage("schema", null);
		}
		return openApiHeader;
	}

	private static void LoadStyle(OpenApiHeader header, string style)
	{
		switch (style)
		{
		case "csv":
			header.Style = ParameterStyle.Simple;
			break;
		case "ssv":
			header.Style = ParameterStyle.SpaceDelimited;
			break;
		case "pipes":
			header.Style = ParameterStyle.PipeDelimited;
			break;
		case "tsv":
			throw new NotSupportedException();
		default:
			throw new OpenApiReaderException("Unrecognized header style: " + style);
		}
	}

	public static OpenApiInfo LoadInfo(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("Info");
		OpenApiInfo openApiInfo = new OpenApiInfo();
		ParseMap(mapNode, openApiInfo, _infoFixedFields, _infoPatternFields, hostDocument);
		return openApiInfo;
	}

	public static OpenApiLicense LoadLicense(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("OpenApiLicense");
		OpenApiLicense openApiLicense = new OpenApiLicense();
		ParseMap(mapNode, openApiLicense, _licenseFixedFields, _licensePatternFields, hostDocument);
		return openApiLicense;
	}

	internal static OpenApiOperation LoadOperation(ParseNode node, OpenApiDocument hostDocument)
	{
		node.Context.SetTempStorage("bodyParameter", null);
		node.Context.SetTempStorage("formParameters", null);
		node.Context.SetTempStorage("operationProduces", null);
		node.Context.SetTempStorage("operationConsumes", null);
		MapNode mapNode = node.CheckMapNode("Operation");
		OpenApiOperation openApiOperation = new OpenApiOperation();
		ParseMap(mapNode, openApiOperation, _operationFixedFields, _operationPatternFields, hostDocument);
		OpenApiParameter fromTempStorage = node.Context.GetFromTempStorage<OpenApiParameter>("bodyParameter");
		if (fromTempStorage != null)
		{
			openApiOperation.RequestBody = CreateRequestBody(node.Context, fromTempStorage);
		}
		else
		{
			List<OpenApiParameter> fromTempStorage2 = node.Context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
			if (fromTempStorage2 != null)
			{
				openApiOperation.RequestBody = CreateFormBody(node.Context, fromTempStorage2);
			}
		}
		OpenApiResponses responses = openApiOperation.Responses;
		if (responses != null)
		{
			foreach (OpenApiResponse item in responses.Values.OfType<OpenApiResponse>())
			{
				ProcessProduces(node.CheckMapNode("responses"), item, node.Context);
			}
		}
		node.Context.SetTempStorage("operationProduces", null);
		return openApiOperation;
	}

	public static OpenApiResponses LoadResponses(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("Responses");
		OpenApiResponses openApiResponses = new OpenApiResponses();
		ParseMap(mapNode, openApiResponses, _responsesFixedFields, _responsesPatternFields, hostDocument);
		return openApiResponses;
	}

	private static OpenApiRequestBody CreateFormBody(ParsingContext context, List<OpenApiParameter> formParameters)
	{
		OpenApiMediaType mediaType = new OpenApiMediaType
		{
			Schema = new OpenApiSchema
			{
				Properties = formParameters.Where((OpenApiParameter p) => p.Name != null).ToDictionary((OpenApiParameter k) => k.Name, delegate(OpenApiParameter v)
				{
					IOpenApiSchema openApiSchema = v.Schema.CreateShallowCopy();
					openApiSchema.Description = v.Description;
					if (openApiSchema is OpenApiSchema openApiSchema2)
					{
						openApiSchema2.Extensions = v.Extensions;
					}
					return openApiSchema;
				}),
				Required = new HashSet<string>(from p in formParameters
					where p.Required && p.Name != null
					select p.Name, StringComparer.Ordinal)
			}
		};
		List<string> list = context.GetFromTempStorage<List<string>>("operationConsumes");
		if (list == null)
		{
			list = context.GetFromTempStorage<List<string>>("globalConsumes");
			if (list == null)
			{
				int num = 1;
				list = new List<string>(num);
				CollectionsMarshal.SetCount(list, num);
				Span<string> span = CollectionsMarshal.AsSpan(list);
				int num2 = 0;
				span[num2] = "application/x-www-form-urlencoded";
				num2++;
			}
		}
		List<string> source = list;
		OpenApiRequestBody openApiRequestBody = new OpenApiRequestBody
		{
			Content = ((IEnumerable<string>)source).ToDictionary((Func<string, string>)((string k) => k), (Func<string, IOpenApiMediaType>)((string _) => mediaType))
		};
		foreach (OpenApiSchema item in (from x in openApiRequestBody.Content.Values
			where x.Schema != null && x.Schema.Properties != null && x.Schema.Properties.Any() && !x.Schema.Type.HasValue
			select x.Schema).OfType<OpenApiSchema>())
		{
			item.Type = JsonSchemaType.Object;
		}
		return openApiRequestBody;
	}

	internal static IOpenApiRequestBody CreateRequestBody(ParsingContext context, IOpenApiParameter bodyParameter)
	{
		List<string> list = context.GetFromTempStorage<List<string>>("operationConsumes");
		if (list == null)
		{
			list = context.GetFromTempStorage<List<string>>("globalConsumes");
			if (list == null)
			{
				int num = 1;
				list = new List<string>(num);
				CollectionsMarshal.SetCount(list, num);
				Span<string> span = CollectionsMarshal.AsSpan(list);
				int num2 = 0;
				span[num2] = "application/json";
				num2++;
			}
		}
		List<string> source = list;
		OpenApiRequestBody openApiRequestBody = new OpenApiRequestBody
		{
			Description = bodyParameter.Description,
			Required = bodyParameter.Required,
			Content = ((IEnumerable<string>)source).ToDictionary((Func<string, string>)((string k) => k), (Func<string, IOpenApiMediaType>)((string _) => new OpenApiMediaType
			{
				Schema = bodyParameter.Schema,
				Examples = bodyParameter.Examples
			})),
			Extensions = bodyParameter.Extensions
		};
		if (bodyParameter.Name != null)
		{
			OpenApiRequestBody openApiRequestBody2 = openApiRequestBody;
			if (openApiRequestBody2.Extensions == null)
			{
				IDictionary<string, IOpenApiExtension> dictionary = (openApiRequestBody2.Extensions = new Dictionary<string, IOpenApiExtension>());
			}
			openApiRequestBody.Extensions["x-bodyName"] = new JsonNodeExtension(bodyParameter.Name);
		}
		return openApiRequestBody;
	}

	private static OpenApiTagReference LoadTagByReference(string tagName, OpenApiDocument? hostDocument)
	{
		return new OpenApiTagReference(tagName, hostDocument);
	}

	private static void LoadStyle(OpenApiParameter p, string v)
	{
		switch (v)
		{
		case "csv":
			if (p.In == ParameterLocation.Query)
			{
				p.Style = ParameterStyle.Form;
			}
			else
			{
				p.Style = ParameterStyle.Simple;
			}
			break;
		case "ssv":
			p.Style = ParameterStyle.SpaceDelimited;
			break;
		case "pipes":
			p.Style = ParameterStyle.PipeDelimited;
			break;
		case "tsv":
			throw new NotSupportedException();
		case "multi":
			p.Style = ParameterStyle.Form;
			p.Explode = true;
			break;
		}
	}

	private static void LoadParameterExamplesExtension(OpenApiParameter parameter, ParseNode node, OpenApiDocument? hostDocument)
	{
		Dictionary<string, IOpenApiExample> value = LoadExamplesExtension(node);
		node.Context.SetTempStorage("examples", value, parameter);
	}

	private static OpenApiSchema GetOrCreateSchema(OpenApiParameter p)
	{
		if (p.Schema is OpenApiSchema result)
		{
			return result;
		}
		IOpenApiSchema openApiSchema = (p.Schema = new OpenApiSchema());
		return (OpenApiSchema)openApiSchema;
	}

	private static void ProcessIn(OpenApiParameter o, ParseNode n, OpenApiDocument hostDocument)
	{
		string scalarValue = n.GetScalarValue();
		switch (scalarValue)
		{
		case "body":
			n.Context.SetTempStorage("parameterIsBodyOrFormData", true);
			n.Context.SetTempStorage("bodyParameter", o);
			break;
		case "formData":
		{
			n.Context.SetTempStorage("parameterIsBodyOrFormData", true);
			List<OpenApiParameter> list = n.Context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
			if (list == null)
			{
				list = new List<OpenApiParameter>();
				n.Context.SetTempStorage("formParameters", list);
			}
			list.Add(o);
			break;
		}
		case "query":
		case "header":
		case "path":
		{
			if (scalarValue.TryGetEnumFromDisplayName<ParameterLocation>(out var result))
			{
				o.In = result;
			}
			break;
		}
		default:
			o.In = null;
			break;
		}
	}

	public static IOpenApiParameter? LoadParameter(ParseNode node, OpenApiDocument hostDocument)
	{
		return LoadParameter(node, loadRequestBody: false, hostDocument);
	}

	public static IOpenApiParameter? LoadParameter(ParseNode node, bool loadRequestBody, OpenApiDocument hostDocument)
	{
		node.Context.SetTempStorage("parameterIsBodyOrFormData", false);
		MapNode mapNode = node.CheckMapNode("parameter");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			return new OpenApiParameterReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
		}
		OpenApiParameter openApiParameter = new OpenApiParameter();
		ParseMap(mapNode, openApiParameter, _parameterFixedFields, _parameterPatternFields, hostDocument);
		IOpenApiSchema fromTempStorage = node.Context.GetFromTempStorage<IOpenApiSchema>("schema");
		if (fromTempStorage != null)
		{
			openApiParameter.Schema = fromTempStorage;
			node.Context.SetTempStorage("schema", null);
		}
		Dictionary<string, IOpenApiExample> fromTempStorage2 = node.Context.GetFromTempStorage<Dictionary<string, IOpenApiExample>>("examples", openApiParameter);
		if (fromTempStorage2 != null)
		{
			openApiParameter.Examples = fromTempStorage2;
			node.Context.SetTempStorage("examples", null);
		}
		bool flag = false;
		if (node.Context.GetFromTempStorage<object>("parameterIsBodyOrFormData") is bool flag2)
		{
			flag = flag2;
		}
		if (flag && !loadRequestBody)
		{
			return null;
		}
		if (loadRequestBody && !flag)
		{
			return null;
		}
		return openApiParameter;
	}

	public static OpenApiPathItem LoadPathItem(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("PathItem");
		OpenApiPathItem openApiPathItem = new OpenApiPathItem();
		ParseMap(mapNode, openApiPathItem, _pathItemFixedFields, _pathItemPatternFields, hostDocument);
		return openApiPathItem;
	}

	private static void LoadPathParameters(OpenApiPathItem pathItem, ParseNode node, OpenApiDocument hostDocument)
	{
		node.Context.SetTempStorage("bodyParameter", null);
		node.Context.SetTempStorage("formParameters", null);
		pathItem.Parameters = node.CreateList(LoadParameter, hostDocument).OfType<IOpenApiParameter>().ToList();
		OpenApiParameter fromTempStorage = node.Context.GetFromTempStorage<OpenApiParameter>("bodyParameter");
		if (fromTempStorage != null && pathItem.Operations != null)
		{
			IOpenApiRequestBody requestBody = CreateRequestBody(node.Context, fromTempStorage);
			{
				foreach (KeyValuePair<HttpMethod, OpenApiOperation> item in pathItem.Operations.Where<KeyValuePair<HttpMethod, OpenApiOperation>>((KeyValuePair<HttpMethod, OpenApiOperation> x) => x.Value.RequestBody == null))
				{
					if (item.Key == HttpMethod.Post || item.Key == HttpMethod.Put || item.Key == new HttpMethod("PATCH"))
					{
						item.Value.RequestBody = requestBody;
					}
				}
				return;
			}
		}
		List<OpenApiParameter> fromTempStorage2 = node.Context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
		if (fromTempStorage2 == null || pathItem.Operations == null)
		{
			return;
		}
		OpenApiRequestBody requestBody2 = CreateFormBody(node.Context, fromTempStorage2);
		foreach (KeyValuePair<HttpMethod, OpenApiOperation> item2 in pathItem.Operations.Where<KeyValuePair<HttpMethod, OpenApiOperation>>((KeyValuePair<HttpMethod, OpenApiOperation> x) => x.Value.RequestBody == null))
		{
			if (item2.Key == HttpMethod.Post || item2.Key == HttpMethod.Put || item2.Key == new HttpMethod("PATCH"))
			{
				item2.Value.RequestBody = requestBody2;
			}
		}
	}

	public static OpenApiPaths LoadPaths(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("Paths");
		OpenApiPaths openApiPaths = new OpenApiPaths();
		ParseMap(mapNode, openApiPaths, _pathsFixedFields, _pathsPatternFields, hostDocument);
		return openApiPaths;
	}

	private static void ProcessProduces(MapNode mapNode, OpenApiResponse response, ParsingContext context)
	{
		if (response.Content == null)
		{
			response.Content = new Dictionary<string, IOpenApiMediaType>();
		}
		else if (context.GetFromTempStorage<bool>("responseProducesSet", response))
		{
			return;
		}
		List<string> list = context.GetFromTempStorage<List<string>>("operationProduces");
		if (list == null)
		{
			list = context.GetFromTempStorage<List<string>>("globalProduces");
			if (list == null)
			{
				list = context.DefaultContentType;
				if (list == null)
				{
					int num = 1;
					list = new List<string>(num);
					CollectionsMarshal.SetCount(list, num);
					Span<string> span = CollectionsMarshal.AsSpan(list);
					int num2 = 0;
					span[num2] = "application/octet-stream";
					num2++;
				}
			}
		}
		IOpenApiSchema fromTempStorage = context.GetFromTempStorage<IOpenApiSchema>("responseSchema", response);
		Dictionary<string, IOpenApiExample> fromTempStorage2 = context.GetFromTempStorage<Dictionary<string, IOpenApiExample>>("examples", response);
		foreach (string item in list)
		{
			if (response.Content.TryGetValue(item, out IOpenApiMediaType value) && value is OpenApiMediaType openApiMediaType)
			{
				if (fromTempStorage != null)
				{
					openApiMediaType.Schema = fromTempStorage;
					ProcessAnyFields(mapNode, openApiMediaType, _mediaTypeAnyFields);
				}
			}
			else
			{
				OpenApiMediaType value2 = new OpenApiMediaType
				{
					Schema = fromTempStorage,
					Examples = fromTempStorage2
				};
				response.Content.Add(item, value2);
			}
		}
		context.SetTempStorage("responseSchema", null, response);
		context.SetTempStorage("examples", null, response);
		context.SetTempStorage("responseProducesSet", true, response);
	}

	private static void LoadResponseExamplesExtension(OpenApiResponse response, ParseNode node, OpenApiDocument? hostDocument)
	{
		Dictionary<string, IOpenApiExample> value = LoadExamplesExtension(node);
		node.Context.SetTempStorage("examples", value, response);
	}

	private static Dictionary<string, IOpenApiExample> LoadExamplesExtension(ParseNode node)
	{
		MapNode mapNode = node.CheckMapNode("x-examples");
		Dictionary<string, IOpenApiExample> dictionary = new Dictionary<string, IOpenApiExample>();
		foreach (PropertyNode item in mapNode)
		{
			OpenApiExample openApiExample = new OpenApiExample();
			foreach (PropertyNode item2 in item.Value.CheckMapNode(item.Name))
			{
				switch (item2.Name.ToLowerInvariant())
				{
				case "summary":
					openApiExample.Summary = item2.Value.GetScalarValue();
					break;
				case "description":
					openApiExample.Description = item2.Value.GetScalarValue();
					break;
				case "value":
					openApiExample.Value = item2.Value.CreateAny();
					break;
				case "externalValue":
					openApiExample.ExternalValue = item2.Value.GetScalarValue();
					break;
				}
			}
			dictionary.Add(item.Name, openApiExample);
		}
		return dictionary;
	}

	private static void LoadExamples(OpenApiResponse response, ParseNode node, OpenApiDocument? hostDocument)
	{
		foreach (PropertyNode item in node.CheckMapNode("examples"))
		{
			LoadExample(response, item.Name, item.Value);
		}
	}

	private static void LoadExample(OpenApiResponse response, string mediaType, ParseNode node)
	{
		JsonNode example = node.CreateAny();
		if (response.Content == null)
		{
			IDictionary<string, IOpenApiMediaType> dictionary = (response.Content = new Dictionary<string, IOpenApiMediaType>());
		}
		OpenApiMediaType openApiMediaType2;
		if (response.Content.TryGetValue(mediaType, out IOpenApiMediaType value) && value is OpenApiMediaType openApiMediaType)
		{
			openApiMediaType2 = openApiMediaType;
		}
		else
		{
			openApiMediaType2 = new OpenApiMediaType
			{
				Schema = node.Context.GetFromTempStorage<IOpenApiSchema>("responseSchema", response)
			};
			response.Content.Add(mediaType, openApiMediaType2);
		}
		openApiMediaType2.Example = example;
	}

	public static IOpenApiResponse LoadResponse(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("response");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			return new OpenApiResponseReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
		}
		OpenApiResponse openApiResponse = new OpenApiResponse();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiResponse, _responseFixedFields, _responsePatternFields, hostDocument);
		}
		if (openApiResponse.Content?.Values != null)
		{
			foreach (OpenApiMediaType item2 in openApiResponse.Content.Values.OfType<OpenApiMediaType>())
			{
				if (item2.Schema != null)
				{
					ProcessAnyFields(mapNode, item2, _mediaTypeAnyFields);
				}
			}
		}
		return openApiResponse;
	}

	public static IOpenApiSchema LoadSchema(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("schema");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			(string, string) referenceIdAndExternalResource = GetReferenceIdAndExternalResource(referencePointer);
			return new OpenApiSchemaReference(referenceIdAndExternalResource.Item1, hostDocument, referenceIdAndExternalResource.Item2);
		}
		OpenApiSchema openApiSchema = new OpenApiSchema();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiSchema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument);
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
		return openApiSchema;
	}

	public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node, OpenApiDocument hostDocument)
	{
		MapNode mapNode = node.CheckMapNode("security");
		OpenApiSecurityRequirement openApiSecurityRequirement = new OpenApiSecurityRequirement();
		foreach (PropertyNode item in mapNode)
		{
			OpenApiSecuritySchemeReference openApiSecuritySchemeReference = LoadSecuritySchemeByReference(hostDocument, item.Name);
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

	private static OpenApiSecuritySchemeReference LoadSecuritySchemeByReference(OpenApiDocument? openApiDocument, string schemeName)
	{
		return new OpenApiSecuritySchemeReference(schemeName, openApiDocument);
	}

	public static IOpenApiSecurityScheme LoadSecurityScheme(ParseNode node, OpenApiDocument hostDocument)
	{
		_flowValue = null;
		_flow = new OpenApiOAuthFlow();
		MapNode mapNode = node.CheckMapNode("securityScheme");
		OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(openApiSecurityScheme, _securitySchemeFixedFields, _securitySchemePatternFields, hostDocument);
		}
		if (_flowValue == "implicit")
		{
			openApiSecurityScheme.Flows = new OpenApiOAuthFlows
			{
				Implicit = _flow
			};
		}
		else if (_flowValue == "password")
		{
			openApiSecurityScheme.Flows = new OpenApiOAuthFlows
			{
				Password = _flow
			};
		}
		else if (_flowValue == "application")
		{
			openApiSecurityScheme.Flows = new OpenApiOAuthFlows
			{
				ClientCredentials = _flow
			};
		}
		else if (_flowValue == "accessCode")
		{
			openApiSecurityScheme.Flows = new OpenApiOAuthFlows
			{
				AuthorizationCode = _flow
			};
		}
		return openApiSecurityScheme;
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
		Dictionary<string, PropertyNode> dictionary = mapNode.ToDictionary((PropertyNode x) => x.Name, (PropertyNode x) => x);
		foreach (string item in fixedFieldMap.Keys.Union(dictionary.Keys))
		{
			if (dictionary.TryGetValue(item, out var value))
			{
				value.ParseField(domainObject, fixedFieldMap, patternFieldMap, doc);
			}
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

	public static JsonNode LoadAny(ParseNode node, OpenApiDocument hostDocument)
	{
		return node.CreateAny();
	}

	private static IOpenApiExtension LoadExtension(string name, ParseNode node)
	{
		if (node.Context.ExtensionParsers != null && node.Context.ExtensionParsers.TryGetValue(name, out Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension> value))
		{
			return value(node.CreateAny(), OpenApiSpecVersion.OpenApi2_0);
		}
		return new JsonNodeExtension(node.CreateAny());
	}

	private static string? LoadString(ParseNode node)
	{
		return node.GetScalarValue();
	}

	private static (string, string?) GetReferenceIdAndExternalResource(string pointer)
	{
		string[] array = pointer.Split('/');
		string item = array[array.Count() - 1];
		string item2 = ((!array[0].StartsWith("#", StringComparison.OrdinalIgnoreCase)) ? (array[0] + "/" + array[1].TrimEnd('#')) : null);
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
