using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Open API visitor base provides common logic for concrete visitors
/// </summary>
public abstract class OpenApiVisitorBase
{
	private readonly Stack<string> _path = new Stack<string>();

	/// <summary>
	/// Properties available to identify context of where an object is within OpenAPI Document
	/// </summary>
	public CurrentKeys CurrentKeys { get; } = new CurrentKeys();

	/// <summary>
	/// Pointer to source of validation error in document
	/// </summary>
	public string PathString => "#/" + string.Join("/", _path.Reverse());

	/// <summary>
	/// Allow Rule to indicate validation error occurred at a deeper context level.
	/// </summary>
	/// <param name="segment">Identifier for context</param>
	public virtual void Enter(string segment)
	{
		_path.Push(segment);
	}

	/// <summary>
	/// Exit from path context level.  Enter and Exit calls should be matched.
	/// </summary>
	public virtual void Exit()
	{
		_path.Pop();
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiDocument" />
	/// </summary>
	public virtual void Visit(OpenApiDocument doc)
	{
	}

	/// <summary>
	/// Visits <see cref="T:System.Text.Json.Nodes.JsonNode" />
	/// </summary>
	/// <param name="node"></param>
	public virtual void Visit(JsonNode node)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiInfo" />
	/// </summary>
	public virtual void Visit(OpenApiInfo info)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiContact" />
	/// </summary>
	public virtual void Visit(OpenApiContact contact)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiLicense" />
	/// </summary>
	public virtual void Visit(OpenApiLicense license)
	{
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiServer" />
	/// </summary>
	public virtual void Visit(IList<OpenApiServer> servers)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiServer" />
	/// </summary>
	public virtual void Visit(OpenApiServer server)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiPaths" />
	/// </summary>
	public virtual void Visit(OpenApiPaths paths)
	{
	}

	/// <summary>
	/// Visits Webhooks&gt;
	/// </summary>
	public virtual void Visit(IDictionary<string, IOpenApiPathItem> webhooks)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiPathItem" />
	/// </summary>
	public virtual void Visit(IOpenApiPathItem pathItem)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiServerVariable" />
	/// </summary>
	public virtual void Visit(OpenApiServerVariable serverVariable)
	{
	}

	/// <summary>
	/// Visits the operations.
	/// </summary>
	public virtual void Visit(IDictionary<HttpMethod, OpenApiOperation> operations)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiOperation" />
	/// </summary>
	public virtual void Visit(OpenApiOperation operation)
	{
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiParameter" />
	/// </summary>
	public virtual void Visit(IList<IOpenApiParameter> parameters)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiParameter" />
	/// </summary>
	public virtual void Visit(IOpenApiParameter parameter)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiRequestBody" />
	/// </summary>
	public virtual void Visit(IOpenApiRequestBody requestBody)
	{
	}

	/// <summary>
	/// Visits headers.
	/// </summary>
	public virtual void Visit(IDictionary<string, IOpenApiHeader> headers)
	{
	}

	/// <summary>
	/// Visits callbacks.
	/// </summary>
	public virtual void Visit(IDictionary<string, IOpenApiCallback> callbacks)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiResponse" />
	/// </summary>
	public virtual void Visit(IOpenApiResponse response)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiResponses" />
	/// </summary>
	public virtual void Visit(OpenApiResponses response)
	{
	}

	/// <summary>
	/// Visits media type content.
	/// </summary>
	public virtual void Visit(IDictionary<string, IOpenApiMediaType> content)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiMediaType" />
	/// </summary>
	public virtual void Visit(IOpenApiMediaType mediaType)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiEncoding" />
	/// </summary>
	public virtual void Visit(OpenApiEncoding encoding)
	{
	}

	/// <summary>
	/// Visits the examples.
	/// </summary>
	public virtual void Visit(IDictionary<string, IOpenApiExample> examples)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiComponents" />
	/// </summary>
	public virtual void Visit(OpenApiComponents components)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiComponents" />
	/// </summary>
	public virtual void Visit(OpenApiExternalDocs externalDocs)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiSchema" />
	/// </summary>
	public virtual void Visit(IOpenApiSchema schema)
	{
	}

	/// <summary>
	/// Visits the links.
	/// </summary>
	public virtual void Visit(IDictionary<string, IOpenApiLink> links)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiLink" />
	/// </summary>
	public virtual void Visit(IOpenApiLink link)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiCallback" />
	/// </summary>
	public virtual void Visit(IOpenApiCallback callback)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiTag" />
	/// </summary>
	public virtual void Visit(OpenApiTag tag)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiTagReference" />
	/// </summary>
	public virtual void Visit(OpenApiTagReference tag)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiHeader" />
	/// </summary>
	public virtual void Visit(IOpenApiHeader header)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiOAuthFlow" />
	/// </summary>
	public virtual void Visit(OpenApiOAuthFlow openApiOAuthFlow)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" />
	/// </summary>
	public virtual void Visit(OpenApiSecurityRequirement securityRequirement)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiSecurityScheme" />
	/// </summary>
	public virtual void Visit(IOpenApiSecurityScheme securityScheme)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiExample" />
	/// </summary>
	public virtual void Visit(IOpenApiExample example)
	{
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiTag" />
	/// </summary>
	public virtual void Visit(ISet<OpenApiTag> openApiTags)
	{
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiTagReference" />
	/// </summary>
	public virtual void Visit(ISet<OpenApiTagReference> openApiTags)
	{
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" />
	/// </summary>
	public virtual void Visit(IList<OpenApiSecurityRequirement> openApiSecurityRequirements)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiExtensible" />
	/// </summary>
	public virtual void Visit(IOpenApiExtensible openApiExtensible)
	{
	}

	/// <summary>
	/// Visits <see cref="T:Microsoft.OpenApi.IOpenApiExtension" />
	/// </summary>
	public virtual void Visit(IOpenApiExtension openApiExtension)
	{
	}

	/// <summary>
	/// Visits list of <see cref="T:Microsoft.OpenApi.IOpenApiExample" />
	/// </summary>
	public virtual void Visit(List<IOpenApiExample> example)
	{
	}

	/// <summary>
	/// Visits a dictionary of server variables
	/// </summary>
	public virtual void Visit(IDictionary<string, OpenApiServerVariable> serverVariables)
	{
	}

	/// <summary>
	/// Visits a dictionary of encodings
	/// </summary>
	/// <param name="encodings"></param>
	public virtual void Visit(IDictionary<string, OpenApiEncoding> encodings)
	{
	}

	/// <summary>
	/// Visits IOpenApiReferenceable instances that are references and not in components
	/// </summary>
	/// <param name="referenceHolder">Referencing object</param>
	public virtual void Visit(IOpenApiReferenceHolder referenceHolder)
	{
	}
}
