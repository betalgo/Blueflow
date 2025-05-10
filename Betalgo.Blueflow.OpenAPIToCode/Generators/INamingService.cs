using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

namespace Betalgo.Blueflow.OpenAPIToCode.Utils;

/// <summary>
///     Interface for naming conversion service.
/// </summary>
public interface INamingService
{
    /// <summary>
    ///     Converts the given name according to the specified purpose/context.
    /// </summary>
    /// <param name="name">The original name.</param>
    /// <param name="purpose">The naming purpose/context.</param>
    /// <returns>The converted name.</returns>
    string Convert(string name, NamingPurpose purpose);

    string ToPascalCase(string input);
}