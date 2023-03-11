using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    public class PlaceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Place place = (Place)value;
            return place.ToActualPlace();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
