using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.OpenApi.YamlReader.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class SRResource
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Microsoft.OpenApi.YamlReader.Properties.SRResource", typeof(SRResource).Assembly);
			}
			return resourceMan;
		}
	}

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

	internal static string ArgumentNullOrWhiteSpace => ResourceManager.GetString("ArgumentNullOrWhiteSpace", resourceCulture);

	internal static string CannotResolveRemoteReferencesSynchronously => ResourceManager.GetString("CannotResolveRemoteReferencesSynchronously", resourceCulture);

	internal static string JsonPointerCannotBeResolved => ResourceManager.GetString("JsonPointerCannotBeResolved", resourceCulture);

	internal static string LoadReferencedObjectFromExternalNotImplmented => ResourceManager.GetString("LoadReferencedObjectFromExternalNotImplmented", resourceCulture);

	internal static string ReferenceHasInvalidFormat => ResourceManager.GetString("ReferenceHasInvalidFormat", resourceCulture);

	internal static string ReferenceV2HasInvalidValue => ResourceManager.GetString("ReferenceV2HasInvalidValue", resourceCulture);

	internal static string ReferenceV3HasInvalidValue => ResourceManager.GetString("ReferenceV3HasInvalidValue", resourceCulture);

	internal SRResource()
	{
	}
}
