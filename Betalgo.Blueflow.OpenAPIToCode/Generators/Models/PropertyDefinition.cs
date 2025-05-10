namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
///     Represents a property definition for class generation.
/// </summary>
public class PropertyDefinition
{
    public string? Name { get; set; }
    public Guid? TypeId { get; set; }
    public string? Type { get; set; }
    public bool IsNullable { get; set; } = false;
    public bool IsCollection { get; set; } = false;
    public string? Summary { get; set; }
    public string JsonName { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public List<PropertyDefinition> SubTypes { get; set; } = new();
}