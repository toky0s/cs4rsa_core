using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    class PhaseCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            Phase phase = (Phase)value;
            return phase switch
            {
                Phase.First => "Giai đoạn một",
                Phase.Second => "Giai đoạn hai",
                Phase.All => "Hai giai đoạn",
                _ => "Chưa thể xác định"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
