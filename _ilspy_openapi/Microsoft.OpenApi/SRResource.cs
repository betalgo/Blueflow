using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.OpenApi;

/// <summary>
///   A strongly-typed resource class, for looking up localized strings, etc.
/// </summary>
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class SRResource
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	/// <summary>
	///   Returns the cached ResourceManager instance used by this class.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Microsoft.OpenApi.Properties.SRResource", typeof(SRResource).Assembly);
			}
			return resourceMan;
		}
	}

	/// <summary>
	///   Overrides the current thread's CurrentUICulture property for all
	///   resource lookups using this strongly typed resource class.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	/// <summary>
	///   Looks up a localized string similar to There must be an active scope for name '{0}' to be written..
	/// </summary>
	internal static string ActiveScopeNeededForPropertyNameWriting => ResourceManager.GetString("ActiveScopeNeededForPropertyNameWriting", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The argument '{0}' is null..
	/// </summary>
	internal static string ArgumentNull => ResourceManager.GetString("ArgumentNull", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The argument '{0}' is null, empty or consists only of white-space..
	/// </summary>
	internal static string ArgumentNullOrWhiteSpace => ResourceManager.GetString("ArgumentNullOrWhiteSpace", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to http://localhost/.
	/// </summary>
	internal static string DefaultBaseUri => ResourceManager.GetString("DefaultBaseUri", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The filed name '{0}' of extension doesn't begin with x-..
	/// </summary>
	internal static string ExtensionFieldNameMustBeginWithXDash => ResourceManager.GetString("ExtensionFieldNameMustBeginWithXDash", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Indentation level cannot be lower than 0..
	/// </summary>
	internal static string IndentationLevelInvalid => ResourceManager.GetString("IndentationLevelInvalid", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The input item should be in type of '{0}'..
	/// </summary>
	internal static string InputItemShouldBeType => ResourceManager.GetString("InputItemShouldBeType", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Invalid Reference identifier '{0}'..
	/// </summary>
	internal static string InvalidReferenceId => ResourceManager.GetString("InvalidReferenceId", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Invalid Reference Type '{0}'..
	/// </summary>
	internal static string InvalidReferenceType => ResourceManager.GetString("InvalidReferenceType", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Local reference must have type specified..
	/// </summary>
	internal static string LocalReferenceRequiresType => ResourceManager.GetString("LocalReferenceRequiresType", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The active scope must be an object scope for property name '{0}' to be written..
	/// </summary>
	internal static string ObjectScopeNeededForPropertyNameWriting => ResourceManager.GetString("ObjectScopeNeededForPropertyNameWriting", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to An error occurred while processing the Open API document..
	/// </summary>
	internal static string OpenApiExceptionGenericError => ResourceManager.GetString("OpenApiExceptionGenericError", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The given OpenAPI format '{0}' is not supported..
	/// </summary>
	internal static string OpenApiFormatNotSupported => ResourceManager.GetString("OpenApiFormatNotSupported", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The object element name '{0}' is required..
	/// </summary>
	internal static string OpenApiObjectElementIsRequired => ResourceManager.GetString("OpenApiObjectElementIsRequired", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The OpenApi element '{0}' is already marked as reference object..
	/// </summary>
	internal static string OpenApiObjectMarkAsReference => ResourceManager.GetString("OpenApiObjectMarkAsReference", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to If the parameter location is "path", this property is REQUIRED and its value MUST be true.
	/// </summary>
	internal static string OpenApiParameterRequiredPropertyMandatory => ResourceManager.GetString("OpenApiParameterRequiredPropertyMandatory", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The given OpenAPI specification version '{0}' is not supported..
	/// </summary>
	internal static string OpenApiSpecVersionNotSupported => ResourceManager.GetString("OpenApiSpecVersionNotSupported", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The type '{0}' is not supported in Open API document..
	/// </summary>
	internal static string OpenApiUnsupportedValueType => ResourceManager.GetString("OpenApiUnsupportedValueType", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to An error occurred while writing the Open API document..
	/// </summary>
	internal static string OpenApiWriterExceptionGenericError => ResourceManager.GetString("OpenApiWriterExceptionGenericError", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Invalid server variable '{0}'. A value was not provided and no default value was provided..
	/// </summary>
	internal static string ParseServerUrlDefaultValueNotAvailable => ResourceManager.GetString("ParseServerUrlDefaultValueNotAvailable", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Value '{0}' is not a valid value for variable '{1}'. If an enum is provided, it should not be empty and the value provided should exist in the enum.
	/// </summary>
	internal static string ParseServerUrlValueNotValid => ResourceManager.GetString("ParseServerUrlValueNotValid", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The given primitive type '{0}' is not supported..
	/// </summary>
	internal static string PrimitiveTypeNotSupported => ResourceManager.GetString("PrimitiveTypeNotSupported", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The reference string '{0}' has invalid format..
	/// </summary>
	internal static string ReferenceHasInvalidFormat => ResourceManager.GetString("ReferenceHasInvalidFormat", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Remote reference not supported..
	/// </summary>
	internal static string RemoteReferenceNotSupported => ResourceManager.GetString("RemoteReferenceNotSupported", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The runtime expression '{0}' has invalid format..
	/// </summary>
	internal static string RuntimeExpressionHasInvalidFormat => ResourceManager.GetString("RuntimeExpressionHasInvalidFormat", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The runtime expression '{0}' should start with '$'.
	/// </summary>
	internal static string RuntimeExpressionMustBeginWithDollar => ResourceManager.GetString("RuntimeExpressionMustBeginWithDollar", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Scope must be present to end..
	/// </summary>
	internal static string ScopeMustBePresentToEnd => ResourceManager.GetString("ScopeMustBePresentToEnd", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The scope to end is expected to be of type '{0}' but it is of type '{0}'..
	/// </summary>
	internal static string ScopeToEndHasIncorrectType => ResourceManager.GetString("ScopeToEndHasIncorrectType", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The source expression '{0}' has invalid format..
	/// </summary>
	internal static string SourceExpressionHasInvalidFormat => ResourceManager.GetString("SourceExpressionHasInvalidFormat", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Can not find visitor type registered for type '{0}'..
	/// </summary>
	internal static string UnknownVisitorType => ResourceManager.GetString("UnknownVisitorType", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The key '{0}' in '{1}' of components MUST match the regular expression '{2}'..
	/// </summary>
	internal static string Validation_ComponentsKeyMustMatchRegularExpr => ResourceManager.GetString("Validation_ComponentsKeyMustMatchRegularExpr", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The extension name '{0}' in '{1}' object MUST begin with 'x-'..
	/// </summary>
	internal static string Validation_ExtensionNameMustBeginWithXDash => ResourceManager.GetString("Validation_ExtensionNameMustBeginWithXDash", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The field '{0}' in '{1}' object is REQUIRED..
	/// </summary>
	internal static string Validation_FieldIsRequired => ResourceManager.GetString("Validation_FieldIsRequired", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The path item name '{0}' MUST begin with a slash..
	/// </summary>
	internal static string Validation_PathItemMustBeginWithSlash => ResourceManager.GetString("Validation_PathItemMustBeginWithSlash", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The path signature '{0}' MUST be unique..
	/// </summary>
	internal static string Validation_PathSignatureMustBeUnique => ResourceManager.GetString("Validation_PathSignatureMustBeUnique", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The same rule cannot be in the same rule set twice..
	/// </summary>
	internal static string Validation_RuleAddTwice => ResourceManager.GetString("Validation_RuleAddTwice", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Schema {0} property {1} is null..
	/// </summary>
	internal static string Validation_SchemaPropertyObjectRequired => ResourceManager.GetString("Validation_SchemaPropertyObjectRequired", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The schema reference '{0}' does not point to an existing schema..
	/// </summary>
	internal static string Validation_SchemaReferenceDoesNotExist => ResourceManager.GetString("Validation_SchemaReferenceDoesNotExist", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to Schema {0} must contain property specified in the discriminator {1} in the required field list..
	/// </summary>
	internal static string Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator => ResourceManager.GetString("Validation_SchemaRequiredFieldListMustContainThePropertySpecifiedInTheDiscriminator", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to The string '{0}' MUST be in the format of an email address..
	/// </summary>
	internal static string Validation_StringMustBeEmailAddress => ResourceManager.GetString("Validation_StringMustBeEmailAddress", resourceCulture);

	/// <summary>
	///   Looks up a localized string similar to OpenAPI document must be added to an OpenApiWorkspace to be able to resolve external references..
	/// </summary>
	internal static string WorkspaceRequredForExternalReferenceResolution => ResourceManager.GetString("WorkspaceRequredForExternalReferenceResolution", resourceCulture);

	internal SRResource()
	{
	}
}
