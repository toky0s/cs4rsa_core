using System;
using System.Globalization;
using System.Windows.Data;
using Cs4rsa.Service.SubjectCrawler.Utils;

namespace Cs4rsa.WPF.Converter
{
    class DayCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            DayOfWeek dayOfWeek = (DayOfWeek)value;
            return dayOfWeek.ToCs4rsaVietnamese();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
