using System;
using System.Globalization;

namespace Microsoft.OpenApi.Readers.ParseNodes;

internal class ParserHelper
{
	public static decimal ParseDecimalWithFallbackOnOverflow(string value, decimal defaultValue)
	{
		try
		{
			return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
		}
		catch (OverflowException)
		{
			return defaultValue;
		}
	}
}
