using System;
using System.Globalization;
using System.Windows.Data;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.WPF.Converter
{
    public class PlaceCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            Place place = (Place)value;
            return place.ToActualPlace();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
