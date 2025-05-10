namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models
{
    /// <summary>
    /// Represents a string enum definition for code generation.
    /// </summary>
    public class EnumDefinition : DefinitionBase
    {
        public IEnumerable<EnumValueDefinition> Values { get; set; } = new List<EnumValueDefinition>();
    }
}
