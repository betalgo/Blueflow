using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Betalgo.Blueflow.OpenApiExtensions;

public static class SchemaHelpers
{
    public static bool IsEnum(this OpenApiSchema schema)
    {
        return schema is { Enum: { Count: > 0 }, Type: "string" or null };
    }


    public static bool IsOneOf(this OpenApiSchema schema)
    {
        return schema.OneOf.Any() && schema.OneOf.SelectMany(r=>r.Properties).Any();
    }

    public static bool IsAllOf(this OpenApiSchema schema)
    {
        return schema.AllOf.Any();
    }

    public static bool IsAnyOf(this OpenApiSchema schema)
    {
        return schema.AnyOf.Any();
    }

    public static bool HasReference(this OpenApiSchema schema)
    {
        return schema.Reference != null;
    }

    public static bool IsPoly(this OpenApiSchema schema)
    {
        return schema.IsOneOf() || schema.IsAllOf() || schema.IsAnyOf();
    }

    public static bool IsArray(this OpenApiSchema schema)
    {
        return schema.Type == "array";
    }

    public static bool IsObject(this OpenApiSchema schema)
    {
        return schema.Type == "object" || schema.Type == null;
    }

    public static bool IsObjectWithoutReference(this OpenApiSchema schema)
    {
        return schema is { Type: "object", Reference: null };
    }

    public static bool IsOneOfProperty(this OpenApiSchema schema)
    {
        return schema.GetBlueflowPropertyType() == "OneOf";
    }

    public static bool IsAllOfProperty(this OpenApiSchema schema)
    {
        return schema.GetBlueflowPropertyType() == "AllOf";
    }

    public static bool IsAnyOfProperty(this OpenApiSchema schema)
    {
        return schema.GetBlueflowPropertyType() == "AnyOf";
    }

    public static bool IsPolyTypeProperty(this OpenApiSchema schema)
    {
        return schema.IsOneOfProperty() || schema.IsAllOfProperty() || schema.IsAnyOfProperty();
    }
    public static void SetBlueflowExtension(this OpenApiSchema schema, string key, IOpenApiExtension? value)
    {
        if (value == null)
        {
            if (schema.Extensions != null && schema.Extensions.ContainsKey($"x-blueflow-{key}"))
            {
                schema.Extensions.Remove($"x-blueflow-{key}");
            }
        }
        else
        {
            schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
            schema.Extensions[$"x-blueflow-{key}"] = value;
        }

    }
    public static T? GetBlueflowExtension<T>(this OpenApiSchema schema, string key) where T : IOpenApiAny
    {
        if (schema.Extensions != null && schema.Extensions.TryGetValue($"x-blueflow-{key}", out var ext) && ext is T value)
        {
            return value;
        }
        return default;
    }

    public static void SetBlueflowId(this OpenApiSchema schema)
    {
        schema.SetBlueflowExtension("id", new OpenApiString(Guid.NewGuid().ToString()));
    }

    public static void SetAsMainComponent(this OpenApiSchema schema)
    {
        schema.SetBlueflowExtension("main", new OpenApiBoolean(true));
    }

    public static bool IsMainComponent(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiBoolean>("main")?.Value == true;
    }

    public static void SetBlueflowName(this OpenApiSchema schema, string? name)
    {
        schema.SetBlueflowExtension("name", new OpenApiString(name));
    }

    public static void SetItemOfArray(this OpenApiSchema schema)
    {
        schema.SetBlueflowExtension("is-item-of-array", new OpenApiBoolean(true));
    }

    public static void SetBlueflowPropertyType(this OpenApiSchema schema, string? name)
    {
        schema.SetBlueflowExtension("poly-type", new OpenApiString(name));
    }

    public static bool IsItemOfArray(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiBoolean>("is-item-of-array")?.Value == true;
    }

    public static string? GetBlueflowName(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiString>("name")?.Value;
    }

    public static string? GetBlueflowPropertyType(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiString>("poly-type")?.Value;
    }
    public static Guid GetBlueFlowId(this OpenApiSchema schema)
    {
        return Guid.Parse(schema.GetBlueflowExtension<OpenApiString>("id")?.Value?? Guid.NewGuid().ToString());
    }

    public static void SetBlueflowSelfKey(this OpenApiSchema schema, string? key)
    {
        if (string.IsNullOrEmpty(key)) return;

        schema.SetBlueflowExtension("self", new OpenApiString(key));
    }

    public static string? GetSelfKey(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiString>("self")?.Value;
    }

    public static string? GetBaseType(this OpenApiSchema schema)
    {
        if (schema.Properties.TryGetValue("type", out var ext) && ext.Enum.FirstOrDefault() is OpenApiString apiString)
        {
            return apiString.Value;
        }

        if (schema.Properties.TryGetValue("event", out var extenum) && extenum.Enum.FirstOrDefault() is OpenApiString enumString)
        {
            return enumString.Value;
        }

        return null;
    }

    public static IEnumerable<OpenApiSchema> FilterByObjectsWithoutReference(this IEnumerable<OpenApiSchema> schema)
    {
        return schema.Where(r => r.IsObjectWithoutReference() || r.IsEnum());
    }

    public static IEnumerable<OpenApiSchema> ExtractArrayObjectsWithoutReference(this IEnumerable<OpenApiSchema> schema)
    {
        return schema.Where(r => r.Type == "array" && r.Items.IsObjectWithoutReference()).Select(r => r.Items);
    }

    public static IEnumerable<KeyValuePair<string, OpenApiSchema>> FilterByObjectsWithoutReference(this IDictionary<string, OpenApiSchema> schema)
    {
        return schema.Where(r => r.Value.IsObjectWithoutReference() || r.Value.Type == "array" && r.Value.Items.IsObjectWithoutReference());
    }

    public static IEnumerable<OpenApiSchema> GetNestedObjects(this OpenApiSchema schema)
    {
        var list = new List<OpenApiSchema>();
        list.AddRange(schema.Properties.FilterByObjectsWithoutReference().Select(r => r.Value));
        list.AddRange(schema.Properties.Where(r => r.Value.IsEnum()).Select(r => r.Value));
        list.AddRange(schema.AnyOf.FilterByObjectsWithoutReference());
        list.AddRange(schema.OneOf.FilterByObjectsWithoutReference());
        list.AddRange(schema.AllOf.FilterByObjectsWithoutReference());

        //   list.AddRange(schema.Properties.ExtractArrayObjectsWithoutReference());
        list.AddRange(schema.Properties.Where(r => r.Value.Type == "array" && r.Value.Items.IsEnum()).Select(r => r.Value.Items));
        list.AddRange(schema.AnyOf.ExtractArrayObjectsWithoutReference());
        list.AddRange(schema.OneOf.ExtractArrayObjectsWithoutReference());
        list.AddRange(schema.AllOf.ExtractArrayObjectsWithoutReference());

        return list;
    }

    public static IList<OpenApiSchema> GetListOfPolyVariants(this OpenApiSchema schema)
    {
        var variants= schema.OneOf.ToList();
        variants.AddRange(schema.AnyOf);
        variants.AddRange(schema.AllOf);
        foreach (var openApiSchema in variants.ToList())
        {
            variants.AddRange(openApiSchema.GetListOfPolyVariants());
        }
        return variants;
    }
    // Checks if a schema (oneOf/anyOf) is only string or string-enum, and can be treated as enum
    public static bool IsPolyStringEnum(this OpenApiSchema schema)
    {
        if (!schema.IsPoly())
        {
            return false;
        }
        var variants = schema.GetListOfPolyVariants();

        if (!variants.Any()) return false;
        
        // All must be string or string-enum
        foreach (var variant in variants)
        {
            if (variant.Type != "string")
            {
                if ((variant.Type == null && variant.IsPoly()))
                {
                    continue;
                }
                return false;
            }
            // If not enum, must be plain string
            if (!variant.Enum.Any() && variant.Type == "string")
                continue;
            // If enum, must be string enum
            if (variant.Enum.Any() && variant.Type == "string")
                continue;
            return false;
        }

        return true;
    }


    // Converts a poly (oneOf/anyOf) schema to enum if it matches IsPolyStringEnum
    public static void ConvertPolyToEnumIfApplicable(this OpenApiSchema schema)
    {
        
        if (!schema.IsPolyStringEnum()) return;
        var variants = schema.GetListOfPolyVariants();
        
        schema.Type = "string";
        schema.Enum = new List<IOpenApiAny>();
        foreach (var variant in variants)
        {
            if (variant.Enum.Any())
            {
                foreach (var value in variant.Enum)
                {
                    schema.Enum.Add(value);
                }
            }
            else if (variant.Type == "string")
            {
                // Do not add placeholder or title if not enum
                // Just skip
            }
        }
        schema.Enum = schema.Enum.Distinct().ToList();

        schema.OneOf.Clear();
        schema.AnyOf.Clear();
        schema.AllOf.Clear();
    }


    // Checks if an allOf schema has exactly two items: one is a reference and the other is just nullable:true
    public static bool IsNullableReference(this OpenApiSchema schema)
    {
        if (!schema.IsAllOf() || schema.AllOf.Count != 2)
            return false;

        // Check if one is a reference and the other has nullable:true
        var hasReference = false;
        var hasNullableTrue = false;

        foreach (var item in schema.AllOf)
        {
            if (item.Reference != null)
                hasReference = true;
            else if (item.Reference == null && item.Type == null && item.Nullable == true && !item.Properties.Any() && !item.OneOf.Any() && !item.AnyOf.Any() && !item.AllOf.Any())
                hasNullableTrue = true;
        }

        return hasReference && hasNullableTrue;
    }

    // Converts an allOf with a reference and nullable:true to a single reference with nullable:true
    public static void ConvertNullableReferenceIfApplicable(this OpenApiSchema schema)
    {
        if (!schema.IsNullableReference())
            return;

        // Find the reference schema
        var referenceSchema = schema.AllOf.First(item => item.Reference != null);

        // Copy the reference and make it nullable
        schema.Reference = referenceSchema.Reference;
        schema.Nullable = true;

        // Clear the allOf
        schema.AllOf.Clear();
    }
}