using System;
using System.Globalization;
using System.Windows.Data;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.WPF.Converter
{
    public class PhaseCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            Phase phase = (Phase)value;
            switch (phase)
            {
                case Phase.First: return "Giai đoạn một";
                case Phase.Second: return "Giai đoạn hai";
                case Phase.All: return "Hai giai đoạn";
                default: return "Chưa thể xác định";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
