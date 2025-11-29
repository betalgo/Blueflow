using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Blueflow.Chopper.OpenApi;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Blueflow.Chopper.Cli;

internal sealed class SplitCommand : AsyncCommand<SplitCommand.Settings>
{
    private readonly IAnsiConsole _console;
    private readonly OpenApiSplitter _splitter;

    public SplitCommand()
    {
        _console = AnsiConsole.Console;
        _splitter = new(_console);
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var versionCheckTask = CheckForUpdateAsync();

        var options = settings.ToOptions();

        if (options.SourceFile == null && options.SourceUrl == null)
        {
            var file = await DiscoverInputFileAsync(cancellationToken);
            if (file == null)
            {
                return 1;
            }

            options = new()
            {
                SourceFile = file,
                SourceUrl = null,
                OutputDirectory = options.OutputDirectory,
                CleanOutput = options.CleanOutput,
                Sections = options.Sections
            };
        }

        var manifest = await _splitter.SplitAsync(options);

        RenderSummary(manifest, options);

        await versionCheckTask;

        return 0;
    }

    private async Task CheckForUpdateAsync()
    {
        try
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (version == null)
            {
                return;
            }

            var checker = new VersionChecker("Betalgo.Blueflow.Chopper", version);
            var latestVersion = await checker.CheckForUpdateAsync();

            if (latestVersion != null)
            {
                _console.WriteLine();
                _console.MarkupLine($"[yellow]A newer version of Blueflow Chopper is available: {latestVersion}[/]");
                _console.MarkupLine($"Run [green]dotnet tool update -g Betalgo.Blueflow.Chopper[/] to update.");
            }
        }
        catch
        {
            // Swallow any errors during version check to not impact main flow
        }
    }

    private async Task<string?> DiscoverInputFileAsync(CancellationToken cancellationToken)
    {
        var extensions = new[] { "*.yml", "*.yaml", "*.json" };
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidates = new List<string>();

        foreach (var ext in extensions)
        {
            candidates.AddRange(Directory.GetFiles(currentDirectory, ext, SearchOption.TopDirectoryOnly));
        }

        var openApiFiles = new List<string>();
        foreach (var candidate in candidates)
        {
            if (await IsOpenApiFileAsync(candidate, cancellationToken))
            {
                openApiFiles.Add(candidate);
            }
        }

        if (openApiFiles.Count == 0)
        {
            _console.MarkupLine("[red]No input file specified and no valid OpenAPI files found in the current directory.[/]");
            return null;
        }

        if (openApiFiles.Count == 1)
        {
            var file = openApiFiles[0];
            var fileName = Path.GetFileName(file);
            if (_console.Confirm($"Found [green]{fileName}[/]. Do you want to use it?"))
            {
                return file;
            }

            return null;
        }

        var fileNames = openApiFiles.Select(Path.GetFileName).Where(name => name != null).Cast<string>();

        const string noneOption = "None";

        var prompt = new SelectionPrompt<string>().Title("Multiple OpenAPI files found. Please select one:").PageSize(10).AddChoices(fileNames.ToList());

        prompt.AddChoice(noneOption);

        var selection = _console.Prompt(prompt);

        if (selection == noneOption)
        {
            return null;
        }

        return Path.Combine(currentDirectory, selection);
    }

    private static async Task<bool> IsOpenApiFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            // Quick check for "openapi" or "swagger" keyword in the first few lines
            // to avoid parsing entire non-openapi files.
            // For JSON, we might need to read a bit more, but checking first 1KB should be enough.

            const int bufferSize = 4096;
            var buffer = new char[bufferSize];
            using var reader = new StreamReader(filePath);
            var read = await reader.ReadAsync(buffer, 0, bufferSize);
            var content = new string(buffer, 0, read);

            // Simple heuristic
            if (content.Contains("openapi:", StringComparison.OrdinalIgnoreCase) || content.Contains("swagger:", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                // Check for "openapi": or "swagger":
                if (content.Contains("\"openapi\":", StringComparison.OrdinalIgnoreCase) || content.Contains("\"swagger\":", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private void RenderSummary(SplitManifest manifest, SplitOptions options)
    {
        var table = new Table().Centered();
        table.AddColumn("Section");
        table.AddColumn("Files");

        table.AddRow("paths", manifest.Paths.Count.ToString());
        table.AddRow("schemas", manifest.Schemas.Count.ToString());

        _console.WriteLine();
        _console.MarkupLine($"Generated files in [green]{options.OutputDirectory}[/]");
        _console.Write(table);
        _console.MarkupLine($"Manifest: [blue]{Path.Combine(options.OutputDirectory, OpenApiSplitter.ManifestFileName)}[/]");
    }

    public sealed class Settings : CommandSettings
    {
        private const string DefaultSections = "paths,schemas";

        [CommandOption("-i|--input <FILE>")]
        [Description("Path to the source OpenAPI YAML file.")]
        public string? InputFile { get; init; }

        [CommandOption("-u|--url <URL>")]
        [Description("URL to the source OpenAPI YAML file.")]
        public string? Url { get; init; }

        [CommandOption("-o|--output <DIR>")]
        [Description("Directory where split files will be created.")]
        public string OutputDirectory { get; init; } = "openapi-split";

        [CommandOption("-s|--sections <SECTIONS>")]
        [Description("Comma-separated list of sections to export (paths,schemas).")]
        public string Sections { get; init; } = DefaultSections;

        [CommandOption("--clean")]
        [Description("Delete the output directory before writing new files.")]
        public bool Clean { get; init; }

        public override ValidationResult Validate()
        {
            if (!string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(InputFile))
            {
                return ValidationResult.Error("Cannot specify both input file and URL.");
            }

            if (!string.IsNullOrWhiteSpace(Url))
            {
                if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                {
                    return ValidationResult.Error("Invalid URL. Must be HTTP or HTTPS.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(InputFile))
            {
                if (!File.Exists(InputFile))
                {
                    return ValidationResult.Error($"Input file '{InputFile}' does not exist.");
                }
            }

            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                return ValidationResult.Error("Output directory is required.");
            }

            if (!TryParseSections(Sections, out _))
            {
                return ValidationResult.Error("Sections must be a comma-separated list containing 'paths' or 'schemas'.");
            }

            return ValidationResult.Success();
        }

        public SplitOptions ToOptions()
        {
            if (!TryParseSections(Sections, out var parsedSections))
            {
                throw new InvalidOperationException("Unable to parse the sections option.");
            }

            Uri? sourceUrl = null;
            string? sourceFile = null;

            if (!string.IsNullOrWhiteSpace(Url))
            {
                sourceUrl = new(Url);
            }
            else if (!string.IsNullOrWhiteSpace(InputFile))
            {
                sourceFile = Path.GetFullPath(InputFile);
            }
            else
            {
                // Try default if it exists, or leave null for discovery
            }

            return new()
            {
                SourceFile = sourceFile,
                SourceUrl = sourceUrl,
                OutputDirectory = Path.GetFullPath(OutputDirectory),
                CleanOutput = Clean,
                Sections = parsedSections
            };
        }

        private static bool TryParseSections(string value, [NotNullWhen(true)] out IReadOnlyCollection<SplitSection>? sections)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                sections = Array.Empty<SplitSection>();
                return true;
            }

            var parsed = new HashSet<SplitSection>();
            var tokens = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var token in tokens)
            {
                switch (token.ToLowerInvariant())
                {
                    case "paths":
                        parsed.Add(SplitSection.Paths);
                        break;
                    case "schemas":
                        parsed.Add(SplitSection.Schemas);
                        break;
                    default:
                        sections = null;
                        return false;
                }
            }

            sections = parsed.ToArray();
            return true;
        }
    }
}