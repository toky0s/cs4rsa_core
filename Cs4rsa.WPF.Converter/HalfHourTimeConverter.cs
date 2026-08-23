using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    public class HalfHourTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int unit = System.Convert.ToInt32(value);

            // Mỗi đơn vị = 30 phút
            int totalMinutes = unit * 30;

            // Bắt đầu từ 7:00
            int startHour = 7;
            int hour = startHour + (totalMinutes / 60);
            int minute = totalMinutes % 60;

            return $"{hour:D2}:{minute:D2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
