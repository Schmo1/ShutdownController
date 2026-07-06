using System.Globalization;
using System.Windows.Data;

namespace ShutdownController.Converters;

/// <summary>
/// Maps an enum value to a bool by comparing it with the converter parameter.
/// Used to bind a set of RadioButtons (segmented control) to a single enum property.
/// </summary>
public sealed class EnumToBooleanConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is null || parameter is null)
		{
			return false;
		}

		return string.Equals(value.ToString(), parameter.ToString(), StringComparison.OrdinalIgnoreCase);
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		// Only the checked radio button writes back; the unchecked ones are ignored.
		if (value is true && parameter is not null && Enum.TryParse(targetType, parameter.ToString(), out object? result))
		{
			return result!;
		}

		return Binding.DoNothing;
	}
}
