using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class PatternFieldMap<T> : Dictionary<Func<string, bool>, Action<T, string, ParseNode>>
{
}
