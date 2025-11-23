# Blueflow Chopper

**Blueflow Chopper** is a specialized utility designed to break down monolithic OpenAPI files into smaller, manageable YAML snippets.

This tool is particularly useful for AI-assisted development workflows where large OpenAPI specifications exceed context window limits. By splitting the specification into granular files (per path and per schema), developers can selectively feed relevant parts of the API definition to LLMs without overwhelming the context.

## Requirements

- [.NET 10 SDK (Preview)](https://dotnet.microsoft.com/download/dotnet/10.0) or newer.

## Usage

Run the tool using the .NET CLI. You can provide a local file or a remote URL as the source.

### Basic Example

```bash
dotnet run --project Blueflow.Chopper -- --input openapi.yaml --output openapi-split
```

### CLI Options

| Option | Short | Description | Default |
|--------|-------|-------------|---------|
| `--input` | `-i` | Path to the source OpenAPI YAML/JSON file. | (Auto-discover) |
| `--url` | `-u` | URL to a remote OpenAPI YAML/JSON file. | `null` |
| `--output` | `-o` | Directory where split files will be generated. | `openapi-split` |
| `--sections` | `-s` | Comma-separated list of sections to export (`paths`, `schemas`). | `paths,schemas` |
| `--clean` | N/A | Deletes the output directory before writing new files. | `false` |

### Examples

**Split from a URL:**
```bash
dotnet run --project Blueflow.Chopper -- --url https://api.example.com/openapi.yaml --clean
```

**Export only Schemas:**
```bash
dotnet run --project Blueflow.Chopper -- -i openapi.yaml -s schemas
```

## Output Structure

The tool generates a structured directory layout:

- `paths/<resource>/<method>/<operationId>.yml`
  - Contains a single OpenAPI operation definition.
- `components/schemas/<SchemaName>.yml`
  - Contains a single schema definition.
- `manifest.json`
  - A machine-readable summary of all generated files.

## Contributing

We welcome contributions to Blueflow Chopper. To ensure a smooth collaboration process, please adhere to the following guidelines:

- **Code Style**: Follow standard C# coding conventions and existing project patterns.
- **Testing**: Ensure that any new logic is covered by appropriate tests.
- **Pull Requests**: Submit clear, focused pull requests describing the problem solved or feature added. Avoid bundling unrelated changes.

Thank you for helping improve the project.

_A product of **Betalgo**._