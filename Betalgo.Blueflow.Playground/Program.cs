using Betalgo.Blueflow.OpenAPIToCode;
using Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

Console.WriteLine("Betalgo.Blueflow.OpenAPIToCode - Test Client");

// Use the simple OpenAPI file from the project directory
var openApiFile = Path.Combine(Directory.GetCurrentDirectory(), "openapi.yaml");
Console.WriteLine($"Using OpenAPI file: {openApiFile}");

// Setup output directory at C:\Repos\{ProjectName}
string currentDirectory = Directory.GetCurrentDirectory();
string reposDirectory = null;

// Find the Repos directory (parent of Blueflow)
string tempPath = currentDirectory;
while (!string.IsNullOrEmpty(tempPath))
{
    string parentDir = Path.GetDirectoryName(tempPath);
    string dirName = Path.GetFileName(tempPath);
    
    if (dirName.Equals("Blueflow", StringComparison.OrdinalIgnoreCase) && 
        !string.IsNullOrEmpty(parentDir) && 
        Path.GetFileName(parentDir).Equals("Repos", StringComparison.OrdinalIgnoreCase))
    {
        reposDirectory = parentDir; // This is the Repos directory
        break;
    }
    
    tempPath = parentDir;
}

if (string.IsNullOrEmpty(reposDirectory))
{
    // Fallback to current directory if Repos directory not found
    reposDirectory = Path.GetDirectoryName(currentDirectory);
}

// Define the project name consistently throughout the application
const string projectName = "Betalgo.Ranul.AIModel.OpenAI";


// Set output directory to C:\Repos\{ProjectName}
var outputDir = Path.Combine(reposDirectory, "Betalgo.Ranul.AIModel");

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
    ProjectName = projectName,
    OutputDirectory = outputDir,
    ParserConfiguration = new()
    {
        OpenApiDocumentationPath = openApiFile,
        ProjectName = projectName,
        GenerateNestedClasses = true,
        OutputDirectory = outputDir
    }
});
await CreateSolutionAndProject(outputDir, projectName);
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