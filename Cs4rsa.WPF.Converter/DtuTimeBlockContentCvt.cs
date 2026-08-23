using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.WPF.Converter
{
    public class DtuTimeBlockContentCvt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // DMS 396 T | Đồ Án CDIO | P. 405, Hòa Khánh Nam - Tòa Nhà E | 07:00 - 11:15
            if (value == null) return null;
            if (value is SchoolClassUnit @schoolClassUnit)
            {
                var sb = new StringBuilder();
                sb.Append(schoolClassUnit.ClassName)
                .Append(" | ")
                .Append(schoolClassUnit.SchoolClass.SubjectName)
                .Append(" | P.")
                .Append(schoolClassUnit.Room.Name)
                .Append(", ")
                .Append(schoolClassUnit.Room.Place.ToActualPlace())
                .Append(" | ")
                .Append(schoolClassUnit.Start.ToString("HH:mm"))
                .Append(" - ")
                .Append(schoolClassUnit.End.ToString("HH:mm"));
                return sb.ToString();
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
