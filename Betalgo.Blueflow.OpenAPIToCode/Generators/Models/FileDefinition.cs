namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models
{
    /// <summary>
    ///     Represents the definition of a file for code generation purposes.
    ///     Contains collections of classes, enums, and other elements to be rendered in a file.
    /// </summary>
    public class FileDefinition
    {
        /// <summary>
        ///     The namespace for the generated file.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     The list of class definitions to be included in the file.
        /// </summary>
        public List<ClassDefinition> Classes { get; set; } = new List<ClassDefinition>();

        /// <summary>
        ///     The list of enum definitions to be included in the file.
        /// </summary>
        public List<EnumDefinition> Enums { get; set; } = new List<EnumDefinition>();

        /// <summary>
        ///     Additional usings required for the file.
        /// </summary>
        public List<string> Usings { get; set; } = new List<string>();


        public List<string> Content { get; set; }
    }
}