using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isInvert = (parameter != null && parameter.GetType() == typeof(bool)) ? (bool)parameter : false;
            if (isInvert)
            {

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
