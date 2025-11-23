using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader;

internal class PatternFieldMap<T> : Dictionary<Func<string, bool>, Action<T, string, ParseNode, OpenApiDocument>>
{
}
