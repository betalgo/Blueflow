namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models
{
    /// <summary>
    /// Modifiers for generated classes (e.g., public, abstract, sealed).
    /// </summary>
    [Flags]
    public enum ClassModifiers
    {
        None = 0,
        Public = 1 << 0,
        Internal = 1 << 1,
        Abstract = 1 << 2,
        Sealed = 1 << 3,
        Static = 1 << 4
    }
}
