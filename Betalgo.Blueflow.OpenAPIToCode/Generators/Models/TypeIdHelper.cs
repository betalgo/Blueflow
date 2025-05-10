using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators.Models;

/// <summary>
/// Helper methods for working with type references and IDs.
/// </summary>
public static class TypeIdHelper
{
    /// <summary>
    /// Determines the TypeId for a property that references another type.
    /// </summary>
    /// <param name="prop">The OpenAPI schema for the property</param>
    /// <param name="blueflowIdNameDictionary">Dictionary mapping BlueflowIds to their names</param>
    /// <returns>The Guid of the referenced type, or null if no reference is found</returns>
    public static Guid? GetTypeIdForProperty(OpenApiSchema prop, Dictionary<Guid, string> blueflowIdNameDictionary)
    {
        // Case 1: Direct reference to another schema
        if (!string.IsNullOrEmpty(prop.Reference?.Id))
        {
            // Look for the reference in our dictionary by name
            var entries = blueflowIdNameDictionary
                .Where(kvp => kvp.Value == prop.Reference.Id)
                .ToList();
            
            if (entries.Any())
            {
                return entries.First().Key;
            }
        }
        
        // Case 2: For array items with references
        else if (prop.Type == "array" && prop.Items?.Reference != null && !string.IsNullOrEmpty(prop.Items.Reference.Id))
        {
            // Look for the items reference in our dictionary by name
            var entries = blueflowIdNameDictionary
                .Where(kvp => kvp.Value == prop.Items.Reference.Id)
                .ToList();
            
            if (entries.Any())
            {
                return entries.First().Key;
            }
        }
        
        // Case 3: For enum types identified earlier
        else if (prop.Enum != null && prop.Enum.Count > 0 && (prop.Type == "string" || prop.Type == null))
        {
            // For enums, use the enum's BlueflowId that was assigned earlier
            if (prop.Extensions != null && 
                prop.Extensions.TryGetValue("x-blueflow-id", out var enumExt) && 
                enumExt is OpenApiString enumIdStr && 
                Guid.TryParse(enumIdStr.Value, out var enumId))
            {
                return enumId;
            }
        }
        
        return null;
    }
}
