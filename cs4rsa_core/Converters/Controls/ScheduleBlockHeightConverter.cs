using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters.Controls
{
    internal class ScheduleBlockHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double douHeight = (double)values[0];
            int intStartIndex = (int)values[1];
            int intEndIndex = (int)values[2];
            return douHeight / 16 * (intEndIndex - intStartIndex);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
