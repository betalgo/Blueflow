using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader.V2;

/// <summary>
/// The version specific implementations for OpenAPI V2.0.
/// </summary>
internal class OpenApiV2VersionService : BaseOpenApiVersionService
{
	private readonly Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> _loaders = new Dictionary<Type, Func<ParseNode, OpenApiDocument, object>>
	{
		[typeof(JsonNodeExtension)] = OpenApiV2Deserializer.LoadAny,
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

	internal override Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> Loaders => _loaders;

	/// <summary>
	/// Create Parsing Context
	/// </summary>
	/// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
	public OpenApiV2VersionService(OpenApiDiagnostic diagnostic)
		: base(diagnostic)
	{
	}

	public override OpenApiDocument LoadDocument(RootNode rootNode, Uri location)
	{
		return OpenApiV2Deserializer.LoadOpenApi(rootNode, location);
	}

	public override string GetReferenceScalarValues(MapNode mapNode, string scalarValue)
	{
		throw new InvalidOperationException();
	}
}
