using Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
///     Represents a class definition for code generation.
/// </summary>
public class ClassDefinition : DefinitionBase
{
    public List<PropertyDefinition> Properties { get; set; } = new();
    public List<ClassDefinition?> NestedClasses { get; set; } = new();
    public string? BaseClass { get; set; }
    public List<string>? Interfaces { get; set; }
    public ClassModifiers Modifiers { get; set; } = ClassModifiers.Public;
    public PolyType PolyType { get; set; }
}

public enum PolyType
{
    None,
    OneOf,
    AllOf,
    AnyOf
}