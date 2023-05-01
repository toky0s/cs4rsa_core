using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    /// <summary>
    /// Kiểm tra Type của một object.
    /// </summary>
    internal class IsTypeOfCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            string types = (string)parameter;
            IEnumerable<string> result = types.Split('|').Select(x => x.Trim());
            return result.Contains(value.GetType().Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
