using Betalgo.Blueflow.OpenAPIToCode.Generators;
using Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

namespace Betalgo.Blueflow.OpenAPIToCode;

/// <summary>
///     Main entry point for BlueFlow OpenAPI operations.
/// </summary>
public class BlueFlowOpenApi
{
    private readonly BlueFlowOpenApiConfiguration _configuration;
    private readonly BlueFlowOpenApiEngine _parser;
    private string? _yamlFilePath;

    public BlueFlowOpenApi(ICodeGenerator codeGenerator, BlueFlowOpenApiConfiguration? configuration = null)
    {
        _configuration = configuration ?? new BlueFlowOpenApiConfiguration();
        var config = _configuration.ParserConfiguration;

        _parser = new(codeGenerator, config);
    }

    public BlueFlowOpenApi(CodeLanguage codeLanguage, BlueFlowOpenApiConfiguration? configuration = null)
    {
        var codeGenerator = codeLanguage switch
        {
            CodeLanguage.CSharp => new CSharpCodeGenerator(new CSharpCodeGeneratorConfiguration()),
            _ => throw new ArgumentOutOfRangeException(nameof(codeLanguage), codeLanguage, null)
        };
        _configuration = configuration ?? new BlueFlowOpenApiConfiguration();
        var config = _configuration.ParserConfiguration;
        _parser = new(codeGenerator, config);
    }

    public void LoadYaml(string yamlFilePath)
    {
        if (string.IsNullOrWhiteSpace(yamlFilePath))
            throw new ArgumentException("YAML file path cannot be null or empty.", nameof(yamlFilePath));
        if (!File.Exists(yamlFilePath))
            throw new FileNotFoundException($"YAML file not found: {yamlFilePath}");
        _yamlFilePath = yamlFilePath;
    }


    ///// <summary>
    ///// Processes an OpenAPI file, assigns x-blueflow-id GUIDs to all schemas, enums, and fields, and writes the result to a new file.
    ///// </summary>
    ///// <param name="inputOpenApiFilePath">Path to the input OpenAPI file.</param>
    ///// <param name="outputOpenApiFilePath">Path to the output OpenAPI file.</param>
    //public void GenerateOpenApiWithBlueflowIds(string inputOpenApiFilePath, string outputOpenApiFilePath)
    //{
    //    _parser.GenerateOpenApiWithBlueflowIds(inputOpenApiFilePath, outputOpenApiFilePath);
    //}
    public void Start()
    {
        _parser.Start2();
    }
}