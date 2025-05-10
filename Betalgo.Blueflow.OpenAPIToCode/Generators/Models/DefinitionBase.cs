namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
///     Base class for all OpenAPI definition types (class, enum, etc).
/// </summary>
public abstract class DefinitionBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<Guid> ParentIds { get; set; } = [];
}