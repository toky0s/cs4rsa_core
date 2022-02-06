using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters.Controls
{
    internal class ScheduleBlockWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double douCanvasWidth = (double)value;
            return douCanvasWidth - 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
