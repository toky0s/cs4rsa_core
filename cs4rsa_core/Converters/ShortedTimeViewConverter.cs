using HelperService;
using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters
{
    public class ShortedTimeViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ShortedTime shortedTime = value as ShortedTime;
            return shortedTime.NewTime.ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
