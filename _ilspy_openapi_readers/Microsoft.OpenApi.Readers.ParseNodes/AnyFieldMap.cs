using System.Collections.Generic;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class AnyFieldMap<T> : Dictionary<string, AnyFieldMapParameter<T>>
{
}
