using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Readers.Interface;

public interface IStreamLoader
{
	Task<Stream> LoadAsync(Uri uri);

	[Obsolete("Use the Async overload")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	Stream Load(Uri uri);
}
