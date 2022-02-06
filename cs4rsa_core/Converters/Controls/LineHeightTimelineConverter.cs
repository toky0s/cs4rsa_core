using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters.Controls
{
    public class LineHeightTimelineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double timelineCanvasHeight = (double)value;
            double range = timelineCanvasHeight / 15;
            int lineIndex = int.Parse((string)parameter);
            double result = lineIndex * range;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
