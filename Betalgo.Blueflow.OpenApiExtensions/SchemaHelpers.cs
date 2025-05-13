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
        return schema.AnyOf.Any() && schema.OneOf.SelectMany(r => r.Properties).Any();
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

  public static void SetAsPathParamater(this OpenApiSchema schema)
    {
        schema.SetBlueflowExtension("path-parameter", new OpenApiBoolean(true));
    }

    public static bool IsPathParamater(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiBoolean>("path-parameter")?.Value == true;
    }
    
    public static void SetInheritFromReference(this OpenApiSchema schema, OpenApiReference reference)
    {
        schema.Reference = reference;
        schema.SetBlueflowExtension("is-inheritance", new OpenApiBoolean(true));
    }
    
    public static void SetInheritFrom(this OpenApiSchema schema, string className)
    {
        schema.SetBlueflowExtension("inherit-from", new OpenApiString(className));
    }
    
    public static string? GetInheritFrom(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiString>("inherit-from")?.Value;
    }
    
    public static bool IsInheritance(this OpenApiSchema schema)
    {
        return schema.GetBlueflowExtension<OpenApiBoolean>("is-inheritance")?.Value == true;
    }
    
    public static bool HasInheritance(this OpenApiSchema schema)
    {
        return schema.IsInheritance() || !string.IsNullOrEmpty(schema.GetInheritFrom());
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
    
    /// <summary>
    /// Generates schema models for path parameters in the OpenApiDocument.
    /// Creates a Request schema for each path operation using the operationId and replaces
    /// the original parameters with a reference to the new schema.
    /// </summary>
    /// <param name="document">The OpenAPI document containing paths to process</param>
    public static void GeneratePathParameterSchemas(OpenApiDocument document)
    {
        if (document.Paths == null || !document.Paths.Any())
        {
            return;
        }
        
        InitializeDocumentComponents(document);
        
        // Process each path and its operations
        foreach (var pathItem in document.Paths)
        {
            ProcessPathOperations(document, pathItem.Key, pathItem.Value.Operations);
        }
    }
    
    /// <summary>
    /// Initialize document components if they don't exist
    /// </summary>
    private static void InitializeDocumentComponents(OpenApiDocument document)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();
    }
    
    /// <summary>
    /// Process all operations for a specific path
    /// </summary>
    private static void ProcessPathOperations(OpenApiDocument document, string path, 
        IDictionary<OperationType, OpenApiOperation> operations)
    {
        // Process each operation type
        var operationTypes = new[] 
        { 
            OperationType.Get, 
            OperationType.Post, 
            OperationType.Put, 
            OperationType.Delete, 
            OperationType.Patch 
        };
        
        foreach (var operationType in operationTypes)
        {
            ProcessPathOperation(document, path, operations, operationType);
        }
    }
    
    /// <summary>
    /// Process a specific operation type within a path
    /// </summary>
    private static void ProcessPathOperation(OpenApiDocument document, string path, 
        IDictionary<OperationType, OpenApiOperation> operations, OperationType operationType)
    {
        // Skip if operation doesn't exist or has no operationId
        if (!operations.TryGetValue(operationType, out var operation) || string.IsNullOrEmpty(operation.OperationId))
        {
            return;
        }

        // Convert the operation to a schema and add it to the document
        ConvertPathRequestToSchema(document, operation);
    }
    
    /// <summary>
    /// Converts an operation's path parameters and request body to a schema
    /// and adds it to the document components.
    /// </summary>
    /// <param name="document">The OpenAPI document</param>
    /// <param name="operation">The operation to convert</param>
    public static void ConvertPathRequestToSchema(OpenApiDocument document, OpenApiOperation operation)
    {
        // Handle special case: operation with only a referenced request body
        if (TryProcessReferencedRequestBody(operation))
        {
            return;
        }
        
        // Generate schema for parameters and request body
        var modelName = operation.OperationId;
        var schema = CreateParameterSchema(operation, modelName);
        var hasParameters = ProcessOperationParameters(schema, operation);
        hasParameters |= ProcessRequestBody(schema, operation, false);
        
        // Only create schema if it has parameters
        if (hasParameters)
        {
            AddSchemaToDocument(document, operation, schema, modelName);
        }
    }
    
    /// <summary>
    /// Try to process an operation that only has a referenced request body and no parameters
    /// </summary>
    /// <returns>True if the operation was processed as a referenced request body, false otherwise</returns>
    private static bool TryProcessReferencedRequestBody(OpenApiOperation operation)
    {
        // Check if the operation has no parameters and only a referenced request body
        if ((operation.Parameters == null || !operation.Parameters.Any()) && 
            operation.RequestBody != null && 
            operation.RequestBody.Content.TryGetValue("application/json", out var content) && 
            content.Schema?.Reference != null)
        {
            string referencedSchemaId = content.Schema.Reference.Id;
            
            if (!string.IsNullOrEmpty(referencedSchemaId))
            {
                // Create a reference to the existing schema
                var schemaReference = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = referencedSchemaId
                    }
                };
                
                // Replace original parameters with reference to the existing schema
                ReplaceOperationParametersWithReference(operation, referencedSchemaId, schemaReference);
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Create a new schema for operation parameters
    /// </summary>
    private static OpenApiSchema CreateParameterSchema(OpenApiOperation operation, string modelName)
    {
        return new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>(),
            Required = new HashSet<string>(),
            Description = $"Request parameters for {operation.OperationId} operation"
        };
    }
    
    /// <summary>
    /// Process all operation parameters and add them to the schema
    /// </summary>
    /// <returns>True if any parameters were processed, false otherwise</returns>
    private static bool ProcessOperationParameters(OpenApiSchema schema, OpenApiOperation operation)
    {
        if (operation.Parameters == null || !operation.Parameters.Any())
        {
            return false;
        }
        
        // Process all parameters and mark them as path parameters
        foreach (var parameter in operation.Parameters)
        {
            // Use original schema directly to preserve all properties
            var paramSchema = parameter.Schema;
            
            // Apply parameter description if available
            if (!string.IsNullOrEmpty(parameter.Description))
            {
                paramSchema.Description = parameter.Description;
            }
            
            // Mark as path parameter
            paramSchema.SetAsPathParamater();
            
            // Copy any custom extensions
            CopyExtensions(parameter.Extensions, paramSchema);
            
            // Add to schema properties
            schema.Properties[parameter.Name] = paramSchema;
            if (parameter.Required)
            {
                schema.Required.Add(parameter.Name);
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Copy extensions from source to target schema
    /// </summary>
    private static void CopyExtensions(IDictionary<string, IOpenApiExtension> sourceExtensions, OpenApiSchema targetSchema)
    {
        if (sourceExtensions == null || !sourceExtensions.Any())
        {
            return;
        }
        
        targetSchema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        foreach (var ext in sourceExtensions)
        {
            targetSchema.Extensions[ext.Key] = ext.Value;
        }
    }
    
    /// <summary>
    /// Process the request body and add its properties to the schema
    /// </summary>
    /// <param name="schema">The schema to add properties to</param>
    /// <param name="operation">The operation containing the request body</param>
    /// <param name="isReferencedBody">Whether to skip processing if the body is a reference</param>
    /// <returns>True if any request body properties were processed, false otherwise</returns>
    private static bool ProcessRequestBody(OpenApiSchema schema, OpenApiOperation operation, bool isReferencedBody)
    {
        if (operation.RequestBody == null || isReferencedBody || 
            !operation.RequestBody.Content.TryGetValue("application/json", out var mediaType) || 
            mediaType.Schema == null)
        {
            return false;
        }
        
        // Handle reference schemas (inheritance)
        if (mediaType.Schema.Reference != null)
        {
            // Store the full reference for inheritance instead of just the class name
            schema.SetInheritFromReference(mediaType.Schema.Reference);
            return true;
        }
        
        // Handle schemas with properties
        if (mediaType.Schema.Properties != null && mediaType.Schema.Properties.Any())
        {
            foreach (var property in mediaType.Schema.Properties)
            {
                // Use original property schema directly to preserve all data
                schema.Properties[property.Key] = property.Value;
                if (mediaType.Schema.Required != null && mediaType.Schema.Required.Contains(property.Key))
                {
                    schema.Required.Add(property.Key);
                }
            }
            return true;
        }
        
        // Handle primitive types or arrays
        schema.Properties["Body"] = mediaType.Schema; // Use original schema directly
        if (operation.RequestBody.Required)
        {
            schema.Required.Add("Body");
        }
        
        return true;
    }
    
    /// <summary>
    /// Add the schema to the document components and update the operation
    /// </summary>
    private static void AddSchemaToDocument(OpenApiDocument document, OpenApiOperation operation, 
        OpenApiSchema schema, string modelName)
    {
        // Set Blueflow metadata
        schema.SetBlueflowId();
        schema.SetAsMainComponent();
        schema.SetBlueflowName(modelName);
        
        // Add the schema to the components
        document.Components.Schemas[modelName] = schema;
        
        // Create a reference to the schema
        var schemaReference = new OpenApiSchema
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = modelName
            }
        };
        
        // Replace original parameters with reference
        ReplaceOperationParametersWithReference(operation, modelName, schemaReference);
    }
    
    /// <summary>
    /// Replaces all operation parameters with a single parameter referencing a schema
    /// </summary>
    /// <param name="operation">The operation to modify</param>
    /// <param name="modelName">The name of the schema model</param>
    /// <param name="schemaReference">The schema reference to use</param>
    private static void ReplaceOperationParametersWithReference(OpenApiOperation operation, string modelName, OpenApiSchema schemaReference)
    {
        // Initialize Parameters collection if null
        operation.Parameters ??= new List<OpenApiParameter>();
        
        // Save original parameters' required status before clearing
        bool hasRequiredParameters = operation.Parameters.Any(p => p.Required);
        
        // Clear original parameters
        operation.Parameters.Clear();
        
        // Add a new parameter that references our schema
        operation.Parameters.Add(CreateSchemaReferenceParameter(operation, modelName, schemaReference, hasRequiredParameters));
        
        // Remove request body as it's now part of the schema
        operation.RequestBody = null;
    }
    
    /// <summary>
    /// Creates a parameter that references a schema
    /// </summary>
    /// <returns>A new OpenApiParameter instance</returns>
    private static OpenApiParameter CreateSchemaReferenceParameter(OpenApiOperation operation, string modelName, 
        OpenApiSchema schemaReference, bool isRequired)
    {
        return new OpenApiParameter
        {
            Name = modelName,
            In = ParameterLocation.Query, // Location doesn't matter as it's just a reference
            Required = isRequired,
            Schema = schemaReference,
            Description = $"Parameters for {operation.OperationId} operation"
        };
    }
    
    /// <summary>
    /// Creates a deep copy of an OpenApiSchema, including all nested properties and collections.
    /// </summary>
    /// <param name="source">The source schema to clone</param>
    /// <returns>A new OpenApiSchema instance with all properties copied from the source</returns>
    private static OpenApiSchema CloneSchema(OpenApiSchema source)
    {
        if (source == null) return null;
        
        // Create new schema with basic properties
        var clone = new OpenApiSchema
        {
            Title = source.Title,
            Type = source.Type,
            Format = source.Format,
            Description = source.Description,
            Nullable = source.Nullable,
            Default = source.Default,
            Example = source.Example,
            ExternalDocs = source.ExternalDocs,
            Deprecated = source.Deprecated,
            
            // Validation properties
            MinLength = source.MinLength,
            MaxLength = source.MaxLength,
            Pattern = source.Pattern,
            MinItems = source.MinItems,
            MaxItems = source.MaxItems,
            UniqueItems = source.UniqueItems,
            ExclusiveMinimum = source.ExclusiveMinimum,
            ExclusiveMaximum = source.ExclusiveMaximum,
            Minimum = source.Minimum,
            Maximum = source.Maximum,
            MultipleOf = source.MultipleOf,
            
            // Access properties
            ReadOnly = source.ReadOnly,
            WriteOnly = source.WriteOnly,
            
            // Additional metadata
            Discriminator = source.Discriminator,
            Xml = source.Xml
        };
        
        // Copy Reference if exists
        if (source.Reference != null)
        {
            clone.Reference = new OpenApiReference
            {
                Id = source.Reference.Id,
                Type = source.Reference.Type,
                ExternalResource = source.Reference.ExternalResource,
            };
        }
        
        // Copy collections with null checks
        CloneEnumValues(source, clone);
        CloneExtensions(source, clone);
        CloneItemsSchema(source, clone);
        CloneProperties(source, clone);
        CloneRequiredProperties(source, clone);
        CloneComplexTypes(source, clone);
        
        return clone;
    }
    
    /// <summary>
    /// Clones enum values from source to target schema
    /// </summary>
    private static void CloneEnumValues(OpenApiSchema source, OpenApiSchema target)
    {
        if (source.Enum != null && source.Enum.Any())
        {
            target.Enum = new List<IOpenApiAny>(source.Enum);
        }
    }
    
    /// <summary>
    /// Clones extensions from source to target schema
    /// </summary>
    private static void CloneExtensions(OpenApiSchema source, OpenApiSchema target)
    {
        if (source.Extensions != null && source.Extensions.Any())
        {
            target.Extensions = new Dictionary<string, IOpenApiExtension>();
            foreach (var ext in source.Extensions)
            {
                target.Extensions[ext.Key] = ext.Value;
            }
        }
    }
    
    /// <summary>
    /// Clones the Items schema for array types
    /// </summary>
    private static void CloneItemsSchema(OpenApiSchema source, OpenApiSchema target)
    {
        if (source.Items != null)
        {
            target.Items = CloneSchema(source.Items);
        }
    }
    
    /// <summary>
    /// Clones all properties from source to target schema
    /// </summary>
    private static void CloneProperties(OpenApiSchema source, OpenApiSchema target)
    {
        if (source.Properties != null && source.Properties.Any())
        {
            target.Properties = new Dictionary<string, OpenApiSchema>();
            foreach (var prop in source.Properties)
            {
                target.Properties[prop.Key] = CloneSchema(prop.Value);
            }
        }
    }
    
    /// <summary>
    /// Clones required property names from source to target schema
    /// </summary>
    private static void CloneRequiredProperties(OpenApiSchema source, OpenApiSchema target)
    {
        if (source.Required != null && source.Required.Any())
        {
            target.Required = new HashSet<string>(source.Required);
        }
    }
    
    /// <summary>
    /// Clones complex type collections (OneOf, AnyOf, AllOf) from source to target schema
    /// </summary>
    private static void CloneComplexTypes(OpenApiSchema source, OpenApiSchema target)
    {
        // Handle OneOf collection
        if (source.OneOf != null && source.OneOf.Any())
        {
            target.OneOf = new List<OpenApiSchema>();
            foreach (var schema in source.OneOf)
            {
                target.OneOf.Add(CloneSchema(schema));
            }
        }
        
        // Handle AnyOf collection
        if (source.AnyOf != null && source.AnyOf.Any())
        {
            target.AnyOf = new List<OpenApiSchema>();
            foreach (var schema in source.AnyOf)
            {
                target.AnyOf.Add(CloneSchema(schema));
            }
        }
        
        // Handle AllOf collection
        if (source.AllOf != null && source.AllOf.Any())
        {
            target.AllOf = new List<OpenApiSchema>();
            foreach (var schema in source.AllOf)
            {
                target.AllOf.Add(CloneSchema(schema));
            }
        }
    }
}