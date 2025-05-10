using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Betalgo.Blueflow.OpenAPIToCode;

public static class SchemaHelpers
{
    public static bool IsEnum(this OpenApiSchema schema)
    {
        return schema is { Enum: { Count: > 0 }, Type: "string" or null };
    }
 

    public static bool IsOneOf(this OpenApiSchema schema)
    {
        return schema.OneOf.Any();
    }

    public static bool IsAllOf(this OpenApiSchema schema)
    {
        return schema.AllOf.Any();
    }

    public static bool IsAnyOf(this OpenApiSchema schema)
    {
        return schema.AnyOf.Any();
    }
    public static bool IsReference(this OpenApiSchema schema)
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
    public static void SetBlueflowId(this OpenApiSchema schema, string? _)
    {
        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-blueflow-id"] = new OpenApiString(Guid.NewGuid().ToString());
    }
    
    public static void SetAsMainComponent(this OpenApiSchema schema)
    {
        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-blueflow-main"] = new OpenApiBoolean(true);
    }
    public static bool IsMainComponent(this OpenApiSchema schema)
    {
        return schema.Extensions != null && schema.Extensions.TryGetValue("x-blueflow-main", out var ext) && ext is OpenApiBoolean;
    }
    public static void SetBlueflowName(this OpenApiSchema schema, string? name)
    {
        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-blueflow-name"] = new OpenApiString(name);
    }
    public static void SetItemOfArray(this OpenApiSchema schema)
    {
        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-blueflow-is-item-of-array"] = new OpenApiBoolean(true);
    }
    
    public static void SetBlueflowPropertyType(this OpenApiSchema schema, string? name)
    {
        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-blueflow-poly-type"] = new OpenApiString(name);
    }

    public static bool IsItemOfArray(this OpenApiSchema schema)
    {
        return schema.Extensions != null && schema.Extensions.TryGetValue("x-blueflow-is-item-of-array", out var ext) && ext is OpenApiBoolean;
    }
    public static string? GetBlueflowName(this OpenApiSchema schema)
    {
   
        if (schema.Extensions != null && schema.Extensions.TryGetValue("x-blueflow-name", out var ext) && ext is OpenApiString name)
        {
            return name.Value;
        }

        return null;
    }
    public static string? GetBlueflowPropertyType(this OpenApiSchema schema)
    {
   
        if (schema.Extensions != null && schema.Extensions.TryGetValue("x-blueflow-poly-type", out var ext) && ext is OpenApiString value)
        {
            return value.Value;
        }

        return null;
    }

    public static void SetBlueflowSelfKey(this OpenApiSchema schema, string? key)
    {
        if (string.IsNullOrEmpty(key)) return;
        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-blueflow-self"] = new OpenApiString(key);
    }
    public static string? GetSelfKey(this OpenApiSchema schema)
    {
        if (schema.Extensions != null && schema.Extensions.TryGetValue("x-blueflow-self", out var ext) && ext is OpenApiString self)
        {
            return self.Value;
        }

        return null;
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

    public static Guid GetBlueFlowId(this OpenApiSchema schema)
    {
        if (schema.Extensions != null && schema.Extensions.TryGetValue("x-blueflow-id", out var ext) && ext is OpenApiString idStr && Guid.TryParse(idStr.Value, out var parsedId))
        {
            return parsedId;
        }
        return Guid.NewGuid();
    //    throw new InvalidOperationException("The schema does not contain a valid 'x-blueflow-id' extension.");
    }
    public static List<EnumValueDefinition> GetEnumDefinitions(this IList<IOpenApiAny> schema)
    {
        return schema.OfType<OpenApiString>()
            .Select(e => new EnumValueDefinition
            {
                Name = e.Value ?? string.Empty,
                JsonName = e.Value ?? string.Empty
            })
            .ToList();

    }
    public static IEnumerable<OpenApiSchema> FilterByObjectsWithoutReference(this IEnumerable<OpenApiSchema> schema)
    {
        return schema.Where(r => r.IsObjectWithoutReference() || r.IsEnum());
    }
    public static IEnumerable<OpenApiSchema> ExtractArrayObjectsWithoutReference(this IEnumerable<OpenApiSchema> schema)
    {
        return schema.Where(r => r.Type == "array" && r.Items.IsObjectWithoutReference()).Select(r=>r.Items);
    }   
    public static IEnumerable<OpenApiSchema> ExtractArrayObjectsWithoutReference(this IDictionary<string, OpenApiSchema> schema)
    {
        return schema.Where(r => r.Value.Type == "array" && r.Value.Items.IsObjectWithoutReference()).Select(r=>r.Value.Items);
    }
    public static IEnumerable<KeyValuePair<string, OpenApiSchema>> FilterByObjectsWithoutReference(this IDictionary<string,OpenApiSchema> schema)
    {
        return schema.Where(r => r.Value.IsObjectWithoutReference() || (r.Value.Type == "array" && r.Value.Items.IsObjectWithoutReference()));
    }

    public static IEnumerable<OpenApiSchema> GetNestedObjects(this OpenApiSchema schema)
    {
        var list = new List<OpenApiSchema>();
        list.AddRange(schema.Properties.FilterByObjectsWithoutReference().Select(r=>r.Value));
        list.AddRange(schema.Properties.Where(r=>r.Value.IsEnum()).Select(r=>r.Value));
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
    public static EnumDefinition? ExtractEnum(this OpenApiSchema schema, string key, string @namespace)
    {
        if (!schema.IsEnum())
        {
            return null;
        }

        var blueflowId = schema.GetBlueFlowId();
        var values = schema.Enum.GetEnumDefinitions();
        var enumDef = new EnumDefinition
        {
            Id = blueflowId,
            Namespace = @namespace,
            Values = values,
            Summary = schema.Description,
            Name = key,
        };
        return enumDef;
    }

    public static ClassDefinition? ExtractClass(this OpenApiSchema schema, string key, string @namespace, List<Guid> parents)
    {

        if (!schema.IsObject())
        {
            return null;
        }

        var parentsToSent = parents.Select(r => r).ToList();
        parentsToSent.Add(schema.GetBlueFlowId());
        var blueflowId = schema.GetBlueFlowId();
        var classDef = new ClassDefinition
        {
            Id = blueflowId,
            Namespace = @namespace,
            Summary = schema.Description,
            Properties = [],
            Modifiers = ClassModifiers.Public,
            NestedClasses = schema.Properties
                .Where(p => p.Value.IsObject())
                .Select(p => p.Value.Reference == null ? p.Value.ExtractClass(p.Key, @namespace, parentsToSent) : null)
                .Where(c => c != null)
                .ToList(),
            PolyType = PolyType.None,
            Name = key,
            ParentIds = parents
        };
        if (schema.IsPoly())
        {
            if (schema.IsOneOf())
            {
                classDef.PolyType = PolyType.OneOf;
            }
            else if (schema.IsAllOf())
            {
                classDef.PolyType = PolyType.AllOf;
            }
            else if (schema.IsAnyOf())
            {
                classDef.PolyType = PolyType.AnyOf;
            }
            else
            {
                throw new InvalidEnumArgumentException("The schema is not a valid poly type.");
            }
        }
        
        var nestedClassesInArray = schema.Properties.Where(r => r.Value.Type == "array" && r.Value.Items.IsObjectWithoutReference())
            .Select(r => r.Value.Items.Reference == null ? r.Value.Items.ExtractClass(r.Key, @namespace, parentsToSent) : null)
            .Where(c => c != null)
            .ToList();
        var nestedClassesInPropertyAsOneOf = schema.Properties.Where(r => r.Value.IsObjectWithoutReference() && r.Value.OneOf.Any())
            .SelectMany(r => r.Value.OneOf.Where(a => a.Reference == null)
                .Select(o => o.ExtractClass(r.Key + o.Title, @namespace, parentsToSent)))
            .Where(c => c != null)
            .ToList();

        var ones =schema.OneOf.Where(r => r.IsObject() && r.Reference == null).Select(r => r.ExtractClass(key, @namespace, parents)).ToList();
        var alls =schema.AllOf.Where(r => r.IsObject() && r.Reference == null).Select(r => r.ExtractClass(key, @namespace, parents)).ToList();

        var nestedClassesInPropertyAsAllOf = schema.Properties.Where(r => r.Value.IsObjectWithoutReference()&& r.Value.AllOf.Any())
            .SelectMany(r => r.Value.AllOf.Where(a => a.Reference == null)
                .Select(o => o.ExtractClass(r.Key + o.Title, @namespace, parentsToSent)))
            .Where(c => c != null)
            .ToList();

     
        classDef.NestedClasses.AddRange(nestedClassesInArray);
        classDef.NestedClasses.AddRange(ones);
        classDef.NestedClasses.AddRange(alls);
        //classDef.NestedClasses.AddRange(nestedClassesInPropertyAsOneOf);
        //classDef.NestedClasses.AddRange(nestedClassesInPropertyAsAllOf);

        if (schema.IsOneOf())
        {
            foreach (var openApiSchema in schema.OneOf)
            {
                var propDef = openApiSchema.ExtractProperty(null);
                if (propDef == null)
                {
                    continue;
                }
                classDef.Properties.Add(propDef);
            }
        }

        if (schema.IsAllOf())
        {
            foreach (var openApiSchema in schema.AllOf)
            {
                var propDef = openApiSchema.ExtractProperty(null);
                if (propDef == null)
                {
                    continue;
                }
                classDef.Properties.Add(propDef);
            }
        }
        else
        {
            foreach (var property in schema.Properties)
            {
                var propDef = property.Value.ExtractProperty(property.Key);
                if (propDef == null)
                {
                    continue;
                }
                classDef.Properties.Add(propDef);
            }
        }

        return classDef;
    }
    public static PropertyDefinition? ExtractProperty(this OpenApiSchema schema, string? key) 
    {
        var blueflowId = schema.GetBlueFlowId();
        var propDef = new PropertyDefinition
        {
            Type = schema.Type,
            TypeId = schema.Type == "object" || schema.Type == null ? schema.GetBlueFlowId() : null,
            Summary = schema.Description,
            JsonName = key,
            Id = blueflowId,
            IsCollection = schema.Type == "array",
            IsNullable = schema.Nullable,
            Name = key,
        };

        //if (schema.OneOf.Any())
        //{
        //    propDef.Type = "object";
        //    propDef.TypeId = schema.GetBlueFlowId();
        //    propDef.OneOf = schema.OneOf
        //        .Select(o => o.ExtractProperty(key))
        //        .ToList();
        //}
        //if (schema.AllOf.Any())
        //{
        //    propDef.Type = "object";
        //    propDef.TypeId = schema.GetBlueFlowId();
        //    propDef.AllOf = schema.AllOf
        //        .Select(o => o.ExtractProperty(key))
        //        .ToList();
        //}

        if (schema.Items != null && propDef.IsCollection)
        {
            if (schema.Items.Type == "object" || schema.Items.Type == null)
            {
                var a = ExtractProperty(schema.Items, key);
                if (a != null)
                {
                    propDef.SubTypes.Add(a);
                }
            }
        }

        return propDef;
    }

    // Checks if a schema (oneOf/anyOf) is only string or string-enum, and can be treated as enum
    public static bool IsPolyStringEnum(this OpenApiSchema schema)
    {
        // Only applies to oneOf/anyOf
        var variants = schema.OneOf.Any() ? schema.OneOf : schema.AnyOf;
        if (!variants.Any()) return false;
        // All must be string or string-enum
        foreach (var variant in variants)
        {
            if (variant.Type != "string")
                return false;
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
        if (!schema.IsOneOf() && !schema.IsAnyOf()) return;
        if (!schema.IsPolyStringEnum()) return;
        var variants = schema.OneOf.Any() ? schema.OneOf : schema.AnyOf;
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
        schema.OneOf.Clear();
        schema.AnyOf.Clear();
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
            else if (item.Reference == null && item.Type == null && 
                     item.Nullable == true && 
                     !item.Properties.Any() && 
                     !item.OneOf.Any() && 
                     !item.AnyOf.Any() && 
                     !item.AllOf.Any())
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
