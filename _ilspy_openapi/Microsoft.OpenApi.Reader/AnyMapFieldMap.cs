using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader;

internal class AnyMapFieldMap<T, U> : Dictionary<string, AnyMapFieldMapParameter<T, U>>
{
}
