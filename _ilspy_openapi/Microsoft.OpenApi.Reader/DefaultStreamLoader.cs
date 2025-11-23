using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Implementation of IInputLoader that loads streams from URIs
/// </summary>
public class DefaultStreamLoader : IStreamLoader
{
	private readonly HttpClient _httpClient;

	/// <summary>
	/// The default stream loader
	/// </summary>
	/// <param name="httpClient">The HttpClient to use to retrieve documents when needed</param>
	public DefaultStreamLoader(HttpClient httpClient)
	{
		_httpClient = Utils.CheckArgumentNull(httpClient, "httpClient");
	}

	/// <inheritdoc />
	public async Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default(CancellationToken))
	{
		Uri uri2 = ((!baseUrl.AbsoluteUri.Equals("https://openapi.net/")) ? new Uri(baseUrl, uri) : new Uri(Path.Combine(Directory.GetCurrentDirectory(), uri.ToString())));
		Uri uri3 = uri2;
		Stream result;
		switch (uri3.Scheme)
		{
		case "file":
			result = File.OpenRead(uri3.LocalPath);
			break;
		case "http":
		case "https":
			result = await _httpClient.GetStreamAsync(uri3, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			break;
		default:
			throw new ArgumentException("Unsupported scheme");
		}
		return result;
	}
}
