using System;

namespace Microsoft.OpenApi;

/// <summary>
/// The Validator attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class OpenApiRuleAttribute : Attribute
{
}
