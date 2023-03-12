using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    /// <summary>
    /// Converter thực hiện thay thế chuỗi trống 
    /// bằng một template do người dùng truyền vào.
    /// </summary>
    public class EmptyStringPlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return (string)parameter;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
