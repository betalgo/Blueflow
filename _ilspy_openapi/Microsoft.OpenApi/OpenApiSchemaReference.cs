using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Schema reference object
/// </summary>
public class OpenApiSchemaReference : BaseOpenApiReferenceHolder<OpenApiSchema, IOpenApiSchema, JsonSchemaReference>, IOpenApiSchema, IOpenApiDescribedElement, IOpenApiElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiSchema>, IOpenApiReferenceable, IOpenApiSerializable
{
	/// <inheritdoc />
	public string? Description
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Reference.Description))
			{
				return base.Reference.Description;
			}
			return Target?.Description;
		}
		set
		{
			base.Reference.Description = value;
		}
	}

	/// <inheritdoc />
	public string? Title
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Reference.Title))
			{
				return base.Reference.Title;
			}
			return Target?.Title;
		}
		set
		{
			base.Reference.Title = value;
		}
	}

	/// <inheritdoc />
	public Uri? Schema => Target?.Schema;

	/// <inheritdoc />
	public string? Id => Target?.Id;

	/// <inheritdoc />
	public string? Comment => Target?.Comment;

	/// <inheritdoc />
	public IDictionary<string, bool>? Vocabulary => Target?.Vocabulary;

	/// <inheritdoc />
	public string? DynamicRef => Target?.DynamicRef;

	/// <inheritdoc />
	public string? DynamicAnchor => Target?.DynamicAnchor;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiSchema>? Definitions => Target?.Definitions;

	/// <inheritdoc />
	public string? ExclusiveMaximum => Target?.ExclusiveMaximum;

	/// <inheritdoc />
	public string? ExclusiveMinimum => Target?.ExclusiveMinimum;

	/// <inheritdoc />
	public JsonSchemaType? Type => Target?.Type;

	/// <inheritdoc />
	public string? Const => Target?.Const;

	/// <inheritdoc />
	public string? Format => Target?.Format;

	/// <inheritdoc />
	public string? Maximum => Target?.Maximum;

	/// <inheritdoc />
	public string? Minimum => Target?.Minimum;

	/// <inheritdoc />
	public int? MaxLength => Target?.MaxLength;

	/// <inheritdoc />
	public int? MinLength => Target?.MinLength;

	/// <inheritdoc />
	public string? Pattern => Target?.Pattern;

	/// <inheritdoc />
	public decimal? MultipleOf => Target?.MultipleOf;

	/// <inheritdoc />
	public JsonNode? Default
	{
		get
		{
			JsonNode? jsonNode = base.Reference.Default;
			if (jsonNode == null)
			{
				IOpenApiSchema? target = Target;
				if (target == null)
				{
					return null;
				}
				jsonNode = target.Default;
			}
			return jsonNode;
		}
		set
		{
			base.Reference.Default = value;
		}
	}

	/// <inheritdoc />
	public bool ReadOnly
	{
		get
		{
			return base.Reference.ReadOnly ?? Target?.ReadOnly ?? false;
		}
		set
		{
			base.Reference.ReadOnly = value;
		}
	}

	/// <inheritdoc />
	public bool WriteOnly
	{
		get
		{
			return base.Reference.WriteOnly ?? Target?.WriteOnly ?? false;
		}
		set
		{
			base.Reference.WriteOnly = value;
		}
	}

	/// <inheritdoc />
	public IList<IOpenApiSchema>? AllOf => Target?.AllOf;

	/// <inheritdoc />
	public IList<IOpenApiSchema>? OneOf => Target?.OneOf;

	/// <inheritdoc />
	public IList<IOpenApiSchema>? AnyOf => Target?.AnyOf;

	/// <inheritdoc />
	public IOpenApiSchema? Not => Target?.Not;

	/// <inheritdoc />
	public ISet<string>? Required => Target?.Required;

	/// <inheritdoc />
	public IOpenApiSchema? Items => Target?.Items;

	/// <inheritdoc />
	public int? MaxItems => Target?.MaxItems;

	/// <inheritdoc />
	public int? MinItems => Target?.MinItems;

	/// <inheritdoc />
	public bool? UniqueItems => Target?.UniqueItems;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiSchema>? Properties => Target?.Properties;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiSchema>? PatternProperties => Target?.PatternProperties;

	/// <inheritdoc />
	public int? MaxProperties => Target?.MaxProperties;

	/// <inheritdoc />
	public int? MinProperties => Target?.MinProperties;

	/// <inheritdoc />
	public bool AdditionalPropertiesAllowed => Target?.AdditionalPropertiesAllowed ?? true;

	/// <inheritdoc />
	public IOpenApiSchema? AdditionalProperties => Target?.AdditionalProperties;

	/// <inheritdoc />
	public OpenApiDiscriminator? Discriminator => Target?.Discriminator;

	/// <inheritdoc />
	public JsonNode? Example => Target?.Example;

	/// <inheritdoc />
	public IList<JsonNode>? Examples
	{
		get
		{
			IList<JsonNode>? examples = base.Reference.Examples;
			if (examples == null)
			{
				IOpenApiSchema? target = Target;
				if (target == null)
				{
					return null;
				}
				examples = target.Examples;
			}
			return examples;
		}
		set
		{
			base.Reference.Examples = value;
		}
	}

	/// <inheritdoc />
	public IList<JsonNode>? Enum => Target?.Enum;

	/// <inheritdoc />
	public bool UnevaluatedProperties => Target?.UnevaluatedProperties ?? false;

	/// <inheritdoc />
	public OpenApiExternalDocs? ExternalDocs => Target?.ExternalDocs;

	/// <inheritdoc />
	public bool Deprecated
	{
		get
		{
			return base.Reference.Deprecated ?? Target?.Deprecated ?? false;
		}
		set
		{
			base.Reference.Deprecated = value;
		}
	}

	/// <inheritdoc />
	public OpenApiXml? Xml => Target?.Xml;

	/// <inheritdoc />
	public IDictionary<string, IOpenApiExtension>? Extensions => Target?.Extensions;

	/// <inheritdoc />
	public IDictionary<string, JsonNode>? UnrecognizedKeywords => Target?.UnrecognizedKeywords;

	/// <inheritdoc />
	public IDictionary<string, HashSet<string>>? DependentRequired => Target?.DependentRequired;

	/// <summary>
	/// Constructor initializing the reference object.
	/// </summary>
	/// <param name="referenceId">The reference Id.</param>
	/// <param name="hostDocument">The host OpenAPI document.</param>
	/// <param name="externalResource">Optional: External resource in the reference.
	/// It may be:
	/// 1. a absolute/relative file path, for example:  ../commons/pet.json
	/// 2. a Url, for example: http://localhost/pet.json
	/// </param>
	public OpenApiSchemaReference(string referenceId, OpenApiDocument? hostDocument = null, string? externalResource = null)
		: base(referenceId, hostDocument, ReferenceType.Schema, externalResource)
	{
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="schema">The schema reference to copy</param>
	private OpenApiSchemaReference(OpenApiSchemaReference schema)
		: base((BaseOpenApiReferenceHolder<OpenApiSchema, IOpenApiSchema, JsonSchemaReference>)schema)
	{
	}

	/// <inheritdoc />
	public override void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeAsWithoutLoops(writer, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			IOpenApiSerializable openApiSerializable;
			if (!(element is IOpenApiSchema source))
			{
				openApiSerializable = element;
			}
			else
			{
				IOpenApiSerializable openApiSerializable2 = CopyReferenceAsTargetElementWithOverrides(source);
				openApiSerializable = openApiSerializable2;
			}
			openApiSerializable.SerializeAsV31(w);
		});
	}

	/// <inheritdoc />
	public override void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeAsWithoutLoops(writer, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV3(w);
		});
	}

	/// <inheritdoc />
	public override void SerializeAsV2(IOpenApiWriter writer)
	{
		SerializeAsWithoutLoops(writer, delegate(IOpenApiWriter w, IOpenApiSerializable element)
		{
			element.SerializeAsV2(w);
		});
	}

	private void SerializeAsWithoutLoops(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> action)
	{
		if (!writer.GetSettings().ShouldInlineReference(base.Reference))
		{
			action(writer, base.Reference);
			return;
		}
		if (!writer.GetSettings().LoopDetector.PushLoop((IOpenApiSchema)this))
		{
			writer.GetSettings().LoopDetector.SaveLoop((IOpenApiSchema)this);
			action(writer, base.Reference);
			return;
		}
		SerializeInternal(writer, delegate(IOpenApiWriter w, IOpenApiSchema element)
		{
			action(w, element);
		});
		writer.GetSettings().LoopDetector.PopLoop<IOpenApiSchema>();
	}

	/// <inheritdoc />
	public override IOpenApiSchema CopyReferenceAsTargetElementWithOverrides(IOpenApiSchema source)
	{
		if (!(source is OpenApiSchema))
		{
			return source;
		}
		return new OpenApiSchema(this);
	}

	/// <inheritdoc />
	public IOpenApiSchema CreateShallowCopy()
	{
		return new OpenApiSchemaReference(this);
	}

	/// <inheritdoc />
	protected override JsonSchemaReference CopyReference(JsonSchemaReference sourceReference)
	{
		return new JsonSchemaReference(sourceReference);
	}
}
