using Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp.Services;
using Betalgo.Blueflow.OpenAPIToCode.Generators.Models;
using Betalgo.Blueflow.OpenAPIToCode.Utils;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scriban;

namespace Betalgo.Blueflow.OpenAPIToCode.Generators.CSharp;

/// <summary>
///     Represents information about a OneOf variant in an OpenAPI schema.
/// </summary>
public class PolyVariantInfo
{
    /// <summary>
    ///     Gets or sets the type for the variant.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the property name for the variant.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether to use write string.
    /// </summary>
    public bool UseWriteString { get; set; }

    /// <summary>
    ///     Gets or sets the JSON token type.
    /// </summary>
    public string JsonTokenType { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the array element token type if this is an array.
    /// </summary>
    public string? ArrayElementTokenType { get; set; }
}

/// <summary>
///     Implementation of ICodeGenerator for C# code generation.
/// </summary>
public class CSharpCodeGenerator : ICodeGenerator
{
    public CSharpCodeGenerator(ICodeGeneratorConfiguration? configuration = null, INamingService? namingService = null, ITypeMappingService? typeMappingService = null, IDocumentationNormalizerService? documentationNormalizerService = null,
        ITemplateProviderService? templateProviderService = null)
    {
        Configuration = configuration ?? new CSharpCodeGeneratorConfiguration();
        NamingService = namingService ?? new CSharpNamingService();
        TypeMappingService = typeMappingService ?? new CSharpTypeMappingService();
        DocumentationNormalizerService = documentationNormalizerService ?? new CSharpDocumentationNormalizerService();
        TemplateProviderService = templateProviderService ?? new CSharpTemplateProviderService();
        DocumentationNormalizerService.SetBaseDomain(configuration?.DocumentationBaseDomain);
    }

    public ITemplateProviderService TemplateProviderService { get; }

    public IDocumentationNormalizerService DocumentationNormalizerService { get; }

    public ICodeGeneratorConfiguration Configuration { get; }

    public INamingService NamingService { get; }

    public ITypeMappingService TypeMappingService { get; }


    public string RenderFile(FileDefinition fileDefinition)
    {
        var templateText = TemplateProviderService.GetFileTemplate();
        return RenderFile(fileDefinition, templateText);
    }


    public string RenderProperty(OpenApiSchema schema, string? templateText = null)
    {
        templateText ??= TemplateProviderService.GetPropertyTemplate();

        if (schema.IsPolyTypeProperty())
        {
            schema.Nullable = true;
        }

        var typeString = TypeMappingService.MapType(schema);
        var normalizedSummary = DocumentationNormalizerService.Normalize(schema.Description);
        var template = Template.Parse(templateText);
        string? name, jsonName = null;
        if (schema.IsPolyTypeProperty())
        {
            name = NamingService.Convert(typeString, NamingPurpose.AsProperty);
        }
        else
        {
            name = NamingService.Convert(schema.GetSelfKey(), NamingPurpose.Property);
            jsonName = schema.GetSelfKey();
        }

        var result = template.Render(new
        {
            property = new
            {
                name = name,
                type_string = typeString,
                summary = normalizedSummary,
                json_name = jsonName
            }
        });

        return result.Trim();
    }

    public string RenderClass(OpenApiSchema schema, string? templateText = null)
    {
        if (templateText == null)
        {
            if (schema.IsPoly())
            {
                return RenderPolyClass(schema);
            }
            //else if (schema.IsAnyOf())
            //{
            //    return RenderAnyOfClass(schema);
            //}
            //else if (schema.IsAllOf())
            //{
            //    return RenderOneOfClass(schema);

            //}
            else
            {
                templateText = TemplateProviderService.GetClassTemplate();
            }
        }

        var modifiers = "public";
        var properties = schema.Properties.Select(r => RenderProperty(r.Value, null)).ToList();
        var normalizedSummary = DocumentationNormalizerService.Normalize(schema.Description);

        var nestedClasses = schema.GetNestedObjects().Select(Render);
        var constructor = RenderConstructor(schema);
        var template = Template.Parse(templateText);
        var result = template.Render(new
        {
            name = schema.GetBlueflowName(),
            summary = normalizedSummary,
            modifiers,
            //base_types = baseTypesString,
            properties,
            constructor,
            nested_classes = nestedClasses,
            //parents = classDef.ParentIds,
            ido = schema.GetBlueFlowId()
        });
        return result.Trim();
    }

    public string Render(OpenApiSchema schema)
    {
        if (schema.IsObject())
        {
            return RenderClass(schema);
        }

        if (schema.IsEnum())
        {
            return RenderStringEnum(schema);
        }

        if (schema.IsArray())
        {
            if (schema.IsMainComponent() && schema.Items.Reference != null)
            {
                return string.Empty;
            }
            else
            {
                return Render(schema.Items);
            }
        }

        if (schema.IsPolyTypeProperty())
        {
            return RenderProperty(schema);
        }

        if (schema.Type == "boolean")
        {
            return string.Empty;
        }

        throw new NotImplementedException();
    }

    public string RenderStringEnum(OpenApiSchema enumDefinition, string? templateText = null)
    {
        if (enumDefinition == null)
            throw new ArgumentNullException(nameof(enumDefinition));

        // load template
        if (templateText == null)
        {
            templateText = TemplateProviderService.GetStringEnumTemplate();
        }

        var enumName = enumDefinition.GetBlueflowName();
        var summary = DocumentationNormalizerService.Normalize(enumDefinition.Description);
        var modifiers = "public";

        var members = enumDefinition.Enum
            .OfType<OpenApiString>()
            .Select(v => new
            {
                name = NamingService.Convert(v.Value, NamingPurpose.EnumMember),
                value = v.Value
            })
            .ToList();

        var template = Template.Parse(templateText);
        var result = template.Render(new
        {
            summary,
            modifiers,
            name = enumName,
            members
        });

        return result.Trim();
    }


    public List<string> RenderBase()
    {
        var baseStrings = new List<string>();

        // Render Solution template if exists
        if (TemplateProviderService.TemplateExists("Solution"))
        {
            var solutionTemplate = TemplateProviderService.GetSolutionTemplate();
            baseStrings.Add(solutionTemplate);
        }

        // Render Project template if exists
        if (TemplateProviderService.TemplateExists("Project"))
        {
            var projectTemplate = TemplateProviderService.GetProjectTemplate();
            baseStrings.Add(projectTemplate);
        }

        return baseStrings;
    }

    public string RenderPolyClass(OpenApiSchema schema, string? templateText = null)
    {
        var polyType = PolyType.None;
        if (schema.IsOneOf())
        {
            polyType = PolyType.OneOf;
        }
        else if (schema.IsAllOf())
        {
            polyType = PolyType.AllOf;
        }
        else if (schema.IsAnyOf())
        {
            polyType = PolyType.AnyOf;
        }

        templateText ??= polyType switch
        {
            PolyType.OneOf => TemplateProviderService.GetOneOfClassTemplate(),
            PolyType.AllOf => TemplateProviderService.GetOneOfClassTemplate(),
            PolyType.AnyOf => TemplateProviderService.GetOneOfClassTemplate(),
            PolyType.None => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null),
            _ => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null)
        };

        var converterTemplateText = polyType switch
        {
            PolyType.OneOf => TemplateProviderService.GetOneOfConverterTemplate(),
            PolyType.AllOf => TemplateProviderService.GetOneOfConverterTemplate(),
            PolyType.AnyOf => TemplateProviderService.GetOneOfConverterTemplate(),
            PolyType.None => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null),
            _ => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null)
        };
        var properties = polyType switch
        {
            PolyType.OneOf => schema.OneOf.Select(r => RenderProperty(r)).ToList(),
            PolyType.AllOf => schema.AllOf.Select(r => RenderProperty(r)).ToList(),
            PolyType.AnyOf => schema.AnyOf.Select(r => RenderProperty(r)).ToList(),
            PolyType.None => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null),
            _ => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null)
        };
        var modifiers = "public";
        //var properties = schema.OneOf.Select(r => RenderProperty(r)).ToList();
        var normalizedSummary = DocumentationNormalizerService.Normalize(schema.Description);
        var nestedClasses = schema.GetNestedObjects().Select(Render).ToList();
        // Process the converter template
        var className = schema.GetBlueflowName();
        var oneOfVariants = polyType switch
        {
            PolyType.OneOf => schema.OneOf.Select(GetOneOfVariantInfo).ToList(),
            PolyType.AllOf => schema.AllOf.Select(GetOneOfVariantInfo).ToList(),
            PolyType.AnyOf => schema.AnyOf.Select(GetOneOfVariantInfo).ToList(),
            PolyType.None => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null),
            _ => throw new ArgumentOutOfRangeException(nameof(polyType), polyType, null)
        };
        var converterTemplate = Template.Parse(converterTemplateText);
        var converterResult = converterTemplate.Render(new
        {
            name = className,
            discriminator = "type",
            one_of_variants = oneOfVariants.Select(v => new
                {
                    cs_type = v.Type,
                    property_name = v.PropertyName,
                    use_write_string = v.UseWriteString,
                    json_token_type = v.JsonTokenType,
                    array_element_token_type = v.ArrayElementTokenType
                })
                .ToList()
        });

        var template = Template.Parse(templateText);
        var result = template.Render(new
        {
            name = className,
            summary = normalizedSummary,
            modifiers,
            //base_types = baseTypesString,
            properties,
            // constructor,
            nested_classes = nestedClasses,
            //parents = classDef.ParentIds,
            ido = schema.GetBlueFlowId(),
            converter = converterResult,
            one_of_variants = oneOfVariants.Select(v => new
                {
                    cs_type = v.Type,
                    property_name = v.PropertyName,
                    use_write_string = v.UseWriteString,
                    json_token_type = v.JsonTokenType,
                    array_element_token_type = v.ArrayElementTokenType
                })
                .ToList()
        });
        return result.Trim();
    }

    private PolyVariantInfo GetOneOfVariantInfo(OpenApiSchema schema)
    {
        var csType = TypeMappingService.MapType(schema);
        var propertyName = NamingService.Convert(NamingService.ToPascalCase(csType), NamingPurpose.AsProperty);
        var useWriteString = csType == "string";
        var jsonTokenType = GetJsonTokenType(schema);

        // Determine array element token type if this is an array
        string? arrayElementTokenType = null;
        if (schema.IsArray())
        {
            arrayElementTokenType = GetJsonTokenType(schema.Items);
        }

        return new()
        {
            Type = csType,
            PropertyName = propertyName,
            UseWriteString = useWriteString,
            JsonTokenType = jsonTokenType,
            ArrayElementTokenType = arrayElementTokenType
        };
    }

    private string GetJsonTokenType(OpenApiSchema schema)
    {
        return schema.Type switch
        {
            "boolean" => "True",
            "integer" => "Number",
            "number" => "Number",
            "string" => "String",
            "array" => "StartArray",
            "object" => "StartObject",
            _ => "String" // Default case
        };
    }

    public string RenderAnyOfClass(OpenApiSchema schema, string? templateText = null)
    {
        if (templateText == null)
        {
            templateText = TemplateProviderService.GetOneOfClassTemplate();
        }

        // Load the JsonConverter template
        var converterTemplateText = TemplateProviderService.GetOneOfConverterTemplate();

        var modifiers = "public";
        var properties = schema.AnyOf.Select(r => RenderProperty(r, null)).ToList();
        var normalizedSummary = DocumentationNormalizerService.Normalize(schema.Description);
        var nestedClasses = schema.GetNestedObjects().Select(Render).ToList();

        // Process the converter template
        var className = NamingService.Convert(schema.GetSelfKey(), NamingPurpose.AnyOfClass);
        var anyOfVariants = schema.AnyOf.Select(variant => GetAnyOfVariantInfo(variant)).ToList();
        var converterTemplate = Template.Parse(converterTemplateText);
        var converterResult = converterTemplate.Render(new
        {
            name = className,
            discriminator = "type",
            any_of_variants = anyOfVariants.Select(v => new
                {
                    cs_type = v.Type,
                    property_name = v.PropertyName,
                    use_write_string = v.UseWriteString,
                    json_token_type = v.JsonTokenType,
                    array_element_token_type = v.ArrayElementTokenType
                })
                .ToList()
        });

        var template = Template.Parse(templateText);
        var result = template.Render(new
        {
            name = className,
            summary = normalizedSummary,
            modifiers,
            //base_types = baseTypesString,
            properties,
            // constructor,
            nested_classes = nestedClasses,
            //parents = classDef.ParentIds,
            ido = schema.GetBlueFlowId(),
            converter = converterResult,
            any_of_variants = anyOfVariants.Select(v => new
                {
                    cs_type = v.Type,
                    property_name = v.PropertyName,
                    use_write_string = v.UseWriteString,
                    json_token_type = v.JsonTokenType,
                    array_element_token_type = v.ArrayElementTokenType
                })
                .ToList()
        });
        return result.Trim();
    }

    private PolyVariantInfo GetAnyOfVariantInfo(OpenApiSchema variant)
    {
        var csType = TypeMappingService.MapType(variant);
        var propertyName = NamingService.Convert(NamingService.ToPascalCase(csType), NamingPurpose.AsProperty);
        var useWriteString = csType == "string";
        var jsonTokenType = GetJsonTokenType(variant);

        // Determine array element token type if this is an array
        string? arrayElementTokenType = null;
        if (variant.Type == "array" && variant.Items != null)
        {
            arrayElementTokenType = GetJsonTokenType(variant.Items);
        }

        return new()
        {
            Type = csType,
            PropertyName = propertyName,
            UseWriteString = useWriteString,
            JsonTokenType = jsonTokenType,
            ArrayElementTokenType = arrayElementTokenType
        };
    }

    public string RenderFile(FileDefinition fileDefinition, string templateText)
    {
        var usings = fileDefinition.Usings ?? [];
        var ns = fileDefinition.Namespace;
        var template = Template.Parse(templateText);
        var result = template.Render(new
        {
            usings,
            ns,
            items = fileDefinition.Content
        });
        return result.Trim();
    }

    public string? RenderConstructor(OpenApiSchema schema)
    {
        var nonNullableProps = schema.Properties.Where(p => !p.Value.Nullable).ToList();
        if (!nonNullableProps.Any())
        {
            return null;
        }

        if (schema.IsPoly())
        {
            return null;
        }

        var templateText = TemplateProviderService.GetConstructorTemplate();

        var constructorParameters = nonNullableProps.Select(p => new
            {
                type = TypeMappingService.MapType(p.Value),
                name = NamingService.Convert(p.Value.GetSelfKey(), NamingPurpose.Parameter),
                summary = DocumentationNormalizerService.Normalize(p.Value.Description)
            })
            .ToList();
        var assignments = nonNullableProps.Select(p => $"{NamingService.Convert(p.Value.GetSelfKey(), NamingPurpose.Property)} = {NamingService.Convert(p.Value.GetSelfKey(), NamingPurpose.Parameter)};").ToList();
        var template = Template.Parse(templateText);
        var result = template.Render(new
        {
            name = schema.GetBlueflowName(),
            NamingPurpose.Class,
            constructor_parameters = constructorParameters,
            assignments
        });
        return result.Trim();
    }
}

public enum PolyType
{
    None,
    OneOf,
    AllOf,
    AnyOf
}

public interface ITemplate
{
}