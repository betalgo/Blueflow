using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Microsoft.OpenApi;

/// <summary>
/// A directory structure representing the paths of an OpenAPI document.
/// </summary>
public class OpenApiUrlTreeNode
{
	private const string RootPathSegment = "/";

	private const string PathSeparator = "\\";

	/// <summary>
	/// Dictionary that maps a set of HTTP methods to HTML color.  Keys are sorted, upper-cased, concatenated HTTP methods.
	/// </summary>
	internal static readonly IReadOnlyDictionary<string, MermaidNodeStyle> MermaidNodeStyles = new Dictionary<string, MermaidNodeStyle>(StringComparer.OrdinalIgnoreCase)
	{
		{
			"GET",
			new MermaidNodeStyle("lightSteelBlue", MermaidNodeShape.SquareCornerRectangle)
		},
		{
			"POST",
			new MermaidNodeStyle("Lightcoral", MermaidNodeShape.OddShape)
		},
		{
			"GET_POST",
			new MermaidNodeStyle("forestGreen", MermaidNodeShape.RoundedCornerRectangle)
		},
		{
			"DELETE_GET_PATCH",
			new MermaidNodeStyle("yellowGreen", MermaidNodeShape.Circle)
		},
		{
			"DELETE_GET_PATCH_PUT",
			new MermaidNodeStyle("oliveDrab", MermaidNodeShape.Circle)
		},
		{
			"DELETE_GET_PUT",
			new MermaidNodeStyle("olive", MermaidNodeShape.Circle)
		},
		{
			"DELETE_GET",
			new MermaidNodeStyle("DarkSeaGreen", MermaidNodeShape.Circle)
		},
		{
			"DELETE",
			new MermaidNodeStyle("Tomato", MermaidNodeShape.Rhombus)
		},
		{
			"OTHER",
			new MermaidNodeStyle("White", MermaidNodeShape.SquareCornerRectangle)
		}
	};

	/// <summary>
	/// All the subdirectories of a node.
	/// </summary>
	public IDictionary<string, OpenApiUrlTreeNode> Children { get; } = new Dictionary<string, OpenApiUrlTreeNode>();

	/// <summary>
	/// The relative directory path of the current node from the root node.
	/// </summary>
	public string Path { get; set; } = "";

	/// <summary>
	/// Dictionary of labels and Path Item objects that describe the operations available on a node.
	/// </summary>
	public IDictionary<string, IOpenApiPathItem> PathItems { get; } = new Dictionary<string, IOpenApiPathItem>();

	/// <summary>
	/// A dictionary of key value pairs that contain information about a node.
	/// </summary>
	public IDictionary<string, List<string>> AdditionalData { get; set; } = new Dictionary<string, List<string>>();

	/// <summary>
	/// Flag indicating whether a node segment is a path parameter.
	/// </summary>
	public bool IsParameter => Segment.StartsWith("{", StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// The subdirectory of a relative path.
	/// </summary>
	public string Segment { get; private set; }

	/// <summary>
	/// Flag indicating whether the node's PathItems dictionary has operations
	/// under a given label.
	/// </summary>
	/// <param name="label">The name of the key for the target operations
	/// in the node's PathItems dictionary.</param>
	/// <returns>true or false.</returns>
	public bool HasOperations(string label)
	{
		Utils.CheckArgumentNullOrEmpty(label, "label");
		if (PathItems != null && PathItems.TryGetValue(label, out IOpenApiPathItem value))
		{
			Dictionary<HttpMethod, OpenApiOperation> operations = value.Operations;
			if (operations != null)
			{
				return operations.Count > 0;
			}
			return false;
		}
		return false;
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="segment">The subdirectory of a relative path.</param>
	private OpenApiUrlTreeNode(string segment)
	{
		Segment = segment;
	}

	/// <summary>
	/// Creates an empty structured directory of <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node.
	/// </summary>
	/// <returns>The root node of the created <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> directory structure.</returns>
	public static OpenApiUrlTreeNode Create()
	{
		return new OpenApiUrlTreeNode("/");
	}

	/// <summary>
	/// Creates a structured directory of <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> nodes from the paths of an OpenAPI document.
	/// </summary>
	/// <param name="doc">The OpenAPI document.</param>
	/// <param name="label">Name tag for labelling the <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> nodes in the directory structure.</param>
	/// <returns>The root node of the created <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> directory structure.</returns>
	public static OpenApiUrlTreeNode Create(OpenApiDocument doc, string label)
	{
		Utils.CheckArgumentNull(doc, "doc");
		Utils.CheckArgumentNullOrEmpty(label, "label");
		OpenApiUrlTreeNode openApiUrlTreeNode = Create();
		OpenApiPaths openApiPaths = doc?.Paths;
		if (openApiPaths != null)
		{
			foreach (KeyValuePair<string, IOpenApiPathItem> item in openApiPaths)
			{
				openApiUrlTreeNode.Attach(item.Key, item.Value, label);
			}
		}
		return openApiUrlTreeNode;
	}

	/// <summary>
	/// Retrieves the paths from an OpenAPI document and appends the items to an <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node.
	/// </summary>
	/// <param name="doc">The OpenAPI document.</param>
	/// <param name="label">Name tag for labelling related <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> nodes in the directory structure.</param>
	public void Attach(OpenApiDocument doc, string label)
	{
		Utils.CheckArgumentNull(doc, "doc");
		Utils.CheckArgumentNullOrEmpty(label, "label");
		OpenApiPaths paths = doc.Paths;
		if (paths == null)
		{
			return;
		}
		foreach (KeyValuePair<string, IOpenApiPathItem> item in paths)
		{
			Attach(item.Key, item.Value, label);
		}
	}

	/// <summary>
	/// Appends a path and the PathItem to an <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node.
	/// </summary>
	/// <param name="path">An OpenAPI path.</param>
	/// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
	/// <param name="label">A name tag for labelling the <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node.</param>
	/// <returns>An <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node describing an OpenAPI path.</returns>
	public OpenApiUrlTreeNode Attach(string path, IOpenApiPathItem pathItem, string label)
	{
		Utils.CheckArgumentNullOrEmpty(label, "label");
		Utils.CheckArgumentNullOrEmpty(path, "path");
		Utils.CheckArgumentNull(pathItem, "pathItem");
		if (path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			path = path.Substring(1);
		}
		string[] segments = path.Split('/');
		return Attach(segments, pathItem, label, "");
	}

	/// <summary>
	/// Assembles the constituent properties of an <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node.
	/// </summary>
	/// <param name="segments">IEnumerable subdirectories of a relative path.</param>
	/// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
	/// <param name="label">A name tag for labelling the <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node.</param>
	/// <param name="currentPath">The relative path of a node.</param>
	/// <returns>An <see cref="T:Microsoft.OpenApi.OpenApiUrlTreeNode" /> node with all constituent properties assembled.</returns>
	private OpenApiUrlTreeNode Attach(IEnumerable<string> segments, IOpenApiPathItem pathItem, string label, string currentPath)
	{
		string text = segments.FirstOrDefault();
		if (text == null)
		{
			if (PathItems.ContainsKey(label))
			{
				throw new ArgumentException("A duplicate label already exists for this node: " + label, "label");
			}
			Path = currentPath;
			PathItems.Add(label, pathItem);
			return this;
		}
		if (text.Length == 0 && currentPath.Length == 0)
		{
			Path = currentPath;
			PathItems.Add(label, pathItem);
			return this;
		}
		if (Children.TryGetValue(text, out OpenApiUrlTreeNode value))
		{
			string currentPath2 = currentPath + "\\" + text;
			return value.Attach(segments.Skip(1), pathItem, label, currentPath2);
		}
		string text2 = currentPath + "\\" + text;
		OpenApiUrlTreeNode openApiUrlTreeNode = new OpenApiUrlTreeNode(text)
		{
			Path = text2
		};
		Children[text] = openApiUrlTreeNode;
		return openApiUrlTreeNode.Attach(segments.Skip(1), pathItem, label, text2);
	}

	/// <summary>
	/// Adds additional data information to the AdditionalData property of the node.
	/// </summary>
	/// <param name="additionalData">A dictionary of key value pairs that contain information about a node.</param>
	public void AddAdditionalData(Dictionary<string, List<string>> additionalData)
	{
		Utils.CheckArgumentNull(additionalData, "additionalData");
		foreach (KeyValuePair<string, List<string>> additionalDatum in additionalData)
		{
			AdditionalData[additionalDatum.Key] = additionalDatum.Value;
		}
	}

	/// <summary>
	/// Write tree as Mermaid syntax
	/// </summary>
	/// <param name="writer">StreamWriter to write the Mermaid content to</param>
	public void WriteMermaid(TextWriter writer)
	{
		writer.WriteLine("graph LR");
		foreach (KeyValuePair<string, MermaidNodeStyle> mermaidNodeStyle in MermaidNodeStyles)
		{
			writer.WriteLine($"classDef {mermaidNodeStyle.Key} fill:{mermaidNodeStyle.Value.Color},stroke:#333,stroke-width:2px");
		}
		ProcessNode(this, writer);
	}

	private static void ProcessNode(OpenApiUrlTreeNode node, TextWriter writer)
	{
		string text = (string.IsNullOrEmpty(node.Path) ? "/" : SanitizeMermaidNode(node.Path));
		string text2 = GetMethods(node);
		var (value, value2) = GetShapeDelimiters(text2);
		foreach (KeyValuePair<string, OpenApiUrlTreeNode> child in node.Children)
		{
			var (value3, value4) = GetShapeDelimiters(GetMethods(child.Value));
			writer.WriteLine($"{text}{value}\"{node.Segment}\"{value2} --> {SanitizeMermaidNode(child.Value.Path)}{value3}\"{child.Key}\"{value4}");
			ProcessNode(child.Value, writer);
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = "OTHER";
		}
		writer.WriteLine("class " + text + " " + text2);
	}

	private static string GetMethods(OpenApiUrlTreeNode node)
	{
		return string.Join("_", (from o in node.PathItems.Where<KeyValuePair<string, IOpenApiPathItem>>((KeyValuePair<string, IOpenApiPathItem> p) => p.Value.Operations != null).SelectMany((KeyValuePair<string, IOpenApiPathItem> p) => p.Value.Operations.Select<KeyValuePair<HttpMethod, OpenApiOperation>, HttpMethod>((KeyValuePair<HttpMethod, OpenApiOperation> o) => o.Key)).Distinct()
			select o.ToString().ToUpper() into o
			orderby o
			select o).ToList());
	}

	private static (string, string) GetShapeDelimiters(string methods)
	{
		if (MermaidNodeStyles.TryGetValue(methods, out MermaidNodeStyle value))
		{
			return value.Shape switch
			{
				MermaidNodeShape.Circle => ("((", "))"), 
				MermaidNodeShape.RoundedCornerRectangle => ("(", ")"), 
				MermaidNodeShape.Rhombus => ("{", "}"), 
				MermaidNodeShape.SquareCornerRectangle => ("[", "]"), 
				MermaidNodeShape.OddShape => (">", "]"), 
				_ => ("[", "]"), 
			};
		}
		return ("[", "]");
	}

	private static string SanitizeMermaidNode(string token)
	{
		return token.Replace("\\", "/").Replace("{", ":").Replace("}", "")
			.Replace(".", "_")
			.Replace("(", "_")
			.Replace(")", "_")
			.Replace(";", "_")
			.Replace("-", "_")
			.Replace("graph", "gra_ph")
			.Replace("default", "def_ault");
	}
}
