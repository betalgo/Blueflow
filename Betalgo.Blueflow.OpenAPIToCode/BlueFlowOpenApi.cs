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

    public void Start()
    {
        _parser.Start2();
    }
}