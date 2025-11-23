using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers;

public class ParsingContext
{
	private readonly Stack<string> _currentLocation = new Stack<string>();

	private readonly Dictionary<string, object> _tempStorage = new Dictionary<string, object>();

	private readonly Dictionary<object, Dictionary<string, object>> _scopedTempStorage = new Dictionary<object, Dictionary<string, object>>();

	private readonly Dictionary<string, Stack<string>> _loopStacks = new Dictionary<string, Stack<string>>();

	internal Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>> ExtensionParsers { get; set; } = new Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>>();

	internal RootNode RootNode { get; set; }

	internal List<OpenApiTag> Tags { get; private set; } = new List<OpenApiTag>();

	internal Uri BaseUrl { get; set; }

	internal List<string> DefaultContentType { get; set; }

	public OpenApiDiagnostic Diagnostic { get; }

	internal IOpenApiVersionService VersionService { get; set; }

	public ParsingContext(OpenApiDiagnostic diagnostic)
	{
		Diagnostic = diagnostic;
	}

	internal OpenApiDocument Parse(YamlDocument yamlDocument)
	{
		RootNode = new RootNode(this, yamlDocument);
		string version = GetVersion(RootNode);
		string text = version;
		if (text != null)
		{
			OpenApiDocument result;
			if (text == "2.0")
			{
				VersionService = new OpenApiV2VersionService(Diagnostic);
				result = VersionService.LoadDocument(RootNode);
				Diagnostic.SpecificationVersion = (OpenApiSpecVersion)0;
			}
			else
			{
				if (!text.StartsWith("3.0"))
				{
					goto IL_009a;
				}
				VersionService = new OpenApiV3VersionService(Diagnostic);
				result = VersionService.LoadDocument(RootNode);
				Diagnostic.SpecificationVersion = (OpenApiSpecVersion)1;
			}
			return result;
		}
		goto IL_009a;
		IL_009a:
		throw new OpenApiUnsupportedSpecVersionException(version);
	}

	internal T ParseFragment<T>(YamlDocument yamlDocument, OpenApiSpecVersion version) where T : IOpenApiElement
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		ParseNode node = ParseNode.Create(this, yamlDocument.RootNode);
		T result = default(T);
		if ((int)version != 0)
		{
			if ((int)version == 1)
			{
				VersionService = new OpenApiV3VersionService(Diagnostic);
				return VersionService.LoadElement<T>(node);
			}
			return result;
		}
		VersionService = new OpenApiV2VersionService(Diagnostic);
		return VersionService.LoadElement<T>(node);
	}

	private static string GetVersion(RootNode rootNode)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		ParseNode parseNode = rootNode.Find(new JsonPointer("/openapi"));
		if (parseNode != null)
		{
			return parseNode.GetScalarValue();
		}
		return rootNode.Find(new JsonPointer("/swagger"))?.GetScalarValue();
	}

	public void EndObject()
	{
		_currentLocation.Pop();
	}

	public string GetLocation()
	{
		return "#/" + string.Join("/", (from s in _currentLocation.Reverse()
			select s.Replace("~", "~0").Replace("/", "~1")).ToArray());
	}

	public T GetFromTempStorage<T>(string key, object scope = null)
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

	public void SetTempStorage(string key, object value, object scope = null)
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

	public void StartObject(string objectName)
	{
		_currentLocation.Push(objectName);
	}

	public bool PushLoop(string loopId, string key)
	{
		if (!_loopStacks.TryGetValue(loopId, out var value))
		{
			value = new Stack<string>();
			_loopStacks.Add(loopId, value);
		}
		if (!value.Contains(key))
		{
			value.Push(key);
			return true;
		}
		return false;
	}

	internal void ClearLoop(string loopid)
	{
		_loopStacks[loopid].Clear();
	}

	public void PopLoop(string loopid)
	{
		if (_loopStacks[loopid].Count > 0)
		{
			_loopStacks[loopid].Pop();
		}
	}
}
