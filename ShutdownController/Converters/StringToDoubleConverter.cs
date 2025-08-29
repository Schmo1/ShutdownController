
using Serilog;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ShutdownController.Converters;

public partial class StringToDoubleConverter : IValueConverter
{

    [GeneratedRegex("(\\d+(\\.\\d*)?)|(-?\\.\\d+)", RegexOptions.Compiled)]
	private static partial Regex DoubleRegex();

	public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.ToString();
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? valueStr = value.ToString();

        if(string.IsNullOrEmpty(valueStr))
        {
            return null;
		}

		if (DoubleRegex().Count(valueStr) == 0)
        {
            return 1;
		}

		MatchCollection collection = DoubleRegex().Matches(valueStr);
        
        string str = collection[0].Groups[0].Value;

        try
        {
            return double.Parse(str, CultureInfo.CurrentCulture);
        
        }catch (Exception e)
        {
            Log.Logger.Error(e, "Error on double convert. Exception: {Message}", e.Message);
		}

        return 1;
    }
}
