using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;

namespace Microsoft.OpenApi.Readers.V2;

internal class OpenApiV2VersionService : IOpenApiVersionService
{
	private Dictionary<Type, Func<ParseNode, object>> _loaders = new Dictionary<Type, Func<ParseNode, object>>
	{
		[typeof(IOpenApiAny)] = OpenApiV2Deserializer.LoadAny,
		[typeof(OpenApiContact)] = OpenApiV2Deserializer.LoadContact,
		[typeof(OpenApiExternalDocs)] = OpenApiV2Deserializer.LoadExternalDocs,
		[typeof(OpenApiHeader)] = OpenApiV2Deserializer.LoadHeader,
		[typeof(OpenApiInfo)] = OpenApiV2Deserializer.LoadInfo,
		[typeof(OpenApiLicense)] = OpenApiV2Deserializer.LoadLicense,
		[typeof(OpenApiOperation)] = OpenApiV2Deserializer.LoadOperation,
		[typeof(OpenApiParameter)] = OpenApiV2Deserializer.LoadParameter,
		[typeof(OpenApiPathItem)] = OpenApiV2Deserializer.LoadPathItem,
		[typeof(OpenApiPaths)] = OpenApiV2Deserializer.LoadPaths,
		[typeof(OpenApiResponse)] = OpenApiV2Deserializer.LoadResponse,
		[typeof(OpenApiResponses)] = OpenApiV2Deserializer.LoadResponses,
		[typeof(OpenApiSchema)] = OpenApiV2Deserializer.LoadSchema,
		[typeof(OpenApiSecurityRequirement)] = OpenApiV2Deserializer.LoadSecurityRequirement,
		[typeof(OpenApiSecurityScheme)] = OpenApiV2Deserializer.LoadSecurityScheme,
		[typeof(OpenApiTag)] = OpenApiV2Deserializer.LoadTag,
		[typeof(OpenApiXml)] = OpenApiV2Deserializer.LoadXml
	};

	public OpenApiDiagnostic Diagnostic { get; }

	public OpenApiV2VersionService(OpenApiDiagnostic diagnostic)
	{
		Diagnostic = diagnostic;
	}

	private static OpenApiReference ParseLocalReference(string localReference)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		if (string.IsNullOrWhiteSpace(localReference))
		{
			throw new ArgumentException(string.Format(SRResource.ArgumentNullOrWhiteSpace, "localReference"));
		}
		string[] array = localReference.Split(new char[1] { '/' });
		if (array.Length >= 3)
		{
			ReferenceType value = ParseReferenceType(array[1]);
			string id = localReference.Substring(array[0].Length + "/".Length + array[1].Length + "/".Length);
			return new OpenApiReference
			{
				Type = value,
				Id = id
			};
		}
		throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, localReference));
	}

	private static ReferenceType ParseReferenceType(string referenceTypeName)
	{
		return (ReferenceType)(referenceTypeName switch
		{
			"definitions" => 0, 
			"parameters" => 2, 
			"responses" => 1, 
			"headers" => 5, 
			"tags" => 9, 
			"securityDefinitions" => 6, 
			_ => throw new OpenApiReaderException("Unknown reference type '" + referenceTypeName + "'"), 
		});
	}

	private static ReferenceType GetReferenceTypeV2FromName(string referenceType)
	{
		return (ReferenceType)(referenceType switch
		{
			"definitions" => 0, 
			"parameters" => 2, 
			"responses" => 1, 
			"tags" => 9, 
			"securityDefinitions" => 6, 
			_ => throw new ArgumentException(), 
		});
	}

	public OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type)
	{
		//IL_009c: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		bool flag;
		if (!string.IsNullOrWhiteSpace(reference))
		{
			string[] array = reference.Split(new char[1] { '#' });
			if (array.Length == 1)
			{
				if (!type.HasValue)
				{
					return new OpenApiReference
					{
						ExternalResource = array[0]
					};
				}
				if (type.HasValue)
				{
					ReferenceType valueOrDefault = type.GetValueOrDefault();
					if ((int)valueOrDefault == 6 || (int)valueOrDefault == 9)
					{
						flag = true;
						goto IL_005b;
					}
				}
				flag = false;
				goto IL_005b;
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
						return null;
					}
				}
				string text = array[1];
				if (text.StartsWith("/definitions/"))
				{
					string[] array2 = text.Split(new char[1] { '/' });
					ReferenceType referenceTypeV2FromName = GetReferenceTypeV2FromName(array2[1]);
					if (!type.HasValue)
					{
						type = referenceTypeV2FromName;
					}
					else if (type != (ReferenceType?)referenceTypeV2FromName)
					{
						throw new OpenApiException("Referenced type mismatch");
					}
					text = array2[2];
				}
				return new OpenApiReference
				{
					ExternalResource = array[0],
					Type = type,
					Id = text
				};
			}
		}
		goto IL_0140;
		IL_0140:
		throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, reference));
		IL_005b:
		if (flag)
		{
			return new OpenApiReference
			{
				Type = type,
				Id = reference
			};
		}
		goto IL_0140;
	}

	public OpenApiDocument LoadDocument(RootNode rootNode)
	{
		return OpenApiV2Deserializer.LoadOpenApi(rootNode);
	}

	public T LoadElement<T>(ParseNode node) where T : IOpenApiElement
	{
		return (T)_loaders[typeof(T)](node);
	}
}
