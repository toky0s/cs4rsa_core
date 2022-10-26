using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters
{
    class PlaceToStringConverter : IValueConverter
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
