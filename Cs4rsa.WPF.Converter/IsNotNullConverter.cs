using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string sValue)
            {
                return !string.IsNullOrWhiteSpace(sValue);
            }
            return value is object;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
