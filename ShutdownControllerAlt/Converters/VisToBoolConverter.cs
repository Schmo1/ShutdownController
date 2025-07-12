using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ShutdownController.Converters
{
    public class VisToBool : IValueConverter

    {

        private bool _inverted = false;

        public bool Inverted 
        { 
            get { return _inverted; }  
            set { _inverted = value; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)value)
                return Inverted ? Visibility.Collapsed : Visibility.Visible;
            else
                return Inverted ? Visibility.Visible : Visibility.Collapsed;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible ? !Inverted : Inverted;
        }

    }
}
