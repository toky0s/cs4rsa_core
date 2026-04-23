using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Visibility value.
        /// True maps to Visibility.Visible, and False maps to Visibility.Collapsed.
        /// If the parameter is a boolean and is true, the visibility will be inverted (True maps to Collapsed, False maps to Visible).
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <param name="parameter">A boolean value indicating whether to invert the visibility.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A Visibility value based on the boolean input.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value?.ToString(), out bool b))
            {
                if (bool.TryParse(parameter?.ToString(), out bool invert) && invert)
                {
                    b = !b;
                }
                return b ? Visibility.Visible : Visibility.Collapsed;
            }

            // Fix: check null trực tiếp thay vì dùng pattern matching với object
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }

            return false;
        }
    }
}
