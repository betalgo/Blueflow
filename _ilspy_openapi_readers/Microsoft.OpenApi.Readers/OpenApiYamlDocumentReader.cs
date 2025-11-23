using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Services;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers;

internal class OpenApiYamlDocumentReader : IOpenApiReader<YamlDocument, OpenApiDiagnostic>
{
	private readonly OpenApiReaderSettings _settings;

	public OpenApiYamlDocumentReader(OpenApiReaderSettings settings = null)
	{
		_settings = settings ?? new OpenApiReaderSettings();
	}

	public OpenApiDocument Read(YamlDocument input, out OpenApiDiagnostic diagnostic)
	{
		//IL_0070: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		diagnostic = new OpenApiDiagnostic();
		ParsingContext parsingContext = new ParsingContext(diagnostic)
		{
			ExtensionParsers = _settings.ExtensionParsers,
			BaseUrl = _settings.BaseUrl,
			DefaultContentType = _settings.DefaultContentType
		};
		OpenApiDocument val = null;
		try
		{
			val = parsingContext.Parse(input);
			if (_settings.LoadExternalRefs)
			{
				throw new InvalidOperationException("Cannot load external refs using the synchronous Read, use ReadAsync instead.");
			}
			ResolveReferences(diagnostic, val);
		}
		catch (OpenApiException ex)
		{
			OpenApiException ex2 = ex;
			diagnostic.Errors.Add(new OpenApiError(ex2));
		}
		if (_settings.RuleSet != null && _settings.RuleSet.Rules.Count > 0)
		{
			IEnumerable<OpenApiError> source = OpenApiElementExtensions.Validate((IOpenApiElement)(object)val, _settings.RuleSet);
			foreach (OpenApiValidatorError item in source.OfType<OpenApiValidatorError>())
			{
				diagnostic.Errors.Add((OpenApiError)(object)item);
			}
			foreach (OpenApiValidatorWarning item2 in source.OfType<OpenApiValidatorWarning>())
			{
				diagnostic.Warnings.Add((OpenApiError)(object)item2);
			}
		}
		return val;
	}

	public async Task<ReadResult> ReadAsync(YamlDocument input, CancellationToken cancellationToken = default(CancellationToken))
	{
		OpenApiDiagnostic diagnostic = new OpenApiDiagnostic();
		ParsingContext parsingContext = new ParsingContext(diagnostic)
		{
			ExtensionParsers = _settings.ExtensionParsers,
			BaseUrl = _settings.BaseUrl
		};
		OpenApiDocument document = null;
		try
		{
			document = parsingContext.Parse(input);
			if (_settings.LoadExternalRefs)
			{
				OpenApiDiagnostic openApiDiagnostic = await LoadExternalRefsAsync(document, cancellationToken);
				if (openApiDiagnostic != null)
				{
					diagnostic.Errors.AddRange(openApiDiagnostic.Errors);
					diagnostic.Warnings.AddRange(openApiDiagnostic.Warnings);
				}
			}
			ResolveReferences(diagnostic, document);
		}
		catch (OpenApiException ex)
		{
			OpenApiException ex2 = ex;
			diagnostic.Errors.Add(new OpenApiError(ex2));
		}
		if (_settings.RuleSet != null && _settings.RuleSet.Rules.Count > 0)
		{
			IEnumerable<OpenApiError> source = OpenApiElementExtensions.Validate((IOpenApiElement)(object)document, _settings.RuleSet);
			foreach (OpenApiValidatorError item in source.OfType<OpenApiValidatorError>())
			{
				diagnostic.Errors.Add((OpenApiError)(object)item);
			}
			foreach (OpenApiValidatorWarning item2 in source.OfType<OpenApiValidatorWarning>())
			{
				diagnostic.Warnings.Add((OpenApiError)(object)item2);
			}
		}
		return new ReadResult
		{
			OpenApiDocument = document,
			OpenApiDiagnostic = diagnostic
		};
	}

	private Task<OpenApiDiagnostic> LoadExternalRefsAsync(OpenApiDocument document, CancellationToken cancellationToken = default(CancellationToken))
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		OpenApiWorkspace val = new OpenApiWorkspace();
		DefaultStreamLoader defaultStreamLoader = new DefaultStreamLoader(_settings.BaseUrl);
		return new OpenApiWorkspaceLoader(val, _settings.CustomExternalLoader ?? defaultStreamLoader, _settings).LoadAsync(new OpenApiReference
		{
			ExternalResource = "/"
		}, document, null, cancellationToken);
	}

	private void ResolveReferences(OpenApiDiagnostic diagnostic, OpenApiDocument document)
	{
		List<OpenApiError> list = new List<OpenApiError>();
		switch (_settings.ReferenceResolution)
		{
		case ReferenceResolutionSetting.ResolveAllReferences:
			throw new ArgumentException("Resolving external references is not supported");
		case ReferenceResolutionSetting.ResolveLocalReferences:
			list.AddRange(document.ResolveReferences());
			break;
		}
		foreach (OpenApiError item in list)
		{
			diagnostic.Errors.Add(item);
		}
	}

	public T ReadFragment<T>(YamlDocument input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
	{
		//IL_0033: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		diagnostic = new OpenApiDiagnostic();
		ParsingContext parsingContext = new ParsingContext(diagnostic)
		{
			ExtensionParsers = _settings.ExtensionParsers
		};
		IOpenApiElement val = null;
		try
		{
			val = (IOpenApiElement)(object)parsingContext.ParseFragment<T>(input, version);
		}
		catch (OpenApiException ex)
		{
			OpenApiException ex2 = ex;
			diagnostic.Errors.Add(new OpenApiError(ex2));
		}
		if (_settings.RuleSet != null && _settings.RuleSet.Rules.Count > 0)
		{
			foreach (OpenApiError item in OpenApiElementExtensions.Validate(val, _settings.RuleSet))
			{
				diagnostic.Errors.Add(item);
			}
		}
		return (T)(object)val;
	}
}
