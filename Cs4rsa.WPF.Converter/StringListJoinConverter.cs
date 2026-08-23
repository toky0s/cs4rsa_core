using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    public class StringListJoinConverter : IValueConverter
    {
        // Convert: nối list chuỗi thành một chuỗi duy nhất
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<string> list = value as IEnumerable<string>;
            if (list == null || !list.Any())
                return null;

            // Nếu có tham số, dùng làm separator, mặc định là ", "
            string separator = parameter as string ?? ", ";

            return string.Join(separator, list);
        }

        // ConvertBack: có thể tách chuỗi thành list nếu cần
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            if (input == null)
                return null;

            string separator = parameter as string ?? ", ";
            return input.Split(new[] { separator }, StringSplitOptions.None).ToList();
        }
    }
}
