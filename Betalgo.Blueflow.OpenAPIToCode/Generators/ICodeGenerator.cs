using Betalgo.Blueflow.OpenAPIToCode.Utils;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;
using Microsoft.OpenApi.Models;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators;

/// <summary>
///     Interface for code generation classes.
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    ///     The naming service used for name conversions.
    /// </summary>
    INamingService NamingService { get; }

    /// <summary>
    ///     The type mapping service used for type conversions.
    /// </summary>
    ITypeMappingService TypeMappingService { get; }

    /// <summary>
    ///     The documentation normalizer service used for normalizing documentation strings.
    /// </summary>
    IDocumentationNormalizerService DocumentationNormalizerService { get; }

    /// <summary>
    ///     The configuration used for code generation.
    /// </summary>
    ICodeGeneratorConfiguration Configuration { get; }

    /// <summary>
    ///     Renders the base code or configuration for the code generator and returns a list of generated code strings.
    /// </summary>
    /// <returns>A list of generated base code strings.</returns>
    List<string> RenderBase();

    /// <summary>
    ///     Renders a class based on the provided class definition and returns the generated code as a string.
    /// </summary>
    /// <param name="classDefinition">The class definition containing all necessary information for code generation.</param>
    /// <returns>The generated class code as a string.</returns>
    string RenderClass(OpenApiSchema classDefinition, string? templateText);
    string Render(OpenApiSchema classDefinition);

    /// <summary>
    ///     Renders a property based on the provided property definition and returns the generated code as a string.
    /// </summary>
    /// <param name="propertyDefinition">The property definition containing all necessary information for code generation.</param>
    /// <returns>The generated property code as a string.</returns>
    string RenderProperty(OpenApiSchema propertyDefinition, string? templateText);

    /// <summary>
    /// Renders a string enum class based on the provided enum definition and returns the generated code as a string.
    /// </summary>
    /// <param name="enumDefinition">The enum definition representing the string enum.</param>
    /// <returns>The generated string enum class code as a string.</returns>
    string RenderStringEnum(OpenApiSchema enumDefinition, string? templateText);

    /// <summary>
    ///     Renders a file based on the provided file definition and returns the generated code as a string.
    /// </summary>
    /// <param name="fileDefinition">The file definition containing all necessary information for file-level code generation.</param>
    /// <returns>The generated file code as a string.</returns>
    string RenderFile(FileDefinition fileDefinition);
}