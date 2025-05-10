namespace Betalgo.Blueflow.OpenAPIToCode.Generators;

public interface ITemplateProviderService
{
    /// <summary>
    ///     Gets the template text for a specified template type.
    /// </summary>
    /// <param name="templateType">The type of template to retrieve</param>
    /// <returns>The template text</returns>
    string GetTemplateText(string templateType);

    /// <summary>
    ///     Gets the template text for a file template.
    /// </summary>
    /// <returns>The file template text</returns>
    string GetFileTemplate();

    /// <summary>
    ///     Gets the template text for a property template.
    /// </summary>
    /// <returns>The property template text</returns>
    string GetPropertyTemplate();

    /// <summary>
    ///     Gets the template text for a class template.
    /// </summary>
    /// <returns>The class template text</returns>
    string GetClassTemplate();

    /// <summary>
    ///     Gets the template text for a OneOf class template.
    /// </summary>
    /// <returns>The OneOf class template text</returns>
    string GetOneOfClassTemplate();

    /// <summary>
    ///     Gets the template text for a OneOf converter template.
    /// </summary>
    /// <returns>The OneOf converter template text</returns>
    string GetOneOfConverterTemplate();

    /// <summary>
    ///     Gets the template text for an AnyOf class template.
    /// </summary>
    /// <returns>The AnyOf class template text</returns>
    string GetAnyOfClassTemplate();

    /// <summary>
    ///     Gets the template text for an AnyOf converter template.
    /// </summary>
    /// <returns>The AnyOf converter template text</returns>
    string GetAnyOfConverterTemplate();

    /// <summary>
    ///     Gets the template text for a constructor template.
    /// </summary>
    /// <returns>The constructor template text</returns>
    string GetConstructorTemplate();

    /// <summary>
    ///     Gets the template text for a string enum template.
    /// </summary>
    /// <returns>The string enum template text</returns>
    string GetStringEnumTemplate();

    /// <summary>
    ///     Gets the template text for a solution template.
    /// </summary>
    /// <returns>The solution template text</returns>
    string GetSolutionTemplate();

    /// <summary>
    ///     Gets the template text for a project template.
    /// </summary>
    /// <returns>The project template text</returns>
    string GetProjectTemplate();

    /// <summary>
    ///     Determines if a template exists.
    /// </summary>
    /// <param name="templateType">The type of template to check</param>
    /// <returns>True if the template exists, false otherwise</returns>
    bool TemplateExists(string templateType);
}