using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    public class IsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !ReferenceEquals(value, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
