namespace Betalgo.Blueflow.OpenAPIToCode.Generators;

/// <summary>
///     Interface for normalizing documentation strings.
/// </summary>
public interface IDocumentationNormalizerService
{
    /// <summary>
    ///     Normalizes a documentation string.
    /// </summary>
    /// <param name="input">The input documentation string.</param>
    /// <returns>The normalized documentation string.</returns>
    string? Normalize(string? input);

    void SetBaseDomain(string? baseDomain);
}