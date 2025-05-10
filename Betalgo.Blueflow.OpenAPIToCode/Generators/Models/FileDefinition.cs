namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
///     Represents the definition of a file for code generation purposes.
///     Contains collections of classes, enums, and other elements to be rendered in a file.
/// </summary>
public class FileDefinition
{
    /// <summary>
    ///     The namespace for the generated file.
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    ///     Additional usings required for the file.
    /// </summary>
    public List<string> Usings { get; set; } = new();

    public List<string> Content { get; set; }
}