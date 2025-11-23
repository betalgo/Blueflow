using System;
using System.CodeDom.Compiler;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Microsoft.OpenApi.Reader;

[JsonSerializable(typeof(JsonObject))]
[GeneratedCode("System.Text.Json.SourceGeneration", "8.0.13.2707")]
internal class SourceGenerationContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<JsonObject>? _JsonObject;

	private static readonly JsonSerializerOptions s_defaultOptions = new JsonSerializerOptions();

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<JsonObject> JsonObject => _JsonObject ?? (_JsonObject = (JsonTypeInfo<JsonObject>)base.Options.GetTypeInfo(typeof(JsonObject)));

	/// <summary>
	/// The default <see cref="T:System.Text.Json.Serialization.JsonSerializerContext" /> associated with a default <see cref="T:System.Text.Json.JsonSerializerOptions" /> instance.
	/// </summary>
	public static SourceGenerationContext Default { get; } = new SourceGenerationContext(new JsonSerializerOptions(s_defaultOptions));

	/// <summary>
	/// The source-generated options associated with this context.
	/// </summary>
	protected override JsonSerializerOptions? GeneratedSerializerOptions { get; } = s_defaultOptions;

	private JsonTypeInfo<JsonObject> Create_JsonObject(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<JsonObject> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<JsonObject>(options, JsonMetadataServices.JsonObjectConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	/// <inheritdoc />
	public SourceGenerationContext()
		: base(null)
	{
	}

	/// <inheritdoc />
	public SourceGenerationContext(JsonSerializerOptions options)
		: base(options)
	{
	}

	private static bool TryGetTypeInfoForRuntimeCustomConverter<TJsonMetadataType>(JsonSerializerOptions options, out JsonTypeInfo<TJsonMetadataType> jsonTypeInfo)
	{
		JsonConverter runtimeConverterForType = GetRuntimeConverterForType(typeof(TJsonMetadataType), options);
		if (runtimeConverterForType != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<TJsonMetadataType>(options, runtimeConverterForType);
			return true;
		}
		jsonTypeInfo = null;
		return false;
	}

	private static JsonConverter? GetRuntimeConverterForType(Type type, JsonSerializerOptions options)
	{
		for (int i = 0; i < options.Converters.Count; i++)
		{
			JsonConverter jsonConverter = options.Converters[i];
			if (jsonConverter != null && jsonConverter.CanConvert(type))
			{
				return ExpandConverter(type, jsonConverter, options, validateCanConvert: false);
			}
		}
		return null;
	}

	private static JsonConverter ExpandConverter(Type type, JsonConverter converter, JsonSerializerOptions options, bool validateCanConvert = true)
	{
		if (validateCanConvert && !converter.CanConvert(type))
		{
			throw new InvalidOperationException($"The converter '{converter.GetType()}' is not compatible with the type '{type}'.");
		}
		if (converter is JsonConverterFactory jsonConverterFactory)
		{
			converter = jsonConverterFactory.CreateConverter(type, options);
			if (converter == null || converter is JsonConverterFactory)
			{
				throw new InvalidOperationException($"The converter '{jsonConverterFactory.GetType()}' cannot return null or a JsonConverterFactory instance.");
			}
		}
		return converter;
	}

	/// <inheritdoc />
	public override JsonTypeInfo? GetTypeInfo(Type type)
	{
		base.Options.TryGetTypeInfo(type, out JsonTypeInfo typeInfo);
		return typeInfo;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(JsonObject))
		{
			return Create_JsonObject(options);
		}
		return null;
	}
}
