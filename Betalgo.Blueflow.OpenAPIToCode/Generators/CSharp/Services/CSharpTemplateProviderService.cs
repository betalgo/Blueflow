namespace Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp.Services;

/// <summary>
///     Default implementation of ITemplateProviderService for C# code generation.
/// </summary>
public class CSharpTemplateProviderService : ITemplateProviderService
{
    private readonly string _templatesBasePath;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CSharpTemplateProviderService" /> class.
    /// </summary>
    public CSharpTemplateProviderService()
    {
        // Use AppContext.BaseDirectory instead of Directory.GetCurrentDirectory()
        // to avoid mixing output files with source code, per .NET conventions
        _templatesBasePath = Path.Combine(AppContext.BaseDirectory, "Generators", "CSharp", "DefaultTemplates");
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CSharpTemplateProviderService" /> class with a custom templates path.
    /// </summary>
    /// <param name="templatesBasePath">The base path for templates</param>
    public CSharpTemplateProviderService(string templatesBasePath)
    {
        _templatesBasePath = templatesBasePath;
    }

    /// <inheritdoc />
    public string GetTemplateText(string templateType)
    {
        var templatePath = Path.Combine(_templatesBasePath, $"{templateType}.sbn");
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        return File.ReadAllText(templatePath);
    }

    /// <inheritdoc />
    public string GetFileTemplate()
    {
        return GetTemplateText("file_template");
    }

    /// <inheritdoc />
    public string GetPropertyTemplate()
    {
        return GetTemplateText("property_template");
    }

    /// <inheritdoc />
    public string GetClassTemplate()
    {
        return GetTemplateText("class_template");
    }

    /// <inheritdoc />
    public string GetOneOfClassTemplate()
    {
        return GetTemplateText("oneof_template");
    }

    /// <inheritdoc />
    public string GetOneOfConverterTemplate()
    {
        return GetTemplateText("oneof_converter_template");
    }

    /// <inheritdoc />
    public string GetAnyOfClassTemplate()
    {
        return GetTemplateText("anyof_template");
    }

    /// <inheritdoc />
    public string GetAnyOfConverterTemplate()
    {
        return GetTemplateText("anyof_converter_template");
    }

    /// <inheritdoc />
    public string GetConstructorTemplate()
    {
        return GetTemplateText("required_constructor_template");
    }

    /// <inheritdoc />
    public string GetStringEnumTemplate()
    {
        return GetTemplateText("enum_as_string_template");
    }

    /// <inheritdoc />
    public string GetSolutionTemplate()
    {
        return GetTemplateText("solution_template");
    }

    /// <inheritdoc />
    public string GetProjectTemplate()
    {
        return GetTemplateText("project_template");
    }

    /// <inheritdoc />
    public bool TemplateExists(string templateType)
    {
        var templatePath = Path.Combine(_templatesBasePath, $"{templateType}.sbn");
        return File.Exists(templatePath);
    }
}