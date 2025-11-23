using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader;

internal class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode, OpenApiDocument>>
{
}
