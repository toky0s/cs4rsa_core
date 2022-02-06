using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters.Controls
{
    internal class TopRangeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double timelineCanvasHeight = (double)values[0];
            double range = timelineCanvasHeight / 15;
            int lineIndex = (int)values[1];
            double result = lineIndex * range;
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
