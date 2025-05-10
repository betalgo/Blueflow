namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models
{
    /// <summary>
    /// Represents a single value in a string enum, with both C# name and original JSON name.
    /// </summary>
    public class EnumValueDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string JsonName { get; set; } = string.Empty;
    }
}
