using System;
using System.Globalization;
using System.Windows.Data;

namespace ShutdownController.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            if (strValue.Length == 1)
                strValue = "0" + strValue;

            return strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt32(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
