using System.Collections.Generic;

namespace Microsoft.OpenApi;

internal class CopyReferences(OpenApiDocument target) : OpenApiVisitorBase()
{
	private readonly OpenApiDocument _target = target;

	public OpenApiComponents Components = new OpenApiComponents();

	/// <inheritdoc />
	public override void Visit(IOpenApiReferenceHolder referenceHolder)
	{
		if (!(referenceHolder is OpenApiSchemaReference openApiSchemaReference))
		{
			if (!(referenceHolder is OpenApiSchema schema))
			{
				if (!(referenceHolder is OpenApiParameterReference openApiParameterReference))
				{
					if (!(referenceHolder is OpenApiParameter parameter))
					{
						if (!(referenceHolder is OpenApiResponseReference openApiResponseReference))
						{
							if (!(referenceHolder is OpenApiResponse response))
							{
								if (!(referenceHolder is OpenApiRequestBodyReference openApiRequestBodyReference))
								{
									if (!(referenceHolder is OpenApiRequestBody requestBody))
									{
										if (!(referenceHolder is OpenApiExampleReference openApiExampleReference))
										{
											if (!(referenceHolder is OpenApiExample example))
											{
												if (!(referenceHolder is OpenApiHeaderReference openApiHeaderReference))
												{
													if (!(referenceHolder is OpenApiHeader header))
													{
														if (!(referenceHolder is OpenApiCallbackReference openApiCallbackReference))
														{
															if (!(referenceHolder is OpenApiCallback callback))
															{
																if (!(referenceHolder is OpenApiLinkReference openApiLinkReference))
																{
																	if (!(referenceHolder is OpenApiLink link))
																	{
																		if (!(referenceHolder is OpenApiSecuritySchemeReference openApiSecuritySchemeReference))
																		{
																			if (!(referenceHolder is OpenApiSecurityScheme securityScheme))
																			{
																				if (!(referenceHolder is OpenApiPathItemReference openApiPathItemReference))
																				{
																					if (referenceHolder is OpenApiPathItem pathItem)
																					{
																						AddPathItemToComponents(pathItem);
																					}
																				}
																				else
																				{
																					AddPathItemToComponents(openApiPathItemReference.Target, openApiPathItemReference.Reference?.Id);
																				}
																			}
																			else
																			{
																				AddSecuritySchemeToComponents(securityScheme);
																			}
																		}
																		else
																		{
																			AddSecuritySchemeToComponents(openApiSecuritySchemeReference.Target, openApiSecuritySchemeReference.Reference?.Id);
																		}
																	}
																	else
																	{
																		AddLinkToComponents(link);
																	}
																}
																else
																{
																	AddLinkToComponents(openApiLinkReference.Target, openApiLinkReference.Reference?.Id);
																}
															}
															else
															{
																AddCallbackToComponents(callback);
															}
														}
														else
														{
															AddCallbackToComponents(openApiCallbackReference.Target, openApiCallbackReference.Reference?.Id);
														}
													}
													else
													{
														AddHeaderToComponents(header);
													}
												}
												else
												{
													AddHeaderToComponents(openApiHeaderReference.Target, openApiHeaderReference.Reference?.Id);
												}
											}
											else
											{
												AddExampleToComponents(example);
											}
										}
										else
										{
											AddExampleToComponents(openApiExampleReference.Target, openApiExampleReference.Reference?.Id);
										}
									}
									else
									{
										AddRequestBodyToComponents(requestBody);
									}
								}
								else
								{
									AddRequestBodyToComponents(openApiRequestBodyReference.Target, openApiRequestBodyReference.Reference?.Id);
								}
							}
							else
							{
								AddResponseToComponents(response);
							}
						}
						else
						{
							AddResponseToComponents(openApiResponseReference.Target, openApiResponseReference.Reference?.Id);
						}
					}
					else
					{
						AddParameterToComponents(parameter);
					}
				}
				else
				{
					AddParameterToComponents(openApiParameterReference.Target, openApiParameterReference.Reference?.Id);
				}
			}
			else
			{
				AddSchemaToComponents(schema);
			}
		}
		else
		{
			AddSchemaToComponents(openApiSchemaReference.Target, openApiSchemaReference.Reference?.Id);
		}
		base.Visit(referenceHolder);
	}

	private void AddSchemaToComponents(IOpenApiSchema? schema, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureSchemasExist();
		if (referenceId == null || schema == null)
		{
			return;
		}
		IDictionary<string, IOpenApiSchema>? schemas = Components.Schemas;
		if (schemas == null || !schemas.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.Schemas == null)
			{
				IDictionary<string, IOpenApiSchema> dictionary = (components.Schemas = new Dictionary<string, IOpenApiSchema>());
			}
			Components.Schemas.Add(referenceId, schema);
		}
	}

	private void AddParameterToComponents(IOpenApiParameter? parameter, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureParametersExist();
		if (parameter == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiParameter>? parameters = Components.Parameters;
		if (parameters == null || !parameters.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.Parameters == null)
			{
				IDictionary<string, IOpenApiParameter> dictionary = (components.Parameters = new Dictionary<string, IOpenApiParameter>());
			}
			Components.Parameters.Add(referenceId, parameter);
		}
	}

	private void AddResponseToComponents(IOpenApiResponse? response, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureResponsesExist();
		if (referenceId == null || response == null)
		{
			return;
		}
		IDictionary<string, IOpenApiResponse>? responses = Components.Responses;
		if (responses == null || !responses.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.Responses == null)
			{
				IDictionary<string, IOpenApiResponse> dictionary = (components.Responses = new Dictionary<string, IOpenApiResponse>());
			}
			Components.Responses.Add(referenceId, response);
		}
	}

	private void AddRequestBodyToComponents(IOpenApiRequestBody? requestBody, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureRequestBodiesExist();
		if (requestBody == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiRequestBody>? requestBodies = Components.RequestBodies;
		if (requestBodies == null || !requestBodies.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.RequestBodies == null)
			{
				IDictionary<string, IOpenApiRequestBody> dictionary = (components.RequestBodies = new Dictionary<string, IOpenApiRequestBody>());
			}
			Components.RequestBodies.Add(referenceId, requestBody);
		}
	}

	private void AddLinkToComponents(IOpenApiLink? link, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureLinksExist();
		if (link == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiLink>? links = Components.Links;
		if (links == null || !links.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.Links == null)
			{
				IDictionary<string, IOpenApiLink> dictionary = (components.Links = new Dictionary<string, IOpenApiLink>());
			}
			Components.Links.Add(referenceId, link);
		}
	}

	private void AddCallbackToComponents(IOpenApiCallback? callback, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureCallbacksExist();
		if (callback == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiCallback>? callbacks = Components.Callbacks;
		if (callbacks == null || !callbacks.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.Callbacks == null)
			{
				IDictionary<string, IOpenApiCallback> dictionary = (components.Callbacks = new Dictionary<string, IOpenApiCallback>());
			}
			Components.Callbacks.Add(referenceId, callback);
		}
	}

	private void AddHeaderToComponents(IOpenApiHeader? header, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureHeadersExist();
		if (header == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiHeader>? headers = Components.Headers;
		if (headers == null || !headers.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.Headers == null)
			{
				IDictionary<string, IOpenApiHeader> dictionary = (components.Headers = new Dictionary<string, IOpenApiHeader>());
			}
			Components.Headers.Add(referenceId, header);
		}
	}

	private void AddExampleToComponents(IOpenApiExample? example, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureExamplesExist();
		if (example == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiExample>? examples = Components.Examples;
		if (examples == null || !examples.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.Examples == null)
			{
				IDictionary<string, IOpenApiExample> dictionary = (components.Examples = new Dictionary<string, IOpenApiExample>());
			}
			Components.Examples.Add(referenceId, example);
		}
	}

	private void AddPathItemToComponents(IOpenApiPathItem? pathItem, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsurePathItemsExist();
		if (pathItem == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiPathItem>? pathItems = Components.PathItems;
		if (pathItems == null || !pathItems.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.PathItems == null)
			{
				IDictionary<string, IOpenApiPathItem> dictionary = (components.PathItems = new Dictionary<string, IOpenApiPathItem>());
			}
			Components.PathItems.Add(referenceId, pathItem);
		}
	}

	private void AddSecuritySchemeToComponents(IOpenApiSecurityScheme? securityScheme, string? referenceId = null)
	{
		EnsureComponentsExist();
		EnsureSecuritySchemesExist();
		if (securityScheme == null || referenceId == null)
		{
			return;
		}
		IDictionary<string, IOpenApiSecurityScheme>? securitySchemes = Components.SecuritySchemes;
		if (securitySchemes == null || !securitySchemes.ContainsKey(referenceId))
		{
			OpenApiComponents components = Components;
			if (components.SecuritySchemes == null)
			{
				IDictionary<string, IOpenApiSecurityScheme> dictionary = (components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>());
			}
			Components.SecuritySchemes.Add(referenceId, securityScheme);
		}
	}

	/// <inheritdoc />
	public override void Visit(IOpenApiSchema schema)
	{
		if (schema is OpenApiSchemaReference openApiSchemaReference)
		{
			AddSchemaToComponents(openApiSchemaReference.Target, openApiSchemaReference.Reference?.Id);
		}
		base.Visit(schema);
	}

	private void EnsureComponentsExist()
	{
		OpenApiDocument target = target;
		if (target.Components == null)
		{
			OpenApiComponents openApiComponents = (target.Components = new OpenApiComponents());
		}
	}

	private void EnsureSchemasExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.Schemas == null)
			{
				IDictionary<string, IOpenApiSchema> dictionary = (components.Schemas = new Dictionary<string, IOpenApiSchema>());
			}
		}
	}

	private void EnsureParametersExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.Parameters == null)
			{
				IDictionary<string, IOpenApiParameter> dictionary = (components.Parameters = new Dictionary<string, IOpenApiParameter>());
			}
		}
	}

	private void EnsureResponsesExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.Responses == null)
			{
				IDictionary<string, IOpenApiResponse> dictionary = (components.Responses = new Dictionary<string, IOpenApiResponse>());
			}
		}
	}

	private void EnsureRequestBodiesExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.RequestBodies == null)
			{
				IDictionary<string, IOpenApiRequestBody> dictionary = (components.RequestBodies = new Dictionary<string, IOpenApiRequestBody>());
			}
		}
	}

	private void EnsureExamplesExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.Examples == null)
			{
				IDictionary<string, IOpenApiExample> dictionary = (components.Examples = new Dictionary<string, IOpenApiExample>());
			}
		}
	}

	private void EnsureHeadersExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.Headers == null)
			{
				IDictionary<string, IOpenApiHeader> dictionary = (components.Headers = new Dictionary<string, IOpenApiHeader>());
			}
		}
	}

	private void EnsureCallbacksExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.Callbacks == null)
			{
				IDictionary<string, IOpenApiCallback> dictionary = (components.Callbacks = new Dictionary<string, IOpenApiCallback>());
			}
		}
	}

	private void EnsureLinksExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.Links == null)
			{
				IDictionary<string, IOpenApiLink> dictionary = (components.Links = new Dictionary<string, IOpenApiLink>());
			}
		}
	}

	private void EnsureSecuritySchemesExist()
	{
		if (target.Components != null)
		{
			OpenApiComponents components = target.Components;
			if (components.SecuritySchemes == null)
			{
				IDictionary<string, IOpenApiSecurityScheme> dictionary = (components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>());
			}
		}
	}

	private void EnsurePathItemsExist()
	{
		if (target.Components != null)
		{
			target.Components.PathItems = new Dictionary<string, IOpenApiPathItem>();
		}
	}
}
