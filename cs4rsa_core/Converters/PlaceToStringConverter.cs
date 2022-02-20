using SubjectCrawlService1.DataTypes.Enums;
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
            return place switch
            {
                Place.QUANGTRUNG => "Quang Trung",
                Place.PHANTHANH => "Phan Thanh",
                Place.HOAKHANH => "Hoà Khánh",
                Place.NVL_137 => "137 Nguyễn Văn Linh",
                Place.VIETTIN => "334/4 Nguyễn Văn Linh",
                _ => "254 Nguyễn Văn Linh",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
