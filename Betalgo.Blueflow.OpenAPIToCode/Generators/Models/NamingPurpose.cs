namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
///     Specifies the purpose/context for naming conversion.
/// </summary>
public enum NamingPurpose
{
    Class,
    OneOfClass,
    AnyOfClass,
    AllOfClass,
    Property,
    AsProperty,
    Enum,
    EnumMember,
    Method,
    Parameter
}