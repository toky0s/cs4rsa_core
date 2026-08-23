using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    /// <summary>
    /// Kiểm tra Type của một object.
    /// </summary>
    internal class IsTypeOfCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            var types = (string)parameter;
            var result = types?.Split('|').Select(x => x.Trim());
            return result?.Contains(value.GetType().Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
