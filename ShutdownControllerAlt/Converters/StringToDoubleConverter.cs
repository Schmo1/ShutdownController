using ShutdownController.Utility;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ShutdownController.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string newStr = value.ToString().Replace(',', '.');

            Regex regex = new Regex(Properties.ConstTemplates.RegexToFindDouble);
            
            //filter the right value 1.244.55 => 1.244
            if (regex.Matches(newStr).Count == 0)
                return 1;

            MatchCollection collection = regex.Matches(newStr);

            string str = collection[0].Groups[0].Value;

            try
            {
                return double.Parse(str, new CultureInfo("en-US"));
            
            }catch (Exception e)
            {
                MyLogger.Instance().Error("Error on double convert. Exception: " + e.Message);
            }

            return 1;
        }
    }
}
