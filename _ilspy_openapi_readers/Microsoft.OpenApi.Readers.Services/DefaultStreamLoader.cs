using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers.Services;

internal class DefaultStreamLoader : IStreamLoader
{
	private readonly Uri baseUrl;

	private HttpClient _httpClient = new HttpClient();

	public DefaultStreamLoader(Uri baseUrl)
	{
		this.baseUrl = baseUrl;
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Stream Load(Uri uri)
	{
		return LoadAsync(uri).GetAwaiter().GetResult();
	}

	public async Task<Stream> LoadAsync(Uri uri)
	{
		Uri uri2 = new Uri(baseUrl, uri);
		switch (uri2.Scheme)
		{
		case "file":
			return File.OpenRead(uri2.AbsolutePath);
		case "http":
		case "https":
			return await _httpClient.GetStreamAsync(uri2);
		default:
			throw new ArgumentException("Unsupported scheme");
		}
	}
}
