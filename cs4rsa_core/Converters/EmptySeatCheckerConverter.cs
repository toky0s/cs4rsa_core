using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    class EmptySeatCheckerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int emptySeat = (int)value;
            int compareToValue = int.Parse((string)parameter);
            return emptySeat <= compareToValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
