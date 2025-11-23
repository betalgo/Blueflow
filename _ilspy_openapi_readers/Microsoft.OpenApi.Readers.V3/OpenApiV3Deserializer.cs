using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3;

internal static class OpenApiV3Deserializer
{
	private static readonly FixedFieldMap<OpenApiCallback> _callbackFixedFields = new FixedFieldMap<OpenApiCallback>();

	private static readonly PatternFieldMap<OpenApiCallback> _callbackPatternFields = new PatternFieldMap<OpenApiCallback>
	{
		{
			(string s) => !s.StartsWith("x-"),
			delegate(OpenApiCallback o, string p, ParseNode n)
			{
				o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n));
			}
		},
		{
			(string s) => s.StartsWith("x-"),
			delegate(OpenApiCallback o, string p, ParseNode n)
			{
				OpenApiExtensibleExtensions.AddExtension<OpenApiCallback>(o, p, LoadExtension(p, n));
			}
		}
	};

	private static FixedFieldMap<OpenApiComponents> _componentsFixedFields = new FixedFieldMap<OpenApiComponents>
	{
		{
			"schemas",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.Schemas = n.CreateMapWithReference<OpenApiSchema>((ReferenceType)0, (Func<MapNode, OpenApiSchema>)LoadSchema);
			}
		},
		{
			"responses",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.Responses = n.CreateMapWithReference<OpenApiResponse>((ReferenceType)1, (Func<MapNode, OpenApiResponse>)LoadResponse);
			}
		},
		{
			"parameters",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.Parameters = n.CreateMapWithReference<OpenApiParameter>((ReferenceType)2, (Func<MapNode, OpenApiParameter>)LoadParameter);
			}
		},
		{
			"examples",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.Examples = n.CreateMapWithReference<OpenApiExample>((ReferenceType)3, (Func<MapNode, OpenApiExample>)LoadExample);
			}
		},
		{
			"requestBodies",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.RequestBodies = n.CreateMapWithReference<OpenApiRequestBody>((ReferenceType)4, (Func<MapNode, OpenApiRequestBody>)LoadRequestBody);
			}
		},
		{
			"headers",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.Headers = n.CreateMapWithReference<OpenApiHeader>((ReferenceType)5, (Func<MapNode, OpenApiHeader>)LoadHeader);
			}
		},
		{
			"securitySchemes",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.SecuritySchemes = n.CreateMapWithReference<OpenApiSecurityScheme>((ReferenceType)6, (Func<MapNode, OpenApiSecurityScheme>)LoadSecurityScheme);
			}
		},
		{
			"links",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.Links = n.CreateMapWithReference<OpenApiLink>((ReferenceType)7, (Func<MapNode, OpenApiLink>)LoadLink);
			}
		},
		{
			"callbacks",
			delegate(OpenApiComponents o, ParseNode n)
			{
				o.Callbacks = n.CreateMapWithReference<OpenApiCallback>((ReferenceType)8, (Func<MapNode, OpenApiCallback>)LoadCallback);
			}
		}
	};

	private static PatternFieldMap<OpenApiComponents> _componentsPatternFields = new PatternFieldMap<OpenApiComponents> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiComponents o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiComponents>(o, p, LoadExtension(p, n));
		}
	} };

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
			"email",
			delegate(OpenApiContact o, ParseNode n)
			{
				o.Email = n.GetScalarValue();
			}
		},
		{
			"url",
			delegate(OpenApiContact o, ParseNode n)
			{
				o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
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

	private static readonly FixedFieldMap<OpenApiDiscriminator> _discriminatorFixedFields = new FixedFieldMap<OpenApiDiscriminator>
	{
		{
			"propertyName",
			delegate(OpenApiDiscriminator o, ParseNode n)
			{
				o.PropertyName = n.GetScalarValue();
			}
		},
		{
			"mapping",
			delegate(OpenApiDiscriminator o, ParseNode n)
			{
				o.Mapping = n.CreateSimpleMap(LoadString);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiDiscriminator> _discriminatorPatternFields = new PatternFieldMap<OpenApiDiscriminator>();

	private static FixedFieldMap<OpenApiDocument> _openApiFixedFields = new FixedFieldMap<OpenApiDocument>
	{
		{
			"openapi",
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
			"servers",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.Servers = n.CreateList(LoadServer);
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
			"components",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.Components = LoadComponents(n);
			}
		},
		{
			"tags",
			delegate(OpenApiDocument o, ParseNode n)
			{
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Expected O, but got Unknown
				o.Tags = n.CreateList(LoadTag);
				foreach (OpenApiTag tag in o.Tags)
				{
					tag.Reference = new OpenApiReference
					{
						Id = tag.Name,
						Type = (ReferenceType)9
					};
				}
			}
		},
		{
			"externalDocs",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.ExternalDocs = LoadExternalDocs(n);
			}
		},
		{
			"security",
			delegate(OpenApiDocument o, ParseNode n)
			{
				o.SecurityRequirements = n.CreateList(LoadSecurityRequirement);
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

	private static readonly FixedFieldMap<OpenApiEncoding> _encodingFixedFields = new FixedFieldMap<OpenApiEncoding>
	{
		{
			"contentType",
			delegate(OpenApiEncoding o, ParseNode n)
			{
				o.ContentType = n.GetScalarValue();
			}
		},
		{
			"headers",
			delegate(OpenApiEncoding o, ParseNode n)
			{
				o.Headers = n.CreateMap(LoadHeader);
			}
		},
		{
			"style",
			delegate(OpenApiEncoding o, ParseNode n)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				o.Style = StringExtensions.GetEnumFromDisplayName<ParameterStyle>(n.GetScalarValue());
			}
		},
		{
			"explode",
			delegate(OpenApiEncoding o, ParseNode n)
			{
				o.Explode = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"allowReserved",
			delegate(OpenApiEncoding o, ParseNode n)
			{
				o.AllowReserved = bool.Parse(n.GetScalarValue());
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiEncoding> _encodingPatternFields = new PatternFieldMap<OpenApiEncoding> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiEncoding o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiEncoding>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiExample> _exampleFixedFields = new FixedFieldMap<OpenApiExample>
	{
		{
			"summary",
			delegate(OpenApiExample o, ParseNode n)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiExample o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"value",
			delegate(OpenApiExample o, ParseNode n)
			{
				o.Value = n.CreateAny();
			}
		},
		{
			"externalValue",
			delegate(OpenApiExample o, ParseNode n)
			{
				o.ExternalValue = n.GetScalarValue();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiExample> _examplePatternFields = new PatternFieldMap<OpenApiExample> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiExample o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiExample>(o, p, LoadExtension(p, n));
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
			"required",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Required = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"deprecated",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Deprecated = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"allowEmptyValue",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.AllowEmptyValue = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"allowReserved",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.AllowReserved = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"style",
			delegate(OpenApiHeader o, ParseNode n)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				o.Style = StringExtensions.GetEnumFromDisplayName<ParameterStyle>(n.GetScalarValue());
			}
		},
		{
			"explode",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Explode = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"content",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Content = n.CreateMap(LoadMediaType);
			}
		},
		{
			"schema",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Schema = LoadSchema(n);
			}
		},
		{
			"examples",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Examples = n.CreateMap(LoadExample);
			}
		},
		{
			"example",
			delegate(OpenApiHeader o, ParseNode n)
			{
				o.Example = n.CreateAny();
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

	public static FixedFieldMap<OpenApiInfo> InfoFixedFields = new FixedFieldMap<OpenApiInfo>
	{
		{
			"title",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.Title = n.GetScalarValue();
			}
		},
		{
			"version",
			delegate(OpenApiInfo o, ParseNode n)
			{
				o.Version = n.GetScalarValue();
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
		}
	};

	public static PatternFieldMap<OpenApiInfo> InfoPatternFields = new PatternFieldMap<OpenApiInfo> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiInfo o, string k, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiInfo>(o, k, LoadExtension(k, n));
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

	private static readonly FixedFieldMap<OpenApiLink> _linkFixedFields = new FixedFieldMap<OpenApiLink>
	{
		{
			"operationRef",
			delegate(OpenApiLink o, ParseNode n)
			{
				o.OperationRef = n.GetScalarValue();
			}
		},
		{
			"operationId",
			delegate(OpenApiLink o, ParseNode n)
			{
				o.OperationId = n.GetScalarValue();
			}
		},
		{
			"parameters",
			delegate(OpenApiLink o, ParseNode n)
			{
				o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper);
			}
		},
		{
			"requestBody",
			delegate(OpenApiLink o, ParseNode n)
			{
				o.RequestBody = LoadRuntimeExpressionAnyWrapper(n);
			}
		},
		{
			"description",
			delegate(OpenApiLink o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"server",
			delegate(OpenApiLink o, ParseNode n)
			{
				o.Server = LoadServer(n);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiLink> _linkPatternFields = new PatternFieldMap<OpenApiLink> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiLink o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiLink>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiMediaType> _mediaTypeFixedFields = new FixedFieldMap<OpenApiMediaType>
	{
		{
			"schema",
			delegate(OpenApiMediaType o, ParseNode n)
			{
				o.Schema = LoadSchema(n);
			}
		},
		{
			"examples",
			delegate(OpenApiMediaType o, ParseNode n)
			{
				o.Examples = n.CreateMap(LoadExample);
			}
		},
		{
			"example",
			delegate(OpenApiMediaType o, ParseNode n)
			{
				o.Example = n.CreateAny();
			}
		},
		{
			"encoding",
			delegate(OpenApiMediaType o, ParseNode n)
			{
				o.Encoding = n.CreateMap(LoadEncoding);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiMediaType> _mediaTypePatternFields = new PatternFieldMap<OpenApiMediaType> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiMediaType o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiMediaType>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields = new AnyFieldMap<OpenApiMediaType> { 
	{
		"example",
		new AnyFieldMapParameter<OpenApiMediaType>((OpenApiMediaType s) => s.Example, delegate(OpenApiMediaType s, IOpenApiAny v)
		{
			s.Example = v;
		}, (OpenApiMediaType s) => s.Schema)
	} };

	private static readonly AnyMapFieldMap<OpenApiMediaType, OpenApiExample> _mediaTypeAnyMapOpenApiExampleFields = new AnyMapFieldMap<OpenApiMediaType, OpenApiExample> { 
	{
		"examples",
		new AnyMapFieldMapParameter<OpenApiMediaType, OpenApiExample>((OpenApiMediaType m) => m.Examples, (OpenApiExample e) => e.Value, delegate(OpenApiExample e, IOpenApiAny v)
		{
			e.Value = v;
		}, (OpenApiMediaType m) => m.Schema)
	} };

	private static readonly FixedFieldMap<OpenApiOAuthFlow> _oAuthFlowFixedFields = new FixedFieldMap<OpenApiOAuthFlow>
	{
		{
			"authorizationUrl",
			delegate(OpenApiOAuthFlow o, ParseNode n)
			{
				o.AuthorizationUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"tokenUrl",
			delegate(OpenApiOAuthFlow o, ParseNode n)
			{
				o.TokenUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"refreshUrl",
			delegate(OpenApiOAuthFlow o, ParseNode n)
			{
				o.RefreshUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"scopes",
			delegate(OpenApiOAuthFlow o, ParseNode n)
			{
				o.Scopes = n.CreateSimpleMap(LoadString);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiOAuthFlow> _oAuthFlowPatternFields = new PatternFieldMap<OpenApiOAuthFlow> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiOAuthFlow o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiOAuthFlow>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiOAuthFlows> _oAuthFlowsFixedFields = new FixedFieldMap<OpenApiOAuthFlows>
	{
		{
			"implicit",
			delegate(OpenApiOAuthFlows o, ParseNode n)
			{
				o.Implicit = LoadOAuthFlow(n);
			}
		},
		{
			"password",
			delegate(OpenApiOAuthFlows o, ParseNode n)
			{
				o.Password = LoadOAuthFlow(n);
			}
		},
		{
			"clientCredentials",
			delegate(OpenApiOAuthFlows o, ParseNode n)
			{
				o.ClientCredentials = LoadOAuthFlow(n);
			}
		},
		{
			"authorizationCode",
			delegate(OpenApiOAuthFlows o, ParseNode n)
			{
				o.AuthorizationCode = LoadOAuthFlow(n);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiOAuthFlows> _oAuthFlowsPatternFields = new PatternFieldMap<OpenApiOAuthFlows> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiOAuthFlows o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiOAuthFlows>(o, p, LoadExtension(p, n));
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
			"requestBody",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.RequestBody = LoadRequestBody(n);
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
			"callbacks",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Callbacks = n.CreateMap(LoadCallback);
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
		},
		{
			"servers",
			delegate(OpenApiOperation o, ParseNode n)
			{
				o.Servers = n.CreateList(LoadServer);
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

	private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields = new FixedFieldMap<OpenApiParameter>
	{
		{
			"name",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Name = n.GetScalarValue();
			}
		},
		{
			"in",
			delegate(OpenApiParameter o, ParseNode n)
			{
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				string scalarValue = n.GetScalarValue();
				if ((from ParameterLocation e in Enum.GetValues(typeof(ParameterLocation))
					select EnumExtensions.GetDisplayName((Enum)(object)e)).Contains(scalarValue))
				{
					o.In = StringExtensions.GetEnumFromDisplayName<ParameterLocation>(n.GetScalarValue());
				}
				else
				{
					o.In = null;
				}
			}
		},
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
			"allowReserved",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.AllowReserved = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"style",
			delegate(OpenApiParameter o, ParseNode n)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				o.Style = StringExtensions.GetEnumFromDisplayName<ParameterStyle>(n.GetScalarValue());
			}
		},
		{
			"explode",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Explode = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"schema",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Schema = LoadSchema(n);
			}
		},
		{
			"content",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Content = n.CreateMap(LoadMediaType);
			}
		},
		{
			"examples",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Examples = n.CreateMap(LoadExample);
			}
		},
		{
			"example",
			delegate(OpenApiParameter o, ParseNode n)
			{
				o.Example = n.CreateAny();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields = new PatternFieldMap<OpenApiParameter> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiParameter o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiParameter>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields = new AnyFieldMap<OpenApiParameter> { 
	{
		"example",
		new AnyFieldMapParameter<OpenApiParameter>((OpenApiParameter s) => s.Example, delegate(OpenApiParameter s, IOpenApiAny v)
		{
			s.Example = v;
		}, (OpenApiParameter s) => s.Schema)
	} };

	private static readonly AnyMapFieldMap<OpenApiParameter, OpenApiExample> _parameterAnyMapOpenApiExampleFields = new AnyMapFieldMap<OpenApiParameter, OpenApiExample> { 
	{
		"examples",
		new AnyMapFieldMapParameter<OpenApiParameter, OpenApiExample>((OpenApiParameter m) => m.Examples, (OpenApiExample e) => e.Value, delegate(OpenApiExample e, IOpenApiAny v)
		{
			e.Value = v;
		}, (OpenApiParameter m) => m.Schema)
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
			"summary",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.Summary = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
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
		{
			"trace",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.AddOperation((OperationType)7, LoadOperation(n));
			}
		},
		{
			"servers",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.Servers = n.CreateList(LoadServer);
			}
		},
		{
			"parameters",
			delegate(OpenApiPathItem o, ParseNode n)
			{
				o.Parameters = n.CreateList(LoadParameter);
			}
		}
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

	private static readonly FixedFieldMap<OpenApiRequestBody> _requestBodyFixedFields = new FixedFieldMap<OpenApiRequestBody>
	{
		{
			"description",
			delegate(OpenApiRequestBody o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"content",
			delegate(OpenApiRequestBody o, ParseNode n)
			{
				o.Content = n.CreateMap(LoadMediaType);
			}
		},
		{
			"required",
			delegate(OpenApiRequestBody o, ParseNode n)
			{
				o.Required = bool.Parse(n.GetScalarValue());
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiRequestBody> _requestBodyPatternFields = new PatternFieldMap<OpenApiRequestBody> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiRequestBody o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiRequestBody>(o, p, LoadExtension(p, n));
		}
	} };

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
		{
			"content",
			delegate(OpenApiResponse o, ParseNode n)
			{
				o.Content = n.CreateMap(LoadMediaType);
			}
		},
		{
			"links",
			delegate(OpenApiResponse o, ParseNode n)
			{
				o.Links = n.CreateMap(LoadLink);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields = new PatternFieldMap<OpenApiResponse> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiResponse o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiResponse>(o, p, LoadExtension(p, n));
		}
	} };

	public static FixedFieldMap<OpenApiResponses> ResponsesFixedFields = new FixedFieldMap<OpenApiResponses>();

	public static PatternFieldMap<OpenApiResponses> ResponsesPatternFields = new PatternFieldMap<OpenApiResponses>
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
			"oneOf",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.OneOf = n.CreateList(LoadSchema);
			}
		},
		{
			"anyOf",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.AnyOf = n.CreateList(LoadSchema);
			}
		},
		{
			"not",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Not = LoadSchema(n);
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
			"nullable",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Nullable = bool.Parse(n.GetScalarValue());
			}
		},
		{
			"discriminator",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Discriminator = LoadDiscriminator(n);
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
			"writeOnly",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.WriteOnly = bool.Parse(n.GetScalarValue());
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
		},
		{
			"deprecated",
			delegate(OpenApiSchema o, ParseNode n)
			{
				o.Deprecated = bool.Parse(n.GetScalarValue());
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

	private static readonly FixedFieldMap<OpenApiSecurityScheme> _securitySchemeFixedFields = new FixedFieldMap<OpenApiSecurityScheme>
	{
		{
			"type",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				o.Type = StringExtensions.GetEnumFromDisplayName<SecuritySchemeType>(n.GetScalarValue());
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
			"scheme",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				o.Scheme = n.GetScalarValue();
			}
		},
		{
			"bearerFormat",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				o.BearerFormat = n.GetScalarValue();
			}
		},
		{
			"openIdConnectUrl",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				o.OpenIdConnectUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
			}
		},
		{
			"flows",
			delegate(OpenApiSecurityScheme o, ParseNode n)
			{
				o.Flows = LoadOAuthFlows(n);
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

	private static readonly FixedFieldMap<OpenApiServer> _serverFixedFields = new FixedFieldMap<OpenApiServer>
	{
		{
			"url",
			delegate(OpenApiServer o, ParseNode n)
			{
				o.Url = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiServer o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		},
		{
			"variables",
			delegate(OpenApiServer o, ParseNode n)
			{
				o.Variables = n.CreateMap(LoadServerVariable);
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiServer> _serverPatternFields = new PatternFieldMap<OpenApiServer> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiServer o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiServer>(o, p, LoadExtension(p, n));
		}
	} };

	private static readonly FixedFieldMap<OpenApiServerVariable> _serverVariableFixedFields = new FixedFieldMap<OpenApiServerVariable>
	{
		{
			"enum",
			delegate(OpenApiServerVariable o, ParseNode n)
			{
				o.Enum = n.CreateSimpleList((ValueNode s) => s.GetScalarValue());
			}
		},
		{
			"default",
			delegate(OpenApiServerVariable o, ParseNode n)
			{
				o.Default = n.GetScalarValue();
			}
		},
		{
			"description",
			delegate(OpenApiServerVariable o, ParseNode n)
			{
				o.Description = n.GetScalarValue();
			}
		}
	};

	private static readonly PatternFieldMap<OpenApiServerVariable> _serverVariablePatternFields = new PatternFieldMap<OpenApiServerVariable> { 
	{
		(string s) => s.StartsWith("x-"),
		delegate(OpenApiServerVariable o, string p, ParseNode n)
		{
			OpenApiExtensibleExtensions.AddExtension<OpenApiServerVariable>(o, p, LoadExtension(p, n));
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
				o.Namespace = new Uri(n.GetScalarValue(), UriKind.Absolute);
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

	public static OpenApiCallback LoadCallback(ParseNode node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("callback");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiCallback>((ReferenceType)8, referencePointer);
		}
		OpenApiCallback val = new OpenApiCallback();
		ParseMap(mapNode, val, _callbackFixedFields, _callbackPatternFields);
		return val;
	}

	public static OpenApiComponents LoadComponents(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("components");
		OpenApiComponents val = new OpenApiComponents();
		ParseMap(mapNode, val, _componentsFixedFields, _componentsPatternFields);
		return val;
	}

	public static OpenApiContact LoadContact(ParseNode node)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		MapNode mapNode = node as MapNode;
		OpenApiContact val = new OpenApiContact();
		ParseMap(mapNode, val, _contactFixedFields, _contactPatternFields);
		return val;
	}

	public static OpenApiDiscriminator LoadDiscriminator(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("discriminator");
		OpenApiDiscriminator val = new OpenApiDiscriminator();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _discriminatorFixedFields, _discriminatorPatternFields);
		}
		return val;
	}

	public static OpenApiDocument LoadOpenApi(RootNode rootNode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		OpenApiDocument val = new OpenApiDocument();
		ParseMap(rootNode.GetMap(), val, _openApiFixedFields, _openApiPatternFields);
		return val;
	}

	public static OpenApiEncoding LoadEncoding(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("encoding");
		OpenApiEncoding val = new OpenApiEncoding();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _encodingFixedFields, _encodingPatternFields);
		}
		return val;
	}

	public static OpenApiExample LoadExample(ParseNode node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("example");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiExample>((ReferenceType)3, referencePointer);
		}
		OpenApiExample val = new OpenApiExample();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _exampleFixedFields, _examplePatternFields);
		}
		return val;
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("header");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiHeader>((ReferenceType)5, referencePointer);
		}
		OpenApiHeader val = new OpenApiHeader();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _headerFixedFields, _headerPatternFields);
		}
		return val;
	}

	public static OpenApiInfo LoadInfo(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("Info");
		OpenApiInfo val = new OpenApiInfo();
		new List<string> { "title", "version" };
		ParseMap(mapNode, val, InfoFixedFields, InfoPatternFields);
		return val;
	}

	internal static OpenApiLicense LoadLicense(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("License");
		OpenApiLicense val = new OpenApiLicense();
		ParseMap(mapNode, val, _licenseFixedFields, _licensePatternFields);
		return val;
	}

	public static OpenApiLink LoadLink(ParseNode node)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("link");
		OpenApiLink val = new OpenApiLink();
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiLink>((ReferenceType)7, referencePointer);
		}
		ParseMap(mapNode, val, _linkFixedFields, _linkPatternFields);
		return val;
	}

	public static OpenApiMediaType LoadMediaType(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("content");
		OpenApiMediaType val = new OpenApiMediaType();
		ParseMap(mapNode, val, _mediaTypeFixedFields, _mediaTypePatternFields);
		ProcessAnyFields(mapNode, val, _mediaTypeAnyFields);
		ProcessAnyMapFields(mapNode, val, _mediaTypeAnyMapOpenApiExampleFields);
		return val;
	}

	public static OpenApiOAuthFlow LoadOAuthFlow(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("OAuthFlow");
		OpenApiOAuthFlow val = new OpenApiOAuthFlow();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _oAuthFlowFixedFields, _oAuthFlowPatternFields);
		}
		return val;
	}

	public static OpenApiOAuthFlows LoadOAuthFlows(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("OAuthFlows");
		OpenApiOAuthFlows val = new OpenApiOAuthFlows();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _oAuthFlowsFixedFields, _oAuthFlowsPatternFields);
		}
		return val;
	}

	internal static OpenApiOperation LoadOperation(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("Operation");
		OpenApiOperation val = new OpenApiOperation();
		ParseMap(mapNode, val, _operationFixedFields, _operationPatternFields);
		return val;
	}

	private static OpenApiTag LoadTagByReference(ParsingContext context, string tagName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_002c: Expected O, but got Unknown
		return new OpenApiTag
		{
			UnresolvedReference = true,
			Reference = new OpenApiReference
			{
				Type = (ReferenceType)9,
				Id = tagName
			}
		};
	}

	public static OpenApiParameter LoadParameter(ParseNode node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("parameter");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiParameter>((ReferenceType)2, referencePointer);
		}
		OpenApiParameter val = new OpenApiParameter();
		ParseMap(mapNode, val, _parameterFixedFields, _parameterPatternFields);
		ProcessAnyFields(mapNode, val, _parameterAnyFields);
		ProcessAnyMapFields(mapNode, val, _parameterAnyMapOpenApiExampleFields);
		return val;
	}

	public static OpenApiPathItem LoadPathItem(ParseNode node)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("PathItem");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiPathItem>((ReferenceType)10, referencePointer);
		}
		OpenApiPathItem val = new OpenApiPathItem();
		ParseMap(mapNode, val, _pathItemFixedFields, _pathItemPatternFields);
		return val;
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

	public static OpenApiRequestBody LoadRequestBody(ParseNode node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("requestBody");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiRequestBody>((ReferenceType)4, referencePointer);
		}
		OpenApiRequestBody val = new OpenApiRequestBody();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _requestBodyFixedFields, _requestBodyPatternFields);
		}
		return val;
	}

	public static OpenApiResponse LoadResponse(ParseNode node)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("response");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiResponse>((ReferenceType)1, referencePointer);
		}
		new List<string>().Add("description");
		OpenApiResponse val = new OpenApiResponse();
		ParseMap(mapNode, val, _responseFixedFields, _responsePatternFields);
		return val;
	}

	public static OpenApiResponses LoadResponses(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("Responses");
		OpenApiResponses val = new OpenApiResponses();
		ParseMap(mapNode, val, ResponsesFixedFields, ResponsesPatternFields);
		return val;
	}

	public static OpenApiSchema LoadSchema(ParseNode node)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("schema");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return new OpenApiSchema
			{
				UnresolvedReference = true,
				Reference = node.Context.VersionService.ConvertToOpenApiReference(referencePointer, (ReferenceType)0)
			};
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
			List<string> value = item.Value.CreateSimpleList((ValueNode valueNode) => valueNode.GetScalarValue());
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("securityScheme");
		string referencePointer = mapNode.GetReferencePointer();
		if (referencePointer != null)
		{
			return mapNode.GetReferencedObject<OpenApiSecurityScheme>((ReferenceType)6, referencePointer);
		}
		OpenApiSecurityScheme val = new OpenApiSecurityScheme();
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(val, _securitySchemeFixedFields, _securitySchemePatternFields);
		}
		return val;
	}

	public static OpenApiServer LoadServer(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("server");
		OpenApiServer val = new OpenApiServer();
		ParseMap(mapNode, val, _serverFixedFields, _serverPatternFields);
		return val;
	}

	public static OpenApiServerVariable LoadServerVariable(ParseNode node)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MapNode mapNode = node.CheckMapNode("serverVariable");
		OpenApiServerVariable val = new OpenApiServerVariable();
		ParseMap(mapNode, val, _serverVariableFixedFields, _serverVariablePatternFields);
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

	private static void ParseMap<T>(MapNode mapNode, T domainObject, FixedFieldMap<T> fixedFieldMap, PatternFieldMap<T> patternFieldMap)
	{
		if (mapNode == null)
		{
			return;
		}
		foreach (PropertyNode item in mapNode)
		{
			item.ParseField(domainObject, fixedFieldMap, patternFieldMap);
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
		//IL_009c: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		foreach (string item in anyListFieldMap.Keys.ToList())
		{
			try
			{
				List<IOpenApiAny> list = new List<IOpenApiAny>();
				mapNode.Context.StartObject(item);
				foreach (IOpenApiAny item2 in anyListFieldMap[item].PropertyGetter(domainObject))
				{
					list.Add(OpenApiAnyConverter.GetSpecificOpenApiAny(item2, anyListFieldMap[item].SchemaGetter(domainObject)));
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

	private static void ProcessAnyMapFields<T, U>(MapNode mapNode, T domainObject, AnyMapFieldMap<T, U> anyMapFieldMap)
	{
		//IL_00d4: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		foreach (string item in anyMapFieldMap.Keys.ToList())
		{
			try
			{
				new List<IOpenApiAny>();
				mapNode.Context.StartObject(item);
				foreach (KeyValuePair<string, U> item2 in anyMapFieldMap[item].PropertyMapGetter(domainObject))
				{
					mapNode.Context.StartObject(item2.Key);
					if (item2.Value != null)
					{
						IOpenApiAny specificOpenApiAny = OpenApiAnyConverter.GetSpecificOpenApiAny(anyMapFieldMap[item].PropertyGetter(item2.Value), anyMapFieldMap[item].SchemaGetter(domainObject));
						anyMapFieldMap[item].PropertySetter(item2.Value, specificOpenApiAny);
					}
				}
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

	private static RuntimeExpression LoadRuntimeExpression(ParseNode node)
	{
		return RuntimeExpression.Build(node.GetScalarValue());
	}

	private static RuntimeExpressionAnyWrapper LoadRuntimeExpressionAnyWrapper(ParseNode node)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		string scalarValue = node.GetScalarValue();
		if (scalarValue != null && scalarValue.StartsWith("$"))
		{
			return new RuntimeExpressionAnyWrapper
			{
				Expression = RuntimeExpression.Build(scalarValue)
			};
		}
		return new RuntimeExpressionAnyWrapper
		{
			Any = OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny())
		};
	}

	public static IOpenApiAny LoadAny(ParseNode node)
	{
		return OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny());
	}

	private static IOpenApiExtension LoadExtension(string name, ParseNode node)
	{
		if (node.Context.ExtensionParsers.TryGetValue(name, out var value))
		{
			IOpenApiExtension val = value(OpenApiAnyConverter.GetSpecificOpenApiAny(node.CreateAny()), (OpenApiSpecVersion)1);
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
