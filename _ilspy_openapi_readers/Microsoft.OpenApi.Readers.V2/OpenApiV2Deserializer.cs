using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers.V2;

internal static class OpenApiV2Deserializer
{
	private static FixedFieldMap<OpenApiContact> _contactFixedFields = new FixedFieldMap<OpenApiContact>
	{
		{
			"name",
			delegate(OpenApiContact o, ParseNode n)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"url",
			delegate(OpenApiContact o, ParseNode n)
			{
				o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"email",
			delegate(OpenApiContact o, ParseNode n)
			{
				o.Email = n.GetScalarValue();
			}
		}
	};

	private static PatternFieldMap<OpenApiContact> _contactPatternFields = new PatternFieldMap<OpenApiContact> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiContact o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiContact>(o, p, LoadExtension(p, n));
		}
	} };

	private static FixedFieldMap<OpenApiDocument> _openApiFixedFields = new FixedFieldMap<OpenApiDocument>
	{
		{
			"swagger",
			delegate
			{
			}
		},
		{
			"info",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.Info = LoadInfo(n);
			}
		},
		{
			"host",
			delegate(OpenApiDocument _, ParseNode n)
			{
				n.Context.SetTempStorage("host", n.GetScalarValue());
			}
		},
		{
			"basePath",
			delegate(OpenApiDocument _, ParseNode n)
			{
				n.Context.SetTempStorage("basePath", n.GetScalarValue());
			}
		},
		{
			"schemes",
			delegate(OpenApiDocument _, ParseNode n)
			{
				n.Context.SetTempStorage("schemes", n.CreateSimpleList((ValueNode s) => s.GetScalarValue()));
			}
		},
		{
			"consumes",
			delegate(OpenApiDocument _, ParseNode n)
			{
				List<string> list = n.CreateSimpleList((ValueNode s) => s.GetScalarValue());
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("globalConsumes", list);
				}
			}
		},
		{
			"produces",
			delegate(OpenApiDocument _, ParseNode n)
			{
				List<string> list = n.CreateSimpleList((ValueNode s) => s.GetScalarValue());
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("globalProduces", list);
				}
			}
		},
		{
			"paths",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.Paths = LoadPaths(n);
			}
		},
		{
			"definitions",
			delegate(OpenApiDocument o, ParseNode n)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Expected O, but got Unknown
				if (o.Components == null)
				{
					o.Components = new OpenApiComponents();
				}
				o.Components.Schemas = n.CreateMapWithReference<OpenApiSchema>((ReferenceType)0, (Func<MapNode, OpenApiSchema>)LoadSchema);
			}
		},
		{
			"parameters",
			delegate(OpenApiDocument o, ParseNode n)
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Expected O, but got Unknown
				if (o.Components == null)
				{
					o.Components = new OpenApiComponents();
				}
				o.Components.Parameters = n.CreateMapWithReference<OpenApiParameter>((ReferenceType)2, (Func<MapNode, OpenApiParameter>)LoadParameter);
				o.Components.RequestBodies = n.CreateMapWithReference<OpenApiRequestBody>((ReferenceType)4, (Func<MapNode, OpenApiRequestBody>)delegate(MapNode p)
				{
					OpenApiParameter val = LoadParameter(p, loadRequestBody: true);
					return (val != null) ? CreateRequestBody(n.Context, val) : null;
				});
			}
		},
		{
			"responses",
			delegate(OpenApiDocument o, ParseNode n)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Expected O, but got Unknown
				if (o.Components == null)
				{
					o.Components = new OpenApiComponents();
				}
				o.Components.Responses = n.CreateMapWithReference<OpenApiResponse>((ReferenceType)1, (Func<MapNode, OpenApiResponse>)LoadResponse);
			}
		},
		{
			"securityDefinitions",
			delegate(OpenApiDocument o, ParseNode n)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Expected O, but got Unknown
				if (o.Components == null)
				{
					o.Components = new OpenApiComponents();
				}
				o.Components.SecuritySchemes = n.CreateMapWithReference<OpenApiSecurityScheme>((ReferenceType)6, (Func<MapNode, OpenApiSecurityScheme>)LoadSecurityScheme);
			}
		},
		{
			"security",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.SecurityRequirements = n.CreateList(LoadSecurityRequirement);
			}
		},
		{
			"tags",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.Tags = n.CreateList(LoadTag);
			}
		},
		{
			"externalDocs",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.ExternalDocs = LoadExternalDocs(n);
			}
		}
	};

	private static PatternFieldMap<OpenApiDocument> _openApiPatternFields = new PatternFieldMap<OpenApiDocument> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiDocument o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiDocument>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields = new FixedFieldMap<OpenApiExternalDocs>
	{
		{
			"description",
			delegate(OpenApiExternalDocs o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"url",
			delegate(OpenApiExternalDocs o, ParseNode n)
			{
				o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiExternalDocs> _externalDocsPatternFields = new PatternFieldMap<OpenApiExternalDocs> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiExternalDocs o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiExternalDocs>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new FixedFieldMap<OpenApiHeader>
	{
		{
			"description",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"type",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Type = n.GetScalarValue();
			}
		},
		{
			"format",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Format = n.GetScalarValue();
			}
		},
		{
			"items",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Items = LoadSchema(n);
			}
		},
		{
			"collectionFormat",
			delegate(OpenApiHeader o, ParseNode n)
			{
				LoadStyle(o, n.GetScalarValue());
			}
		},
		{
			"default",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Default = n.CreateAny();
			}
		},
		{
			"maximum",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue);
			}
		},
		{
			"exclusiveMaximum",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).ExclusiveMaximum = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"minimum",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MinValue);
			}
		},
		{
			"exclusiveMinimum",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).ExclusiveMinimum = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"maxLength",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"minLength",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"pattern",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Pattern = n.GetScalarValue();
			}
		},
		{
			"maxItems",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).MaxItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"minItems",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).MinItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"uniqueItems",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).UniqueItems = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"multipleOf",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).MultipleOf = decimal.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"enum",
			delegate(OpenApiHeader o, ParseNode n)
			{
				GetOrCreateSchema(o).Enum = n.CreateListOfAny();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new PatternFieldMap<OpenApiHeader> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiHeader o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiHeader>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiHeader> _headerAnyFields = new AnyFieldMap<OpenApiHeader> { 
	{
		"default",
		new AnyFieldMapParameter<OpenApiHeader>(delegate(OpenApiHeader p)
		{
			OpenApiSchema schema = p.Schema;
			return (schema == null) ? null : schema.Default;
		}, delegate(OpenApiHeader p, IOpenApiAny v)
		{
			if (p.Schema != null)
			{
				p.Schema.Default = v;
			}
		}, (OpenApiHeader p) => p.Schema)
	} };

	private static readonly AnyListFieldMap<OpenApiHeader> _headerAnyListFields = new AnyListFieldMap<OpenApiHeader> { 
	{
		"enum",
		new AnyListFieldMapParameter<OpenApiHeader>(delegate(OpenApiHeader p)
		{
			OpenApiSchema schema = p.Schema;
			return (schema == null) ? null : schema.Enum;
		}, delegate(OpenApiHeader p, IList<IOpenApiAny> v)
		{
			if (p.Schema != null)
			{
				p.Schema.Enum = v;
			}
		}, (OpenApiHeader p) => p.Schema)
	} };

	private static FixedFieldMap<OpenApiInfo> _infoFixedFields = new FixedFieldMap<OpenApiInfo>
	{
		{
			"title",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.Title = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"termsOfService",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.TermsOfService = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"contact",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.Contact = LoadContact(n);
			}
		},
		{
			"license",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.License = LoadLicense(n);
			}
		},
		{
			"version",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.Version = n.GetScalarValue();
			}
		}
	};

	private static PatternFieldMap<OpenApiInfo> _infoPatternFields = new PatternFieldMap<OpenApiInfo> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiInfo o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiInfo>(o, p, LoadExtension(p, n));
		}
	} };

	private static FixedFieldMap<OpenApiLicense> _licenseFixedFields = new FixedFieldMap<OpenApiLicense>
	{
		{
			"name",
			delegate(OpenApiLicense o, ParseNode n)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"url",
			delegate(OpenApiLicense o, ParseNode n)
			{
				o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		}
	};

	private static PatternFieldMap<OpenApiLicense> _licensePatternFields = new PatternFieldMap<OpenApiLicense> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiLicense o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiLicense>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields = new FixedFieldMap<OpenApiOperation>
	{
		{
			"tags",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Tags = n.CreateSimpleList((ValueNode valueNode) => LoadTagByReference(valueNode.Context, valueNode.GetScalarValue()));
			}
		},
		{
			"summary",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"externalDocs",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.ExternalDocs = LoadExternalDocs(n);
			}
		},
		{
			"operationId",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.OperationId = n.GetScalarValue();
			}
		},
		{
			"parameters",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Parameters = n.CreateList(LoadParameter);
			}
		},
		{
			"consumes",
			delegate(OpenApiOperation _, ParseNode n)
			{
				List<string> list = n.CreateSimpleList((ValueNode s) => s.GetScalarValue());
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("operationConsumes", list);
				}
			}
		},
		{
			"produces",
			delegate(OpenApiOperation _, ParseNode n)
			{
				List<string> list = n.CreateSimpleList((ValueNode s) => s.GetScalarValue());
				if (list.Count > 0)
				{
					n.Context.SetTempStorage("operationProduces", list);
				}
			}
		},
		{
			"responses",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Responses = LoadResponses(n);
			}
		},
		{
			"deprecated",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Deprecated = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"security",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Security = n.CreateList(LoadSecurityRequirement);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields = new PatternFieldMap<OpenApiOperation> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiOperation o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiOperation>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiResponses> _responsesFixedFields = new FixedFieldMap<OpenApiResponses>();

	private static readonly PatternFieldMap<OpenApiResponses> _responsesPatternFields = new PatternFieldMap<OpenApiResponses>
	{
		{
			(string s) => !s.StartsWith("x-"),
			delegate(OpenApiResponses o, string p, ParseNode n)
			{
				((Dictionary<string, OpenApiResponse>)(object)o).Add(p, LoadResponse(n));
			}
		},
		{
			(string s) => s.StartsWith("x-"),
			delegate(OpenApiResponses o, string p, ParseNode n)
			{
				OpenApiExtensibleExtensions.AddExtension<OpenApiResponses>(o, p, LoadExtension(p, n));
			}
		}
	};

	private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields = new FixedFieldMap<OpenApiParameter>
	{
		{
			"name",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{ "in", ProcessIn },
		{
			"description",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"required",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Required = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"deprecated",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Deprecated = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"allowEmptyValue",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.AllowEmptyValue = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"type",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Type = n.GetScalarValue();
			}
		},
		{
			"items",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Items = LoadSchema(n);
			}
		},
		{
			"collectionFormat",
			delegate(OpenApiParameter o, ParseNode n)
			{
				LoadStyle(o, n.GetScalarValue());
			}
		},
		{
			"format",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Format = n.GetScalarValue();
			}
		},
		{
			"minimum",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MinValue);
			}
		},
		{
			"maximum",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue);
			}
		},
		{
			"maxLength",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"minLength",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"readOnly",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).ReadOnly = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"default",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Default = n.CreateAny();
			}
		},
		{
			"pattern",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Pattern = n.GetScalarValue();
			}
		},
		{
			"enum",
			delegate(OpenApiParameter o, ParseNode n)
			{
				GetOrCreateSchema(o).Enum = n.CreateListOfAny();
			}
		},
		{
			"schema",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Schema = LoadSchema(n);
			}
		},
		{ "x-examples", LoadParameterExamplesExtension }
	};

	private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields = new PatternFieldMap<OpenApiParameter> { 
	{
		(string s) => s.StartsWith("x-") && !s.Equals("x-examples", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiParameter o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiParameter>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields = new AnyFieldMap<OpenApiParameter> { 
	{
		"default",
		new AnyFieldMapParameter<OpenApiParameter>(delegate(OpenApiParameter p)
		{
			OpenApiSchema schema = p.Schema;
			return (schema == null) ? null : schema.Default;
		}, delegate(OpenApiParameter p, IOpenApiAny v)
		{
			if (p.Schema != null || v != null)
			{
				GetOrCreateSchema(p).Default = v;
			}
		}, (OpenApiParameter p) => p.Schema)
	} };

	private static readonly AnyListFieldMap<OpenApiParameter> _parameterAnyListFields = new AnyListFieldMap<OpenApiParameter> { 
	{
		"enum",
		new AnyListFieldMapParameter<OpenApiParameter>(delegate(OpenApiParameter p)
		{
			OpenApiSchema schema = p.Schema;
			return (schema == null) ? null : schema.Enum;
		}, delegate(OpenApiParameter p, IList<IOpenApiAny> v)
		{
			if (p.Schema != null || (v != null && v.Count > 0))
			{
				GetOrCreateSchema(p).Enum = v;
			}
		}, (OpenApiParameter p) => p.Schema)
	} };

	private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new FixedFieldMap<OpenApiPathItem>
	{
		{
			"$ref",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Expected O, but got Unknown
				o.Reference = new OpenApiReference
				{
					ExternalResource = n.GetScalarValue()
				};
				o.UnresolvedReference = true;
			}
		},
		{
			"get",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)0, LoadOperation(n));
			}
		},
		{
			"put",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)1, LoadOperation(n));
			}
		},
		{
			"post",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)2, LoadOperation(n));
			}
		},
		{
			"delete",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)3, LoadOperation(n));
			}
		},
		{
			"options",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)4, LoadOperation(n));
			}
		},
		{
			"head",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)5, LoadOperation(n));
			}
		},
		{
			"patch",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)6, LoadOperation(n));
			}
		},
		{ "parameters", LoadPathParameters }
	};

	private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields = new PatternFieldMap<OpenApiPathItem> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiPathItem o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiPathItem>(o, p, LoadExtension(p, n));
		}
	} };

	private static FixedFieldMap<OpenApiPaths> _pathsFixedFields = new FixedFieldMap<OpenApiPaths>();

	private static PatternFieldMap<OpenApiPaths> _pathsPatternFields = new PatternFieldMap<OpenApiPaths>
	{
		{
			(string s) => s.StartsWith("/"),
			delegate(OpenApiPaths o, string k, ParseNode n)
			{
				((Dictionary<string, OpenApiPathItem>)(object)o).Add(k, LoadPathItem(n));
			}
		},
		{
			(string s) => s.StartsWith("x-"),
			delegate(OpenApiPaths o, string p, ParseNode n)
			{
				OpenApiExtensibleExtensions.AddExtension<OpenApiPaths>(o, p, LoadExtension(p, n));
			}
		}
	};

	private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new FixedFieldMap<OpenApiResponse>
	{
		{
			"description",
			delegate(OpenApiResponse o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"headers",
			delegate(OpenApiResponse o, ParseNode n)
			{
				o.Headers = n.CreateMap(LoadHeader);
			}
		},
		{ "examples", LoadExamples },
		{ "x-examples", LoadResponseExamplesExtension },
		{
			"schema",
			delegate(OpenApiResponse o, ParseNode n)
			{
				n.Context.SetTempStorage("responseSchema", LoadSchema(n), o);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields = new PatternFieldMap<OpenApiResponse> { 
	{
		(string s) => s.StartsWith("x-") && !s.Equals("x-examples", StringComparison.OrdinalIgnoreCase),
		delegate(OpenApiResponse o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiResponse>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields = new AnyFieldMap<OpenApiMediaType> { 
	{
		"example",
		new AnyFieldMapParameter<OpenApiMediaType>((OpenApiMediaType m) => m.Example, delegate(OpenApiMediaType m, IOpenApiAny v)
		{
			m.Example = v;
		}, (OpenApiMediaType m) => m.Schema)
	} };

	private static readonly FixedFieldMap<OpenApiSchema> _schemaFixedFields = new FixedFieldMap<OpenApiSchema>
	{
		{
			"title",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Title = n.GetScalarValue();
			}
		},
		{
			"multipleOf",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.MultipleOf = decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture);
			}
		},
		{
			"maximum",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Maximum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MaxValue);
			}
		},
		{
			"exclusiveMaximum",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.ExclusiveMaximum = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"minimum",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Minimum = ParserHelper.ParseDecimalWithFallbackOnOverflow(n.GetScalarValue(), decimal.MinValue);
			}
		},
		{
			"exclusiveMinimum",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.ExclusiveMinimum = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"maxLength",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"minLength",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"pattern",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Pattern = n.GetScalarValue();
			}
		},
		{
			"maxItems",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.MaxItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"minItems",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.MinItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"uniqueItems",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.UniqueItems = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"maxProperties",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.MaxProperties = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"minProperties",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.MinProperties = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
			}
		},
		{
			"required",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Required = new HashSet<string>(n.CreateSimpleList((ValueNode valueNode) => valueNode.GetScalarValue()));
			}
		},
		{
			"enum",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Enum = n.CreateListOfAny();
			}
		},
		{
			"type",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Type = n.GetScalarValue();
			}
		},
		{
			"allOf",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.AllOf = n.CreateList(LoadSchema);
			}
		},
		{
			"items",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Items = LoadSchema(n);
			}
		},
		{
			"properties",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Properties = n.CreateMap(LoadSchema);
			}
		},
		{
			"additionalProperties",
			delegate(OpenApiSchema o, ParseNode n)
			{
				if (n is ValueNode)
				{
					o.AdditionalPropertiesAllowed = bool.Parse(n.GetScalarValue());
				}
				else
				{
					o.AdditionalProperties = LoadSchema(n);
				}
			}
		},
		{
			"description",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"format",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Format = n.GetScalarValue();
			}
		},
		{
			"default",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Default = n.CreateAny();
			}
		},
		{
			"discriminator",
			delegate(OpenApiSchema o, ParseNode n)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Expected O, but got Unknown
				o.Discriminator = new OpenApiDiscriminator
				{
					PropertyName = n.GetScalarValue()
				};
			}
		},
		{
			"readOnly",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.ReadOnly = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"xml",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Xml = LoadXml(n);
			}
		},
		{
			"externalDocs",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.ExternalDocs = LoadExternalDocs(n);
			}
		},
		{
			"example",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Example = n.CreateAny();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiSchema> _schemaPatternFields = new PatternFieldMap<OpenApiSchema> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiSchema o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiSchema>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiSchema> _schemaAnyFields = new AnyFieldMap<OpenApiSchema>
	{
		{
			"default",
			new AnyFieldMapParameter<OpenApiSchema>((OpenApiSchema s) => s.Default, delegate(OpenApiSchema s, IOpenApiAny v)
			{
				s.Default = v;
			}, (OpenApiSchema s) => s)
		},
		{
			"example",
			new AnyFieldMapParameter<OpenApiSchema>((OpenApiSchema s) => s.Example, delegate(OpenApiSchema s, IOpenApiAny v)
			{
				s.Example = v;
			}, (OpenApiSchema s) => s)
		}
	};

	private static readonly AnyListFieldMap<OpenApiSchema> _schemaAnyListFields = new AnyListFieldMap<OpenApiSchema> { 
	{
		"enum",
		new AnyListFieldMapParameter<OpenApiSchema>((OpenApiSchema s) => s.Enum, delegate(OpenApiSchema s, IList<IOpenApiAny> v)
		{
			s.Enum = v;
		}, (OpenApiSchema s) => s)
	} };

	private static string _flowValue;

	private static OpenApiOAuthFlow _flow;

	private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields = new FixedFieldMap<OpenApiSecurityScheme>
	{
		{
			"type",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				switch (n.GetScalarValue())
				{
				case "basic":
					o.Type = (SecuritySchemeType)1;
					o.Scheme = "basic";
					break;
				case "apiKey":
					o.Type = (SecuritySchemeType)0;
					break;
				case "oauth2":
					o.Type = (SecuritySchemeType)2;
					break;
				}
			}
		},
		{
			"description",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"name",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"in",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				o.In = StringExtensions.GetEnumFromDisplayName<ParameterLocation>(n.GetScalarValue());
			}
		},
		{
			"flow",
			delegate(OpenApiSecurityScheme _, ParseNode n)
			{
				_flowValue = n.GetScalarValue();
			}
		},
		{
			"authorizationUrl",
			delegate(OpenApiSecurityScheme _, ParseNode n)
			{
				_flow.AuthorizationUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"tokenUrl",
			delegate(OpenApiSecurityScheme _, ParseNode n)
			{
				_flow.TokenUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"scopes",
			delegate(OpenApiSecurityScheme _, ParseNode n)
			{
				_flow.Scopes = n.CreateSimpleMap(LoadString);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiSecurityScheme> _securitySchemePatternFields = new PatternFieldMap<OpenApiSecurityScheme> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiSecurityScheme o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiSecurityScheme>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiTag> _tagFixedFields = new FixedFieldMap<OpenApiTag>
	{
		{
			"name",
			delegate(OpenApiTag o, ParseNode n)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiTag o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"externalDocs",
			delegate(OpenApiTag o, ParseNode n)
			{
				o.ExternalDocs = LoadExternalDocs(n);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiTag> _tagPatternFields = new PatternFieldMap<OpenApiTag> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiTag o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiTag>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiXml> _xmlFixedFields = new FixedFieldMap<OpenApiXml>
	{
		{
			"name",
			delegate(OpenApiXml o, ParseNode n)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"namespace",
			delegate(OpenApiXml o, ParseNode n)
			{
				if (Uri.IsWellFormedUriString(n.GetScalarValue(), UriKind.Absolute))
				{
					o.Namespace = new Uri(n.GetScalarValue(), UriKind.Absolute);
					return;
				}
				throw new OpenApiReaderException("Xml Namespace requires absolute URL. '" + n.GetScalarValue() + "' is not valid.");
			}
		},
		{
			"prefix",
			delegate(OpenApiXml o, ParseNode n)
			{
				o.Prefix = n.GetScalarValue();
			}
		},
		{
			"attribute",
			delegate(OpenApiXml o, ParseNode n)
			{
				o.Attribute = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"wrapped",
			delegate(OpenApiXml o, ParseNode n)
			{
				o.Wrapped = bool.Parse(n.GetScalarValue());
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiXml> _xmlPatternFields = new PatternFieldMap<OpenApiXml> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiXml o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiXml>(o, p, LoadExtension(p, n));
		}
	} };

	public static OpenApiContact LoadContact(ParseNode node)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		MapNode mapNode = node as MapNode;
		OpenApiContact val = new OpenApiContact();
		ParseMap(mapNode, val, _contactFixedFields, _contactPatternFields);
		return val;
	}

	private static void MakeServers(IList<OpenApiServer> servers, ParsingContext context, RootNode rootNode)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
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
			list = list ?? new List<string> { baseUrl.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped) };
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
			if (server.Url.EndsWith("/"))
			{
				server.Url = server.Url.Substring(0, server.Url.Length - 1);
			}
		}
	}

	private static string BuildUrl(string scheme, string host, string basePath)
	{
		if (string.IsNullOrEmpty(scheme) && !string.IsNullOrEmpty(host))
		{
			host = "//" + host;
		}
		int? num = null;
		if (!string.IsNullOrEmpty(host) && Enumerable.Contains(host, ':'))
		{
			string[] source = host.Split(new char[1] { ':' });
			host = source.First();
			num = int.Parse(source.Last(), CultureInfo.InvariantCulture);
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

	public static OpenApiDocument LoadOpenApi(RootNode rootNode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		OpenApiDocument val = new OpenApiDocument();
		MapNode map = rootNode.GetMap();
		ParseMap(map, val, _openApiFixedFields, _openApiPatternFields);
		if (val.Paths != null)
		{
			ProcessResponsesMediaTypes(rootNode.GetMap(), ((Dictionary<string, OpenApiPathItem>)(object)val.Paths).Values.SelectMany(delegate(OpenApiPathItem path)
			{
				IEnumerable<OpenApiOperation> enumerable = path.Operations?.Values;
				return enumerable ?? Enumerable.Empty<OpenApiOperation>();
			}).SelectMany(delegate(OpenApiOperation operation)
			{
				IEnumerable<OpenApiResponse> enumerable = ((Dictionary<string, OpenApiResponse>)(object)operation.Responses)?.Values;
				return enumerable ?? Enumerable.Empty<OpenApiResponse>();
			}), map.Context);
		}
		MapNode map2 = rootNode.GetMap();
		OpenApiComponents components = val.Components;
		ProcessResponsesMediaTypes(map2, (components == null) ? null : components.Responses?.Values, map.Context);
		if (val.Servers == null)
		{
			val.Servers = new List<OpenApiServer>();
		}
		MakeServers(val.Servers, map.Context, rootNode);
		FixRequestBodyReferences(val);
		return val;
	}

	private static void ProcessResponsesMediaTypes(MapNode mapNode, IEnumerable<OpenApiResponse> responses, ParsingContext context)
	{
		if (responses == null)
		{
			return;
		}
		foreach (OpenApiResponse response in responses)
		{
			ProcessProduces(mapNode, response, context);
			if (response.Content == null)
			{
				continue;
			}
			foreach (OpenApiMediaType value in response.Content.Values)
			{
				ProcessAnyFields(mapNode, value, _mediaTypeAnyFields);
			}
		}
	}

	private static void FixRequestBodyReferences(OpenApiDocument doc)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		OpenApiComponents components = doc.Components;
		IDictionary<string, OpenApiRequestBody> dictionary = ((components != null) ? components.RequestBodies : null);
		if (dictionary != null && dictionary.Count > 0)
		{
			OpenApiComponents components2 = doc.Components;
			new OpenApiWalker((OpenApiVisitorBase)(object)new RequestBodyReferenceFixer((components2 != null) ? components2.RequestBodies : null)).Walk(doc);
		}
	}

	private static bool IsHostValid(string host)
	{
		if (host.Contains(Uri.SchemeDelimiter))
		{
			return false;
		}
		return Uri.CheckHostName(host.Split(new char[1] { ':' }).First()) != UriHostNameType.Unknown;
	}

	public static OpenApiExternalDocs LoadExternalDocs(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("externalDocs");
		OpenApiExternalDocs val = new OpenApiExternalDocs();
		ParseMap(mapNode, val, _externalDocsFixedFields, _externalDocsPatternFields);
		return val;
	}

	public static OpenApiHeader LoadHeader(ParseNode node)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("header");
		OpenApiHeader val = new OpenApiHeader();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _headerFixedFields, _headerPatternFields);
		}
		OpenApiSchema fromTempStorage = node.Context.GetFromTempStorage<OpenApiSchema>("schema");
		if (fromTempStorage != null)
		{
			val.Schema = fromTempStorage;
			node.Context.SetTempStorage("schema", null);
		}
		ProcessAnyFields(mapNode, val, _headerAnyFields);
		ProcessAnyListFields(mapNode, val, _headerAnyListFields);
		return val;
	}

	private static void LoadStyle(OpenApiHeader header, string style)
	{
		switch (style)
		{
		case "csv":
			header.Style = (ParameterStyle)3;
			break;
		case "ssv":
			header.Style = (ParameterStyle)4;
			break;
		case "pipes":
			header.Style = (ParameterStyle)5;
			break;
		case "tsv":
			throw new NotSupportedException();
		default:
			throw new OpenApiReaderException("Unrecognized header style: " + style);
		}
	}

	public static OpenApiInfo LoadInfo(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("Info");
		OpenApiInfo val = new OpenApiInfo();
		ParseMap(mapNode, val, _infoFixedFields, _infoPatternFields);
		return val;
	}

	public static OpenApiLicense LoadLicense(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("OpenApiLicense");
		OpenApiLicense val = new OpenApiLicense();
		ParseMap(mapNode, val, _licenseFixedFields, _licensePatternFields);
		return val;
	}

	internal static OpenApiOperation LoadOperation(ParseNode node)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		node.Context.SetTempStorage("bodyParameter", null);
		node.Context.SetTempStorage("formParameters", null);
		node.Context.SetTempStorage("operationProduces", null);
		node.Context.SetTempStorage("operationConsumes", null);
		MapNode mapNode = node.CheckMapNode("Operation");
		OpenApiOperation val = new OpenApiOperation();
		ParseMap(mapNode, val, _operationFixedFields, _operationPatternFields);
		OpenApiParameter fromTempStorage = node.Context.GetFromTempStorage<OpenApiParameter>("bodyParameter");
		if (fromTempStorage != null)
		{
			val.RequestBody = CreateRequestBody(node.Context, fromTempStorage);
		}
		else
		{
			List<OpenApiParameter> fromTempStorage2 = node.Context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
			if (fromTempStorage2 != null)
			{
				val.RequestBody = CreateFormBody(node.Context, fromTempStorage2);
			}
		}
		foreach (OpenApiResponse value in ((Dictionary<string, OpenApiResponse>)(object)val.Responses).Values)
		{
			ProcessProduces(node.CheckMapNode("responses"), value, node.Context);
		}
		node.Context.SetTempStorage("operationProduces", null);
		return val;
	}

	public static OpenApiResponses LoadResponses(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("Responses");
		OpenApiResponses val = new OpenApiResponses();
		ParseMap(mapNode, val, _responsesFixedFields, _responsesPatternFields);
		return val;
	}

	private static OpenApiRequestBody CreateFormBody(ParsingContext context, List<OpenApiParameter> formParameters)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00ba: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		OpenApiMediaType mediaType = new OpenApiMediaType
		{
			Schema = new OpenApiSchema
			{
				Properties = formParameters.ToDictionary((OpenApiParameter k) => k.Name, delegate(OpenApiParameter v)
				{
					OpenApiSchema schema = v.Schema;
					schema.Description = v.Description;
					schema.Extensions = v.Extensions;
					return schema;
				}),
				Required = new HashSet<string>(from p in formParameters
					where p.Required
					select p.Name)
			}
		};
		List<string> source = context.GetFromTempStorage<List<string>>("operationConsumes") ?? context.GetFromTempStorage<List<string>>("globalConsumes") ?? new List<string> { "application/x-www-form-urlencoded" };
		OpenApiRequestBody val = new OpenApiRequestBody
		{
			Content = source.ToDictionary((string k) => k, (string _) => mediaType)
		};
		foreach (OpenApiMediaType item in val.Content.Values.Where((OpenApiMediaType x) => x.Schema != null && x.Schema.Properties.Any() && string.IsNullOrEmpty(x.Schema.Type)))
		{
			item.Schema.Type = "object";
		}
		return val;
	}

	internal static OpenApiRequestBody CreateRequestBody(ParsingContext context, OpenApiParameter bodyParameter)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ce: Expected O, but got Unknown
		List<string> source = context.GetFromTempStorage<List<string>>("operationConsumes") ?? context.GetFromTempStorage<List<string>>("globalConsumes") ?? new List<string> { "application/json" };
		OpenApiRequestBody val = new OpenApiRequestBody
		{
			Description = bodyParameter.Description,
			Required = bodyParameter.Required,
			Content = ((IEnumerable<string>)source).ToDictionary((Func<string, string>)((string k) => k), (Func<string, OpenApiMediaType>)((string _) => new OpenApiMediaType
			{
				Schema = bodyParameter.Schema,
				Examples = bodyParameter.Examples
			})),
			Extensions = bodyParameter.Extensions
		};
		val.Extensions["x-bodyName"] = (IOpenApiExtension)new OpenApiString(bodyParameter.Name);
		return val;
	}

	private static OpenApiTag LoadTagByReference(ParsingContext context, string tagName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_002c: Expected O, but got Unknown
		return new OpenApiTag
		{
			UnresolvedReference = true,
			Reference = new OpenApiReference
			{
				Id = tagName,
				Type = (ReferenceType)9
			}
		};
	}

	private static void LoadStyle(OpenApiParameter p, string v)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		switch (v)
		{
		case "csv":
			if (p.In == (ParameterLocation?)0)
			{
				p.Style = (ParameterStyle)2;
			}
			else
			{
				p.Style = (ParameterStyle)3;
			}
			break;
		case "ssv":
			p.Style = (ParameterStyle)4;
			break;
		case "pipes":
			p.Style = (ParameterStyle)5;
			break;
		case "tsv":
			throw new NotSupportedException();
		case "multi":
			p.Style = (ParameterStyle)2;
			p.Explode = true;
			break;
		}
	}

	private static void LoadParameterExamplesExtension(OpenApiParameter parameter, ParseNode node)
	{
		Dictionary<string, OpenApiExample> value = LoadExamplesExtension(node);
		node.Context.SetTempStorage("examples", value, parameter);
	}

	private static OpenApiSchema GetOrCreateSchema(OpenApiParameter p)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (p.Schema == null)
		{
			p.Schema = new OpenApiSchema();
		}
		return p.Schema;
	}

	private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (p.Schema == null)
		{
			p.Schema = new OpenApiSchema();
		}
		return p.Schema;
	}

	private static void ProcessIn(OpenApiParameter o, ParseNode n)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
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
			o.In = StringExtensions.GetEnumFromDisplayName<ParameterLocation>(scalarValue);
			break;
		default:
			o.In = null;
			break;
		}
	}

	public static OpenApiParameter LoadParameter(ParseNode node)
	{
		return LoadParameter(node, loadRequestBody: false);
	}

	public static OpenApiParameter LoadParameter(ParseNode node, bool loadRequestBody)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		node.Context.SetTempStorage("parameterIsBodyOrFormData", false);
		MapNode mapNode = node.CheckMapNode("parameter");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiParameter>((ReferenceType)2, referencePointer);
		}
		OpenApiParameter val = new OpenApiParameter();
		ParseMap(mapNode, val, _parameterFixedFields, _parameterPatternFields);
		ProcessAnyFields(mapNode, val, _parameterAnyFields);
		ProcessAnyListFields(mapNode, val, _parameterAnyListFields);
		OpenApiSchema fromTempStorage = node.Context.GetFromTempStorage<OpenApiSchema>("schema");
		if (fromTempStorage != null)
		{
			val.Schema = fromTempStorage;
			node.Context.SetTempStorage("schema", null);
		}
		Dictionary<string, OpenApiExample> fromTempStorage2 = node.Context.GetFromTempStorage<Dictionary<string, OpenApiExample>>("examples", val);
		if (fromTempStorage2 != null)
		{
			val.Examples = fromTempStorage2;
			node.Context.SetTempStorage("examples", null);
		}
		bool flag = (bool)node.Context.GetFromTempStorage<object>("parameterIsBodyOrFormData");
		if (flag && !loadRequestBody)
		{
			return null;
		}
		if (loadRequestBody && !flag)
		{
			return null;
		}
		return val;
	}

	public static OpenApiPathItem LoadPathItem(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("PathItem");
		OpenApiPathItem val = new OpenApiPathItem();
		ParseMap(mapNode, val, _pathItemFixedFields, _pathItemPatternFields);
		return val;
	}

	private static void LoadPathParameters(OpenApiPathItem pathItem, ParseNode node)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Invalid comparison between Unknown and I4
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Invalid comparison between Unknown and I4
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Invalid comparison between Unknown and I4
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Invalid comparison between Unknown and I4
		node.Context.SetTempStorage("bodyParameter", null);
		node.Context.SetTempStorage("formParameters", null);
		pathItem.Parameters = node.CreateList(LoadParameter);
		OpenApiParameter fromTempStorage = node.Context.GetFromTempStorage<OpenApiParameter>("bodyParameter");
		if (fromTempStorage != null)
		{
			OpenApiRequestBody requestBody = CreateRequestBody(node.Context, fromTempStorage);
			{
				foreach (KeyValuePair<OperationType, OpenApiOperation> item in pathItem.Operations.Where((KeyValuePair<OperationType, OpenApiOperation> x) => x.Value.RequestBody == null))
				{
					OperationType key = item.Key;
					if (key - 1 <= 1 || (int)key == 6)
					{
						item.Value.RequestBody = requestBody;
					}
				}
				return;
			}
		}
		List<OpenApiParameter> fromTempStorage2 = node.Context.GetFromTempStorage<List<OpenApiParameter>>("formParameters");
		if (fromTempStorage2 == null)
		{
			return;
		}
		OpenApiRequestBody requestBody2 = CreateFormBody(node.Context, fromTempStorage2);
		foreach (KeyValuePair<OperationType, OpenApiOperation> item2 in pathItem.Operations.Where((KeyValuePair<OperationType, OpenApiOperation> x) => x.Value.RequestBody == null))
		{
			OperationType key = item2.Key;
			if (key - 1 <= 1 || (int)key == 6)
			{
				item2.Value.RequestBody = requestBody2;
			}
		}
	}

	public static OpenApiPaths LoadPaths(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("Paths");
		OpenApiPaths val = new OpenApiPaths();
		ParseMap(mapNode, val, _pathsFixedFields, _pathsPatternFields);
		return val;
	}

	private static void ProcessProduces(MapNode mapNode, OpenApiResponse response, ParsingContext context)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		if (response.Content == null)
		{
			response.Content = new Dictionary<string, OpenApiMediaType>();
		}
		else if (context.GetFromTempStorage<bool>("responseProducesSet", response))
		{
			return;
		}
		List<string> obj = context.GetFromTempStorage<List<string>>("operationProduces") ?? context.GetFromTempStorage<List<string>>("globalProduces") ?? context.DefaultContentType ?? new List<string> { "application/octet-stream" };
		OpenApiSchema fromTempStorage = context.GetFromTempStorage<OpenApiSchema>("responseSchema", response);
		Dictionary<string, OpenApiExample> examples = context.GetFromTempStorage<Dictionary<string, OpenApiExample>>("examples", response) ?? new Dictionary<string, OpenApiExample>();
		foreach (string item in obj)
		{
			if (response.Content.TryGetValue(item, out var value))
			{
				if (fromTempStorage != null)
				{
					value.Schema = fromTempStorage;
					ProcessAnyFields(mapNode, value, _mediaTypeAnyFields);
				}
			}
			else
			{
				OpenApiMediaType value2 = new OpenApiMediaType
				{
					Schema = fromTempStorage,
					Examples = examples
				};
				response.Content.Add(item, value2);
			}
		}
		context.SetTempStorage("responseSchema", null, response);
		context.SetTempStorage("examples", null, response);
		context.SetTempStorage("responseProducesSet", true, response);
	}

	private static void LoadResponseExamplesExtension(OpenApiResponse response, ParseNode node)
	{
		Dictionary<string, OpenApiExample> value = LoadExamplesExtension(node);
		node.Context.SetTempStorage("examples", value, response);
	}

	private static Dictionary<string, OpenApiExample> LoadExamplesExtension(ParseNode node)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("x-examples");
		Dictionary<string, OpenApiExample> dictionary = new Dictionary<string, OpenApiExample>();
		foreach (PropertyNode item in mapNode)
		{
			OpenApiExample val = new OpenApiExample();
			foreach (PropertyNode item2 in item.Value.CheckMapNode(item.Name))
			{
				switch (item2.Name.ToLowerInvariant())
				{
				case "summary":
					val.Summary = item2.Value.GetScalarValue();
					break;
				case "description":
					val.Description = item2.Value.GetScalarValue();
					break;
				case "value":
					val.Value = OpenApiAnyConverter.GetSpecificOpenApiAny(item2.Value.CreateAny());
					break;
				case "externalValue":
					val.ExternalValue = item2.Value.GetScalarValue();
					break;
				}
			}
			dictionary.Add(item.Name, val);
		}
		return dictionary;
	}

	private static void LoadExamples(OpenApiResponse response, ParseNode node)
	{
		foreach (PropertyNode item in node.CheckMapNode("examples"))
		{
			LoadExample(response, item.Name, item.Value);
		}
	}

	private static void LoadExample(OpenApiResponse response, string mediaType, ParseNode node)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		IOpenApiAny example = node.CreateAny();
		if (response.Content == null)
		{
			IDictionary<string, OpenApiMediaType> dictionary = (response.Content = new Dictionary<string, OpenApiMediaType>());
		}
		OpenApiMediaType val;
		if (response.Content.TryGetValue(mediaType, out var value))
		{
			val = value;
		}
		else
		{
			val = new OpenApiMediaType
			{
				Schema = node.Context.GetFromTempStorage<OpenApiSchema>("responseSchema", response)
			};
			response.Content.Add(mediaType, val);
		}
		val.Example = example;
	}

	public static OpenApiResponse LoadResponse(ParseNode node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("response");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiResponse>((ReferenceType)1, referencePointer);
		}
		OpenApiResponse val = new OpenApiResponse();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _responseFixedFields, _responsePatternFields);
		}
		foreach (OpenApiMediaType value in val.Content.Values)
		{
			if (value.Schema != null)
			{
				ProcessAnyFields(mapNode, value, _mediaTypeAnyFields);
			}
		}
		return val;
	}

	public static OpenApiSchema LoadSchema(ParseNode node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("schema");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiSchema>((ReferenceType)0, referencePointer);
		}
		OpenApiSchema val = new OpenApiSchema();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _schemaFixedFields, _schemaPatternFields);
		}
		ProcessAnyFields(mapNode, val, _schemaAnyFields);
		ProcessAnyListFields(mapNode, val, _schemaAnyListFields);
		return val;
	}

	public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("security");
		OpenApiSecurityRequirement val = new OpenApiSecurityRequirement();
		foreach (PropertyNode item in mapNode)
		{
			OpenApiSecurityScheme val2 = LoadSecuritySchemeByReference(mapNode.Context, item.Name);
			List<string> value = item.Value.CreateSimpleList((ValueNode n2) => n2.GetScalarValue());
			if (val2 != null)
			{
				((Dictionary<OpenApiSecurityScheme, IList<string>>)(object)val).Add(val2, (IList<string>)value);
			}
			else
			{
				mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(node.Context.GetLocation(), "Scheme " + item.Name + " is not found"));
			}
		}
		return val;
	}

	private static OpenApiSecurityScheme LoadSecuritySchemeByReference(ParsingContext context, string schemeName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002b: Expected O, but got Unknown
		return new OpenApiSecurityScheme
		{
			UnresolvedReference = true,
			Reference = new OpenApiReference
			{
				Id = schemeName,
				Type = (ReferenceType)6
			}
		};
	}

	public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		_flowValue = null;
		_flow = new OpenApiOAuthFlow();
		MapNode mapNode = node.CheckMapNode("securityScheme");
		OpenApiSecurityScheme val = new OpenApiSecurityScheme();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _securitySchemeFixedFields, _securitySchemePatternFields);
		}
		if (_flowValue == "implicit")
		{
			val.Flows = new OpenApiOAuthFlows
			{
				Implicit = _flow
			};
		}
		else if (_flowValue == "password")
		{
			val.Flows = new OpenApiOAuthFlows
			{
				Password = _flow
			};
		}
		else if (_flowValue == "application")
		{
			val.Flows = new OpenApiOAuthFlows
			{
				ClientCredentials = _flow
			};
		}
		else if (_flowValue == "accessCode")
		{
			val.Flows = new OpenApiOAuthFlows
			{
				AuthorizationCode = _flow
			};
		}
		return val;
	}

	public static OpenApiTag LoadTag(ParseNode n)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = n.CheckMapNode("tag");
		OpenApiTag val = new OpenApiTag();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _tagFixedFields, _tagPatternFields);
		}
		return val;
	}

	private static void ParseMap<T>(MapNode mapNode, T domainObject, FixedFieldMap<T> fixedFieldMap, PatternFieldMap<T> patternFieldMap, List<string> requiredFields = null)
	{
		if (mapNode == null)
		{
			return;
		}
		foreach (string item in fixedFieldMap.Keys.Union(mapNode.Select((PropertyNode x) => x.Name)))
		{
			mapNode[item]?.ParseField(domainObject, fixedFieldMap, patternFieldMap);
			requiredFields?.Remove(item);
		}
	}

	private static void ProcessAnyFields<T>(MapNode mapNode, T domainObject, AnyFieldMap<T> anyFieldMap)
	{
		//IL_006a: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		foreach (string item in anyFieldMap.Keys.ToList())
		{
			try
			{
				mapNode.Context.StartObject(item);
				IOpenApiAny specificOpenApiAny = OpenApiAnyConverter.GetSpecificOpenApiAny(anyFieldMap[item].PropertyGetter(domainObject), anyFieldMap[item].SchemaGetter(domainObject));
				anyFieldMap[item].PropertySetter(domainObject, specificOpenApiAny);
			}
			catch (OpenApiException ex)
			{
				OpenApiException ex2 = ex;
				ex2.Pointer = mapNode.Context.GetLocation();
				mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(ex2));
			}
			finally
			{
				mapNode.Context.EndObject();
			}
		}
	}

	private static void ProcessAnyListFields<T>(MapNode mapNode, T domainObject, AnyListFieldMap<T> anyListFieldMap)
	{
		//IL_00ae: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		foreach (string item in anyListFieldMap.Keys.ToList())
		{
			try
			{
				List<IOpenApiAny> list = new List<IOpenApiAny>();
				mapNode.Context.StartObject(item);
				if (anyListFieldMap.TryGetValue(item, out var value))
				{
					IList<IOpenApiAny> list2 = value.PropertyGetter(domainObject);
					if (list2 != null)
					{
						foreach (IOpenApiAny item2 in list2)
						{
							list.Add(OpenApiAnyConverter.GetSpecificOpenApiAny(item2, anyListFieldMap[item].SchemaGetter(domainObject)));
						}
					}
				}
				anyListFieldMap[item].PropertySetter(domainObject, list);
			}
			catch (OpenApiException ex)
			{
				OpenApiException ex2 = ex;
				ex2.Pointer = mapNode.Context.GetLocation();
				mapNode.Context.Diagnostic.Errors.Add(new OpenApiError(ex2));
			}
			finally
			{
				mapNode.Context.EndObject();
			}
		}
	}

	public static IOpenApiAny LoadAny(ParseNode node)
	{
		return OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny());
	}

	private static IOpenApiExtension LoadExtension(string name, ParseNode node)
	{
		if (node.Context.ExtensionParsers.TryGetValue(name, out var value))
		{
			IOpenApiExtension val = value(OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny()), (OpenApiSpecVersion)0);
			if (val != null)
			{
				return val;
			}
		}
		return (IOpenApiExtension)(object)OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny());
	}

	private static string LoadString(ParseNode node)
	{
		return node.GetScalarValue();
	}

	public static OpenApiXml LoadXml(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("xml");
		OpenApiXml val = new OpenApiXml();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _xmlFixedFields, _xmlPatternFields);
		}
		return val;
	}
}
