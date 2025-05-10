using Betalgo.Blueflow.OpenAPIToCode;
using Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

Console.WriteLine("Betalgo.Blueflow.OpenAPIToCode - Test Client");


// Use the simple OpenAPI file from the project directory
var openApiFile = Path.Combine(Directory.GetCurrentDirectory(), "openapi.yaml");
Console.WriteLine($"Using OpenAPI file: {openApiFile}");

// Setup output directory
var outputDir = Path.Combine(AppContext.BaseDirectory, "output");
Directory.CreateDirectory(outputDir);
Console.WriteLine($"Output directory: {outputDir}");

// Clean up any existing .cs files in the output directory
CleanupOutputDirectory(outputDir);
// --- Custom OpenAPI to Code Generation ---
var openApiToCodeParser = new BlueFlowOpenApi(new CSharpCodeGenerator(new CSharpCodeGeneratorConfiguration
{
    ClassNameSuffix = "Model"
}), new()
{
    ProjectName = "TestAPI",
    OutputDirectory = outputDir,
    ParserConfiguration = new()
    {
        OpenApiDocumentationPath = openApiFile,
        ProjectName = "TestAPI",
        GenerateNestedClasses = true,
        OutputDirectory = outputDir
    }
});
await CreateSolutionAndProject(outputDir, "TestAPI");
openApiToCodeParser.Start();


Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();


static async Task CreateSolutionAndProject(string outputDir, string projectNamespace)
{
    Console.WriteLine("\nCreating solution and project for generated code...");

    // Use the engine's method to generate base files (solution, project, etc.)
    var engine = new BlueFlowOpenApiEngine(new CSharpCodeGenerator(new CSharpCodeGeneratorConfiguration
    {
        ClassNameSuffix = "Model"
    }), new()
    {
        ProjectName = "TestAPI",
        OutputDirectory = outputDir,
    });
    await engine.GenerateBaseFilesIfNotExistAsync(outputDir, projectNamespace.Split('.')[0]);

    var projectName = projectNamespace.Split('.')[0];
    var solutionFile = Path.Combine(outputDir, $"{projectName}.sln");
    var projectFile = Path.Combine(outputDir, $"{projectName}.csproj");

    Console.WriteLine($"Solution created at: {solutionFile}");
    Console.WriteLine($"Project created at: {projectFile}");
}

static void CleanupOutputDirectory(string outputDir)
{
    Console.WriteLine("Cleaning up existing files in output directory...");
    var existingFiles = Directory.GetFiles(outputDir, "*.cs", SearchOption.AllDirectories);
    foreach (var file in existingFiles)
    {
        try
        {
            File.Delete(file);
            Console.WriteLine($"  Deleted: {Path.GetRelativePath(outputDir, file)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Warning: Could not delete {Path.GetRelativePath(outputDir, file)}: {ex.Message}");
        }
    }

    Console.WriteLine("Cleanup completed.");
}