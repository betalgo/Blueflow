using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;

namespace Microsoft.OpenApi.Readers.V3;

internal class OpenApiV3VersionService : IOpenApiVersionService
{
	private static readonly char[] _pathSeparator = new char[1] { '/' };

	private Dictionary<Type, Func<ParseNode, object>> _loaders = new Dictionary<Type, Func<ParseNode, object>>
	{
		[typeof(IOpenApiAny)] = OpenApiV3Deserializer.LoadAny,
		[typeof(OpenApiCallback)] = OpenApiV3Deserializer.LoadCallback,
		[typeof(OpenApiComponents)] = OpenApiV3Deserializer.LoadComponents,
		[typeof(OpenApiContact)] = OpenApiV3Deserializer.LoadContact,
		[typeof(OpenApiEncoding)] = OpenApiV3Deserializer.LoadEncoding,
		[typeof(OpenApiExample)] = OpenApiV3Deserializer.LoadExample,
		[typeof(OpenApiExternalDocs)] = OpenApiV3Deserializer.LoadExternalDocs,
		[typeof(OpenApiHeader)] = OpenApiV3Deserializer.LoadHeader,
		[typeof(OpenApiInfo)] = OpenApiV3Deserializer.LoadInfo,
		[typeof(OpenApiLicense)] = OpenApiV3Deserializer.LoadLicense,
		[typeof(OpenApiLink)] = OpenApiV3Deserializer.LoadLink,
		[typeof(OpenApiMediaType)] = OpenApiV3Deserializer.LoadMediaType,
		[typeof(OpenApiOAuthFlow)] = OpenApiV3Deserializer.LoadOAuthFlow,
		[typeof(OpenApiOAuthFlows)] = OpenApiV3Deserializer.LoadOAuthFlows,
		[typeof(OpenApiOperation)] = OpenApiV3Deserializer.LoadOperation,
		[typeof(OpenApiParameter)] = OpenApiV3Deserializer.LoadParameter,
		[typeof(OpenApiPathItem)] = OpenApiV3Deserializer.LoadPathItem,
		[typeof(OpenApiPaths)] = OpenApiV3Deserializer.LoadPaths,
		[typeof(OpenApiRequestBody)] = OpenApiV3Deserializer.LoadRequestBody,
		[typeof(OpenApiResponse)] = OpenApiV3Deserializer.LoadResponse,
		[typeof(OpenApiResponses)] = OpenApiV3Deserializer.LoadResponses,
		[typeof(OpenApiSchema)] = OpenApiV3Deserializer.LoadSchema,
		[typeof(OpenApiSecurityRequirement)] = OpenApiV3Deserializer.LoadSecurityRequirement,
		[typeof(OpenApiSecurityScheme)] = OpenApiV3Deserializer.LoadSecurityScheme,
		[typeof(OpenApiServer)] = OpenApiV3Deserializer.LoadServer,
		[typeof(OpenApiServerVariable)] = OpenApiV3Deserializer.LoadServerVariable,
		[typeof(OpenApiTag)] = OpenApiV3Deserializer.LoadTag,
		[typeof(OpenApiXml)] = OpenApiV3Deserializer.LoadXml
	};

	public OpenApiDiagnostic Diagnostic { get; }

	public OpenApiV3VersionService(OpenApiDiagnostic diagnostic)
	{
		Diagnostic = diagnostic;
	}

	public OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type)
	{
		//IL_0098: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		string[] array;
		bool flag;
		if (!string.IsNullOrWhiteSpace(reference))
		{
			array = reference.Split(new char[1] { '#' });
			if (array.Length == 1)
			{
				if (type.HasValue)
				{
					ReferenceType valueOrDefault = type.GetValueOrDefault();
					if ((int)valueOrDefault == 6 || (int)valueOrDefault == 9)
					{
						flag = true;
						goto IL_0043;
					}
				}
				flag = false;
				goto IL_0043;
			}
			if (array.Length == 2)
			{
				if (reference.StartsWith("#"))
				{
					try
					{
						return ParseLocalReference(array[1]);
					}
					catch (OpenApiException ex)
					{
						OpenApiException ex2 = ex;
						Diagnostic.Errors.Add(new OpenApiError(ex2));
					}
				}
				string text = array[1];
				OpenApiReference val = new OpenApiReference();
				if (text.StartsWith("/components/"))
				{
					string[] array2 = array[1].Split(new char[1] { '/' });
					ReferenceType enumFromDisplayName = StringExtensions.GetEnumFromDisplayName<ReferenceType>(array2[2]);
					if (!type.HasValue)
					{
						type = enumFromDisplayName;
					}
					else if (type != (ReferenceType?)enumFromDisplayName)
					{
						throw new OpenApiException("Referenced type mismatch");
					}
					text = array2[3];
				}
				else if (text.StartsWith("/paths/"))
				{
					string[] array3 = array[1].Split(_pathSeparator, StringSplitOptions.RemoveEmptyEntries);
					if (array3.Length != 2)
					{
						throw new OpenApiException("Referenced Path mismatch");
					}
					text = array3[1].Replace("~1", "/");
				}
				else
				{
					val.IsFragrament = true;
				}
				val.ExternalResource = array[0];
				val.Type = type;
				val.Id = text;
				return val;
			}
		}
		throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, reference));
		IL_0043:
		if (!flag)
		{
			return new OpenApiReference
			{
				Type = type,
				ExternalResource = array[0]
			};
		}
		return new OpenApiReference
		{
			Type = type,
			Id = reference
		};
	}

	public OpenApiDocument LoadDocument(RootNode rootNode)
	{
		return OpenApiV3Deserializer.LoadOpenApi(rootNode);
	}

	public T LoadElement<T>(ParseNode node) where T : IOpenApiElement
	{
		return (T)_loaders[typeof(T)](node);
	}

	private OpenApiReference ParseLocalReference(string localReference)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		if (string.IsNullOrWhiteSpace(localReference))
		{
			throw new ArgumentException(string.Format(SRResource.ArgumentNullOrWhiteSpace, "localReference"));
		}
		string[] array = localReference.Split(new char[1] { '/' });
		if (array.Length == 4 && array[1] == "components")
		{
			ReferenceType enumFromDisplayName = StringExtensions.GetEnumFromDisplayName<ReferenceType>(array[2]);
			return new OpenApiReference
			{
				Type = enumFromDisplayName,
				Id = array[3]
			};
		}
		throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, localReference));
	}
}
