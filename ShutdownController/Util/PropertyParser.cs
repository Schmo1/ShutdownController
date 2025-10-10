using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShutdownController.Util;

public static class PropertyParser
{
	public static int GetIntProperty(string key, int defaultValue = default)
	{
		var value = App.Current.Properties[key];
		if (int.TryParse(value?.ToString(), out int intValue))
		{
			return intValue;
		}
		else
		{
			App.Current.Properties[key] = defaultValue;
			return defaultValue;
		}
	}

	public static bool GetBoolProperty(string key, bool defaultValue = default)
	{
		var value = App.Current.Properties[key];
		if (bool.TryParse(value?.ToString(), out bool intValue))
		{
			return intValue;
		}
		else
		{
			App.Current.Properties[key] = defaultValue;
			return defaultValue;
		}
	}
}
