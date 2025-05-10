namespace Betalgo.Blueflow.OpenAPIToCode;

/// <summary>
///     Configuration for BlueFlowOpenApi main class.
/// </summary>
public class BlueFlowOpenApiConfiguration
{
    /// <summary>
    ///     Gets or sets the OpenApiToCodeParser configuration.
    /// </summary>
    public BlueFlowOpenApiEngineConfiguration ParserConfiguration { get; set; } = new();

    /// <summary>
    ///     Gets or sets the project name (namespace) for generated code.
    /// </summary>
    public string ProjectName { get; set; } = "DefaultNamespace";

    /// <summary>
    ///     Gets or sets the output directory for generated files.
    /// </summary>
    public string OutputDirectory { get; set; } = "./output";
}