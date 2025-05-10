using System.Text;
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
var openApiToCodeParser = new BlueFlowOpenApi(new CSharpCodeGenerator(new CSharpCodeGeneratorConfiguration()
{
    ClassNameSuffix = "Model"
}), new BlueFlowOpenApiConfiguration()
{
    ProjectName = "TestAPI",
    OutputDirectory = outputDir,
    ParserConfiguration = new BlueFlowOpenApiEngineConfiguration()
    {
        OpenApiDocumentationPath = openApiFile,
        ProjectName = "TestAPI",
        GenerateNestedClasses = true,
        OutputDirectory = outputDir
    }
});
openApiToCodeParser.Start();
//var a = new CSharpCodeGenerator(new CSharpCodeGeneratorConfiguration()
//{
//    ClassNameSuffix = "Model"
//});
//var engine = new BlueFlowOpenApiEngine(a, new BlueFlowOpenApiEngineConfiguration()
//{
//    OpenApiDocumentationPath = openApiFile,
//    ProjectName = "TestAPI",
//    GenerateNestedClasses = true,
//    OutputDirectory = outputDir
//});
//var templatePath = Path.Combine("Generators", "CSharp", "DefaultTemplates", "oneof_template.sbn");
//var templateText = File.ReadAllText(templatePath);


;
//   

//    await CreateSolutionAndProject(outputDir, "TestAPI");

//    // --- Add x-blueflow-id GUIDs to OpenAPI ---
//    var openApiWithGuids = Path.Combine(outputDir, "openapi-with-blueflow-ids.yaml");
//    //openApiToCodeParser.GenerateOpenApiWithBlueflowIds(openApiFile, openApiWithGuids);
//    Console.WriteLine($"\n[OpenApiToCodeParser] OpenAPI file with x-blueflow-id created: {openApiWithGuids}");

//    await openApiToCodeParser.GenerateFilesAsync(openApiFile, outputDir, "TestAPI");
//    Console.WriteLine("\n[OpenApiToCodeParser] Custom code generation completed.");

//    // List generated files
//    var files = Directory.GetFiles(outputDir, "*.cs", SearchOption.AllDirectories);
//    Console.WriteLine($"\nGenerated {files.Length} file(s):");
//    foreach (var file in files)
//    {
//        Console.WriteLine($"  - {Path.GetRelativePath(outputDir, file)}");
//    }

//    // Create solution and project for the generated code
//    await CreateSolutionAndProject(outputDir, "TestAPI");
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Error: {ex.Message}");
//    if (ex.InnerException != null)
//    {
//        Console.WriteLine($"Inner error: {ex.InnerException.Message}");
//    }

//    Console.WriteLine(ex.StackTrace);
//}

Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();


    static async Task CreateSolutionAndProject(string outputDir, string projectNamespace)
    {
        Console.WriteLine("\nCreating solution and project for generated code...");

        // Use the engine's method to generate base files (solution, project, etc.)
        var engine = new BlueFlowOpenApiEngine(new CSharpCodeGenerator());
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

        // Delete all .cs files in the output directory and its subdirectories
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

        // Delete any existing solution/project files
        //var solutionFiles = Directory.GetFiles(outputDir, "*.sln", SearchOption.TopDirectoryOnly);
        //var projectFiles = Directory.GetFiles(outputDir, "*.csproj", SearchOption.TopDirectoryOnly);

        //foreach (var file in solutionFiles.Concat(projectFiles))
        //{
        //    try
        //    {
        //        File.Delete(file);
        //        Console.WriteLine($"  Deleted: {Path.GetRelativePath(outputDir, file)}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"  Warning: Could not delete {Path.GetRelativePath(outputDir, file)}: {ex.Message}");
        //    }
        //}

        Console.WriteLine("Cleanup completed.");
    }