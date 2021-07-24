using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cs4rsa.Converters
{
    class PlaceToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Place place =(Place)value;
            switch (place)
            {
                case Place.QUANGTRUNG:
                    return "Quang Trung";
                case Place.PHANTHANH:
                    return "Phan Thanh";
                case Place.HOAKHANH:
                    return "Hoà Khánh";
                case Place.NVL_137:
                    return "137 Nguyễn Văn Linh";
                case Place.VIETTIN:
                    return "334/4 Nguyễn Văn Linh";
                default:
                    return "254 Nguyễn Văn Linh";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
