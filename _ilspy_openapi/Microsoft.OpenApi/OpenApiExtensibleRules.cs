using System;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// The validation rules for <see cref="T:Microsoft.OpenApi.IOpenApiExtensible" />.
/// </summary>
[OpenApiRule]
public static class OpenApiExtensibleRules
{
	/// <summary>
	/// Extension name MUST start with "x-".
	/// </summary>
	public static ValidationRule<IOpenApiExtensible> ExtensionNameMustStartWithXDash => new ValidationRule<IOpenApiExtensible>("ExtensionNameMustStartWithXDash", delegate(IValidationContext context, IOpenApiExtensible item)
	{
		if (item.Extensions != null)
		{
			context.Enter("extensions");
			foreach (string item in item.Extensions.Keys.Where((string x) => !x.StartsWith("x-", StringComparison.OrdinalIgnoreCase)))
			{
				context.CreateError("ExtensionNameMustStartWithXDash", string.Format(SRResource.Validation_ExtensionNameMustBeginWithXDash, item, context.PathString));
			}
			context.Exit();
		}
	});
}
