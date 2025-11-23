using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader;

internal class AnyFieldMap<T> : Dictionary<string, AnyFieldMapParameter<T>>
{
}
