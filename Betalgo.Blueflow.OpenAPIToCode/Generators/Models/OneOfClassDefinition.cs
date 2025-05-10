using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
/// Represents a oneOf wrapper class definition for code generation.
/// </summary>
public class OneOfClassDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public List<string> PropertyNames { get; set; } = new();
    public List<string> PropertyTypes { get; set; } = new();
    public List<string> PropertyJsonNames { get; set; } = new();
    public string ConverterName { get; set; } = string.Empty;
    
    public class Variant
    {
        public string TokenType { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
    }

    public List<Variant> Variants { get; set; } = new();
}
