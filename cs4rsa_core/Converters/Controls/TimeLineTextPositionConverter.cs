using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters.Controls
{
    public class TimeLineTextPositionConverter : IValueConverter
    {
        public static readonly int INT_TIMELINES_COUNT = 15;
        public static readonly int INT_CURRENT_TIMELINE_FONT_SIZE = 10;

        // Khoảng cách tính từ bottom của text với line ngay bên dưới
        public static readonly int INT_EXTRA_SPACE = 7;
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double douCanvasHeight = (double)value;
            int intTimelineIndex = int.Parse((string)parameter);
            double douRange = douCanvasHeight / INT_TIMELINES_COUNT;
            double douPosition = douRange * intTimelineIndex - (INT_CURRENT_TIMELINE_FONT_SIZE + INT_EXTRA_SPACE);
            return douPosition;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
