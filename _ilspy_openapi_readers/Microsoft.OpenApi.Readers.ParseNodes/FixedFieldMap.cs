using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode>>
{
}
