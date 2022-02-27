using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters
{
    class EmptySeatCheckerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            short emptySeat = (short)value;
            short compareToValue = short.Parse((string)parameter);
            return emptySeat <= compareToValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
