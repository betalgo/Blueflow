using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi;

/// <summary>
/// Security Requirement Object.
/// Each name MUST correspond to a security scheme which is declared in
/// the Security Schemes under the Components Object.
/// If the security scheme is of type "oauth2" or "openIdConnect",
/// then the value is a list of scope names required for the execution.
/// For other security scheme types, the array MUST be empty.
/// </summary>
public class OpenApiSecurityRequirement : Dictionary<OpenApiSecuritySchemeReference, List<string>>, IOpenApiSerializable, IOpenApiElement
{
	/// <summary>
	/// Comparer for OpenApiSecurityScheme that only considers the Id in the Reference
	/// (i.e. the string that will actually be displayed in the written document)
	/// </summary>
	private sealed class OpenApiSecuritySchemeReferenceEqualityComparer : IEqualityComparer<OpenApiSecuritySchemeReference>
	{
		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		public bool Equals(OpenApiSecuritySchemeReference? x, OpenApiSecuritySchemeReference? y)
		{
			if (x == null && y == null)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return GetHashCode(x) == GetHashCode(y);
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		public int GetHashCode(OpenApiSecuritySchemeReference obj)
		{
			if (obj == null)
			{
				return 0;
			}
			string text = obj.Reference?.Id;
			if (!string.IsNullOrEmpty(text))
			{
				return text.GetHashCode();
			}
			return 0;
		}
	}

	/// <summary>
	/// Initializes the <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" /> class.
	/// This constructor ensures that only Reference.Id is considered when two dictionary keys
	/// of type <see cref="T:Microsoft.OpenApi.OpenApiSecurityScheme" /> are compared.
	/// </summary>
	public OpenApiSecurityRequirement()
		: base((IEqualityComparer<OpenApiSecuritySchemeReference>?)new OpenApiSecuritySchemeReferenceEqualityComparer())
	{
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" /> to Open Api v3.2
	/// </summary>
	public virtual void SerializeAsV32(IOpenApiWriter writer)
	{
		SerializeInternal(writer, delegate(IOpenApiWriter w, OpenApiSecuritySchemeReference s)
		{
			if (!string.IsNullOrEmpty(s.Reference.ReferenceV3) && s.Reference.ReferenceV3 != null)
			{
				w.WritePropertyName(s.Reference.ReferenceV3);
			}
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" /> to Open Api v3.1
	/// </summary>
	public virtual void SerializeAsV31(IOpenApiWriter writer)
	{
		SerializeInternal(writer, delegate(IOpenApiWriter w, OpenApiSecuritySchemeReference s)
		{
			if (!string.IsNullOrEmpty(s.Reference.ReferenceV3) && s.Reference.ReferenceV3 != null)
			{
				w.WritePropertyName(s.Reference.ReferenceV3);
			}
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" /> to Open Api v3.0
	/// </summary>
	public virtual void SerializeAsV3(IOpenApiWriter writer)
	{
		SerializeInternal(writer, delegate(IOpenApiWriter w, OpenApiSecuritySchemeReference s)
		{
			if (!string.IsNullOrEmpty(s.Reference.ReferenceV3) && s.Reference.ReferenceV3 != null)
			{
				w.WritePropertyName(s.Reference.ReferenceV3);
			}
		});
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" /> 
	/// </summary>
	private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, OpenApiSecuritySchemeReference> callback)
	{
		Utils.CheckArgumentNull(writer, "writer");
		writer.WriteStartObject();
		foreach (KeyValuePair<OpenApiSecuritySchemeReference, List<string>> item in this.Where<KeyValuePair<OpenApiSecuritySchemeReference, List<string>>>((KeyValuePair<OpenApiSecuritySchemeReference, List<string>> p) => p.Key?.Target != null))
		{
			OpenApiSecuritySchemeReference key = item.Key;
			List<string> value = item.Value;
			callback(writer, key);
			writer.WriteStartArray();
			foreach (string item2 in value)
			{
				writer.WriteValue(item2);
			}
			writer.WriteEndArray();
		}
		writer.WriteEndObject();
	}

	/// <summary>
	/// Serialize <see cref="T:Microsoft.OpenApi.OpenApiSecurityRequirement" /> to Open Api v2.0
	/// </summary>
	public virtual void SerializeAsV2(IOpenApiWriter writer)
	{
		SerializeInternal(writer, delegate(IOpenApiWriter w, OpenApiSecuritySchemeReference s)
		{
			s.SerializeAsV2(w);
		});
	}
}
