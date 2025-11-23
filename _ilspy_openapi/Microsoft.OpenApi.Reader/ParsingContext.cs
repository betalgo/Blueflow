using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.Reader.V3;
using Microsoft.OpenApi.Reader.V31;
using Microsoft.OpenApi.Reader.V32;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// The Parsing Context holds temporary state needed whilst parsing an OpenAPI Document
/// </summary>
public class ParsingContext
{
	private readonly Stack<string> _currentLocation = new Stack<string>();

	private readonly Dictionary<string, object> _tempStorage = new Dictionary<string, object>();

	private readonly Dictionary<object, Dictionary<string, object>> _scopedTempStorage = new Dictionary<object, Dictionary<string, object>>();

	/// <summary>
	/// Extension parsers
	/// </summary>
	public Dictionary<string, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension>>? ExtensionParsers { get; set; } = new Dictionary<string, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension>>();

	internal RootNode? RootNode { get; set; }

	internal List<OpenApiTag> Tags { get; private set; } = new List<OpenApiTag>();

	/// <summary>
	/// The base url for the document
	/// </summary>
	public Uri? BaseUrl { get; set; }

	/// <summary>
	/// Default content type for a response object
	/// </summary>
	public List<string>? DefaultContentType { get; set; }

	/// <summary>
	/// Diagnostic object that returns metadata about the parsing process.
	/// </summary>
	public OpenApiDiagnostic Diagnostic { get; }

	/// <summary>
	/// Service providing all Version specific conversion functions
	/// </summary>
	internal IOpenApiVersionService? VersionService { get; set; }

	/// <summary>
	/// Create Parsing Context
	/// </summary>
	/// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
	public ParsingContext(OpenApiDiagnostic diagnostic)
	{
		Diagnostic = diagnostic;
	}

	/// <summary>
	/// Initiates the parsing process.  Not thread safe and should only be called once on a parsing context
	/// </summary>
	/// <param name="jsonNode">Set of Json nodes to parse.</param>
	/// <param name="location">Location of where the document that is getting loaded is saved</param>
	/// <returns>An OpenApiDocument populated based on the passed yamlDocument </returns>
	public OpenApiDocument Parse(JsonNode jsonNode, Uri location)
	{
		RootNode = new RootNode(this, jsonNode);
		string version = GetVersion(RootNode);
		string text = version;
		if (text != null)
		{
			OpenApiDocument openApiDocument;
			if (text.is2_0())
			{
				VersionService = new OpenApiV2VersionService(Diagnostic);
				openApiDocument = VersionService.LoadDocument(RootNode, location);
				Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi2_0;
				ValidateRequiredFields(openApiDocument, text);
			}
			else
			{
				string version2 = text;
				if (version2.is3_0())
				{
					VersionService = new OpenApiV3VersionService(Diagnostic);
					openApiDocument = VersionService.LoadDocument(RootNode, location);
					Diagnostic.SpecificationVersion = ((!version2.is3_1()) ? OpenApiSpecVersion.OpenApi3_0 : OpenApiSpecVersion.OpenApi3_1);
					ValidateRequiredFields(openApiDocument, version2);
				}
				else
				{
					string version3 = text;
					if (version3.is3_1())
					{
						VersionService = new OpenApiV31VersionService(Diagnostic);
						openApiDocument = VersionService.LoadDocument(RootNode, location);
						Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi3_1;
						ValidateRequiredFields(openApiDocument, version3);
					}
					else
					{
						string version4 = text;
						if (!version4.is3_2())
						{
							goto IL_0146;
						}
						VersionService = new OpenApiV32VersionService(Diagnostic);
						openApiDocument = VersionService.LoadDocument(RootNode, location);
						Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi3_2;
						ValidateRequiredFields(openApiDocument, version4);
					}
				}
			}
			return openApiDocument;
		}
		goto IL_0146;
		IL_0146:
		throw new OpenApiUnsupportedSpecVersionException(version);
	}

	/// <summary>
	/// Initiates the parsing process of a fragment.  Not thread safe and should only be called once on a parsing context
	/// </summary>
	/// <param name="jsonNode"></param>
	/// <param name="version">OpenAPI version of the fragment</param>
	/// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
	/// <returns>An OpenApiDocument populated based on the passed yamlDocument </returns>
	public T? ParseFragment<T>(JsonNode jsonNode, OpenApiSpecVersion version, OpenApiDocument openApiDocument) where T : IOpenApiElement
	{
		ParseNode node = ParseNode.Create(this, jsonNode);
		T result = default(T);
		switch (version)
		{
		case OpenApiSpecVersion.OpenApi2_0:
			VersionService = new OpenApiV2VersionService(Diagnostic);
			return VersionService.LoadElement<T>(node, openApiDocument);
		case OpenApiSpecVersion.OpenApi3_0:
			VersionService = new OpenApiV3VersionService(Diagnostic);
			return VersionService.LoadElement<T>(node, openApiDocument);
		case OpenApiSpecVersion.OpenApi3_1:
			VersionService = new OpenApiV31VersionService(Diagnostic);
			return VersionService.LoadElement<T>(node, openApiDocument);
		case OpenApiSpecVersion.OpenApi3_2:
			VersionService = new OpenApiV32VersionService(Diagnostic);
			return VersionService.LoadElement<T>(node, openApiDocument);
		default:
			return result;
		}
	}

	/// <summary>
	/// Gets the version of the Open API document.
	/// </summary>
	private static string GetVersion(RootNode rootNode)
	{
		ParseNode parseNode = rootNode.Find(new JsonPointer("/openapi"));
		if (parseNode != null)
		{
			return parseNode.GetScalarValue().Replace("\"", string.Empty);
		}
		return rootNode.Find(new JsonPointer("/swagger"))?.GetScalarValue().Replace("\"", string.Empty) ?? throw new OpenApiException("Version node not found.");
	}

	/// <summary>
	/// End the current object.
	/// </summary>
	public void EndObject()
	{
		_currentLocation.Pop();
	}

	/// <summary>
	/// Get the current location as string representing JSON pointer.
	/// </summary>
	public string GetLocation()
	{
		return "#/" + string.Join("/", (from s in _currentLocation.Reverse()
			select s.Replace("~", "~0").Replace("/", "~1")).ToArray());
	}

	/// <summary>
	/// Gets the value from the temporary storage matching the given key.
	/// </summary>
	public T? GetFromTempStorage<T>(string key, object? scope = null)
	{
		Dictionary<string, object> value;
		if (scope == null)
		{
			value = _tempStorage;
		}
		else if (!_scopedTempStorage.TryGetValue(scope, out value))
		{
			return default(T);
		}
		if (!value.TryGetValue(key, out var value2))
		{
			return default(T);
		}
		return (T)value2;
	}

	/// <summary>
	/// Sets the temporary storage for this key and value.
	/// </summary>
	public void SetTempStorage(string key, object? value, object? scope = null)
	{
		Dictionary<string, object> value2;
		if (scope == null)
		{
			value2 = _tempStorage;
		}
		else if (!_scopedTempStorage.TryGetValue(scope, out value2))
		{
			Dictionary<string, object> dictionary = (_scopedTempStorage[scope] = new Dictionary<string, object>());
			value2 = dictionary;
		}
		if (value == null)
		{
			value2.Remove(key);
		}
		else
		{
			value2[key] = value;
		}
	}

	/// <summary>
	/// Starts an object with the given object name.
	/// </summary>
	public void StartObject(string objectName)
	{
		_currentLocation.Push(objectName);
	}

	private void ValidateRequiredFields(OpenApiDocument doc, string version)
	{
		if ((version.is2_0() || version.is3_0()) && doc.Paths == null && RootNode != null)
		{
			RootNode.Context.Diagnostic.Errors.Add(new OpenApiError("", "Paths is a REQUIRED field at " + RootNode.Context.GetLocation()));
		}
	}
}
