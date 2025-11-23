using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.MicrosoftExtensions;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Readers;

public class OpenApiReaderSettings
{
	public ReferenceResolutionSetting ReferenceResolution { get; set; } = ReferenceResolutionSetting.ResolveLocalReferences;

	public bool LoadExternalRefs { get; set; }

	public Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>> ExtensionParsers { get; set; } = new Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>>();

	public ValidationRuleSet RuleSet { get; set; } = ValidationRuleSet.GetDefaultRuleSet();

	public Uri BaseUrl { get; set; }

	public List<string> DefaultContentType { get; set; }

	public IStreamLoader CustomExternalLoader { get; set; }

	public bool LeaveStreamOpen { get; set; }

	public void AddMicrosoftExtensionParsers()
	{
		if (!ExtensionParsers.ContainsKey(OpenApiPagingExtension.Name))
		{
			ExtensionParsers.Add(OpenApiPagingExtension.Name, (IOpenApiAny i, OpenApiSpecVersion _) => (IOpenApiExtension)(object)OpenApiPagingExtension.Parse(i));
		}
		if (!ExtensionParsers.ContainsKey(OpenApiEnumValuesDescriptionExtension.Name))
		{
			ExtensionParsers.Add(OpenApiEnumValuesDescriptionExtension.Name, (IOpenApiAny i, OpenApiSpecVersion _) => (IOpenApiExtension)(object)OpenApiEnumValuesDescriptionExtension.Parse(i));
		}
		if (!ExtensionParsers.ContainsKey(OpenApiPrimaryErrorMessageExtension.Name))
		{
			ExtensionParsers.Add(OpenApiPrimaryErrorMessageExtension.Name, (IOpenApiAny i, OpenApiSpecVersion _) => (IOpenApiExtension)(object)OpenApiPrimaryErrorMessageExtension.Parse(i));
		}
		if (!ExtensionParsers.ContainsKey(OpenApiDeprecationExtension.Name))
		{
			ExtensionParsers.Add(OpenApiDeprecationExtension.Name, (IOpenApiAny i, OpenApiSpecVersion _) => (IOpenApiExtension)(object)OpenApiDeprecationExtension.Parse(i));
		}
		if (!ExtensionParsers.ContainsKey(OpenApiReservedParameterExtension.Name))
		{
			ExtensionParsers.Add(OpenApiReservedParameterExtension.Name, (IOpenApiAny i, OpenApiSpecVersion _) => (IOpenApiExtension)(object)OpenApiReservedParameterExtension.Parse(i));
		}
		if (!ExtensionParsers.ContainsKey(OpenApiEnumFlagsExtension.Name))
		{
			ExtensionParsers.Add(OpenApiEnumFlagsExtension.Name, (IOpenApiAny i, OpenApiSpecVersion _) => (IOpenApiExtension)(object)OpenApiEnumFlagsExtension.Parse(i));
		}
	}
}
