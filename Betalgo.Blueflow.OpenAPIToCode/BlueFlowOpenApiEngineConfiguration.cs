namespace Betalgo.Blueflow.OpenAPIToCode;

public class BlueFlowOpenApiEngineConfiguration
{
    /// <summary>
    /// If true, discovered classes will be generated as nested classes. If false, as separate top-level classes.
    /// </summary>
    public bool GenerateNestedClasses { get; set; } = true;

    public string OpenApiDocumentationPath { get; set; }
    public string ProjectName { get; set; }
    public string OutputDirectory { get; set; }
}
