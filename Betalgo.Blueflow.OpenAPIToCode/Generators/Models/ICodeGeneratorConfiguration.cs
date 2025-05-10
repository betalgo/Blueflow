namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
///     Configuration options for code generators.
/// </summary>
public interface ICodeGeneratorConfiguration
{
    public string? ClassNamePrefix { get; set; }
    public string? ClassNameSuffix { get; set; }
    public string? DocumentationBaseDomain { get; set; }
}