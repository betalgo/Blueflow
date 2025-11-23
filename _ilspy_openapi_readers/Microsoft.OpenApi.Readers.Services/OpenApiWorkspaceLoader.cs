using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers.Services;

internal class OpenApiWorkspaceLoader
{
	private OpenApiWorkspace _workspace;

	private IStreamLoader _loader;

	private readonly OpenApiReaderSettings _readerSettings;

	public OpenApiWorkspaceLoader(OpenApiWorkspace workspace, IStreamLoader loader, OpenApiReaderSettings readerSettings)
	{
		_workspace = workspace;
		_loader = loader;
		_readerSettings = readerSettings;
	}

	internal async Task<OpenApiDiagnostic> LoadAsync(OpenApiReference reference, OpenApiDocument document, OpenApiDiagnostic diagnostic = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		_workspace.AddDocument(reference.ExternalResource, document);
		document.Workspace = _workspace;
		OpenApiRemoteReferenceCollector openApiRemoteReferenceCollector = new OpenApiRemoteReferenceCollector();
		new OpenApiWalker((OpenApiVisitorBase)(object)openApiRemoteReferenceCollector).Walk(document);
		OpenApiStreamReader reader = new OpenApiStreamReader(_readerSettings);
		if (diagnostic == null)
		{
			diagnostic = new OpenApiDiagnostic();
		}
		foreach (OpenApiReference item in openApiRemoteReferenceCollector.References)
		{
			if (!_workspace.Contains(item.ExternalResource))
			{
				ReadResult readResult = await reader.ReadAsync(await _loader.LoadAsync(new Uri(item.ExternalResource, UriKind.RelativeOrAbsolute)), cancellationToken);
				if (readResult.OpenApiDiagnostic != null)
				{
					diagnostic.AppendDiagnostic(readResult.OpenApiDiagnostic, item.ExternalResource);
				}
				if (readResult.OpenApiDocument != null)
				{
					diagnostic = await LoadAsync(item, readResult.OpenApiDocument, diagnostic, cancellationToken);
				}
			}
		}
		return diagnostic;
	}
}
