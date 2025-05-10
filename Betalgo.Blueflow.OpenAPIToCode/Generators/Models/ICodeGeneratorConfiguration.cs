namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models
{
    /// <summary>
    ///     Configuration options for code generators.
    /// </summary>
    public interface ICodeGeneratorConfiguration
    {
        bool UseTabs { get; set; }

        /// <summary>
        ///     Example property: File header to include in generated files.
        /// </summary>
        public string? FileHeader { get; set; }

        public string? ClassNamePrefix { get; set; }
        public string? ClassNameSuffix { get; set; }
        public string? DocumentationBaseDomain { get; set; }
    }
}
