using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal static class OpenApiAnyConverter
{
	public static IOpenApiAny GetSpecificOpenApiAny(IOpenApiAny openApiAny, OpenApiSchema schema = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Expected O, but got Unknown
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Expected O, but got Unknown
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Expected O, but got Unknown
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Expected O, but got Unknown
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Expected O, but got Unknown
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Expected O, but got Unknown
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Expected O, but got Unknown
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Expected O, but got Unknown
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Expected O, but got Unknown
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Expected O, but got Unknown
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Expected O, but got Unknown
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Expected O, but got Unknown
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Expected O, but got Unknown
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Expected O, but got Unknown
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Expected O, but got Unknown
		OpenApiArray val = (OpenApiArray)(object)((openApiAny is OpenApiArray) ? openApiAny : null);
		if (val != null)
		{
			OpenApiArray val2 = new OpenApiArray();
			{
				foreach (IOpenApiAny item in (List<IOpenApiAny>)(object)val)
				{
					((List<IOpenApiAny>)(object)val2).Add(GetSpecificOpenApiAny(item, (schema != null) ? schema.Items : null));
				}
				return (IOpenApiAny)(object)val2;
			}
		}
		OpenApiObject val3 = (OpenApiObject)(object)((openApiAny is OpenApiObject) ? openApiAny : null);
		if (val3 != null)
		{
			OpenApiObject val4 = new OpenApiObject();
			{
				foreach (string item2 in ((Dictionary<string, IOpenApiAny>)(object)val3).Keys.ToList())
				{
					if (((schema != null) ? schema.Properties : null) != null && schema.Properties.TryGetValue(item2, out var value))
					{
						((Dictionary<string, IOpenApiAny>)(object)val4)[item2] = GetSpecificOpenApiAny(((Dictionary<string, IOpenApiAny>)(object)val3)[item2], value);
					}
					else
					{
						((Dictionary<string, IOpenApiAny>)(object)val4)[item2] = GetSpecificOpenApiAny(((Dictionary<string, IOpenApiAny>)(object)val3)[item2], (schema != null) ? schema.AdditionalProperties : null);
					}
				}
				return (IOpenApiAny)(object)val4;
			}
		}
		OpenApiString val5 = (OpenApiString)(object)((openApiAny is OpenApiString) ? openApiAny : null);
		if (val5 == null)
		{
			return openApiAny;
		}
		string value2 = ((OpenApiPrimitive<string>)(object)val5).Value;
		string text = ((schema != null) ? schema.Type : null);
		string text2 = ((schema != null) ? schema.Format : null);
		if (val5.IsExplicit())
		{
			if (schema == null)
			{
				if (DateTimeOffset.TryParse(value2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
				{
					if (!(result.TimeOfDay == TimeSpan.Zero))
					{
						return (IOpenApiAny)new OpenApiDateTime(result);
					}
					return (IOpenApiAny)new OpenApiDate(result.Date);
				}
			}
			else if (text == "string")
			{
				if (text2 == "byte")
				{
					try
					{
						return (IOpenApiAny)new OpenApiByte(Convert.FromBase64String(value2));
					}
					catch (FormatException)
					{
					}
				}
				if (text2 == "binary")
				{
					try
					{
						return (IOpenApiAny)new OpenApiBinary(Encoding.UTF8.GetBytes(value2));
					}
					catch (EncoderFallbackException)
					{
					}
				}
				if (text2 == "date" && DateTimeOffset.TryParse(value2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result2))
				{
					return (IOpenApiAny)new OpenApiDate(result2.Date);
				}
				if (text2 == "date-time" && DateTimeOffset.TryParse(value2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result3))
				{
					return (IOpenApiAny)new OpenApiDateTime(result3);
				}
				if (text2 == "password")
				{
					return (IOpenApiAny)new OpenApiPassword(value2);
				}
			}
			return (IOpenApiAny)(object)val5;
		}
		if ((value2 == null || value2 == "null") ? true : false)
		{
			return (IOpenApiAny)new OpenApiNull();
		}
		if (schema == null || schema.Type == null)
		{
			if (value2 == "true")
			{
				return (IOpenApiAny)new OpenApiBoolean(true);
			}
			if (value2 == "false")
			{
				return (IOpenApiAny)new OpenApiBoolean(false);
			}
			if (int.TryParse(value2, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result4))
			{
				return (IOpenApiAny)new OpenApiInteger(result4);
			}
			if (long.TryParse(value2, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result5))
			{
				return (IOpenApiAny)new OpenApiLong(result5);
			}
			if (double.TryParse(value2, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result6))
			{
				return (IOpenApiAny)new OpenApiDouble(result6);
			}
			if (DateTimeOffset.TryParse(value2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result7))
			{
				return (IOpenApiAny)new OpenApiDateTime(result7);
			}
		}
		else
		{
			if (text == "integer" && text2 == "int32" && int.TryParse(value2, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result8))
			{
				return (IOpenApiAny)new OpenApiInteger(result8);
			}
			if (text == "integer" && text2 == "int64" && long.TryParse(value2, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result9))
			{
				return (IOpenApiAny)new OpenApiLong(result9);
			}
			if (text == "integer" && int.TryParse(value2, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result10))
			{
				return (IOpenApiAny)new OpenApiInteger(result10);
			}
			if (text == "number" && text2 == "float" && float.TryParse(value2, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result11))
			{
				return (IOpenApiAny)new OpenApiFloat(result11);
			}
			if (text == "number" && text2 == "double" && double.TryParse(value2, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result12))
			{
				return (IOpenApiAny)new OpenApiDouble(result12);
			}
			if (text == "number" && double.TryParse(value2, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result13))
			{
				return (IOpenApiAny)new OpenApiDouble(result13);
			}
			if (text == "string" && text2 == "byte")
			{
				try
				{
					return (IOpenApiAny)new OpenApiByte(Convert.FromBase64String(value2));
				}
				catch (FormatException)
				{
				}
			}
			if (text == "string" && text2 == "binary")
			{
				try
				{
					return (IOpenApiAny)new OpenApiBinary(Encoding.UTF8.GetBytes(value2));
				}
				catch (EncoderFallbackException)
				{
				}
			}
			if (text == "string" && text2 == "date" && DateTimeOffset.TryParse(value2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result14))
			{
				return (IOpenApiAny)new OpenApiDate(result14.Date);
			}
			if (text == "string" && text2 == "date-time" && DateTimeOffset.TryParse(value2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result15))
			{
				return (IOpenApiAny)new OpenApiDateTime(result15);
			}
			if (text == "string" && text2 == "password")
			{
				return (IOpenApiAny)new OpenApiPassword(value2);
			}
			if (text == "string")
			{
				return (IOpenApiAny)(object)val5;
			}
			if (text == "boolean" && bool.TryParse(value2, out var result16))
			{
				return (IOpenApiAny)new OpenApiBoolean(result16);
			}
		}
		return (IOpenApiAny)(object)val5;
	}
}
