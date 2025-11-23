using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader;

internal class OpenApiWorkspaceLoader
{
	private readonly OpenApiWorkspace _workspace;

	private readonly IStreamLoader _loader;

	private readonly OpenApiReaderSettings _readerSettings;

	public OpenApiWorkspaceLoader(OpenApiWorkspace workspace, IStreamLoader loader, OpenApiReaderSettings readerSettings)
	{
		_workspace = workspace;
		_loader = loader;
		_readerSettings = readerSettings;
	}

	internal async Task<OpenApiDiagnostic> LoadAsync(BaseOpenApiReference reference, OpenApiDocument? document, string? format = null, OpenApiDiagnostic? diagnostic = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		_workspace.AddDocumentId(reference.ExternalResource, document?.BaseUri);
		OpenApiSpecVersion specificationVersion = diagnostic?.SpecificationVersion ?? OpenApiSpecVersion.OpenApi3_0;
		if (document != null)
		{
			_workspace.RegisterComponents(document);
			document.Workspace = _workspace;
		}
		OpenApiRemoteReferenceCollector openApiRemoteReferenceCollector = new OpenApiRemoteReferenceCollector();
		new OpenApiWalker(openApiRemoteReferenceCollector).Walk(document);
		if (diagnostic == null)
		{
			diagnostic = new OpenApiDiagnostic
			{
				SpecificationVersion = specificationVersion
			};
		}
		foreach (BaseOpenApiReference item in openApiRemoteReferenceCollector.References)
		{
			if (item.ExternalResource != null && !_workspace.Contains(item.ExternalResource))
			{
				Uri uri = new Uri(item.ExternalResource, UriKind.RelativeOrAbsolute);
				ReadResult readResult = await OpenApiDocument.LoadAsync(await _loader.LoadAsync(item.HostDocument.BaseUri, uri, cancellationToken).ConfigureAwait(continueOnCapturedContext: false), format, _readerSettings, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (readResult.Diagnostic != null)
				{
					diagnostic.AppendDiagnostic(readResult.Diagnostic, item.ExternalResource);
				}
				if (readResult.Document != null)
				{
					diagnostic = await LoadAsync(item, readResult.Document, format, diagnostic, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}
		return diagnostic;
	}
}
