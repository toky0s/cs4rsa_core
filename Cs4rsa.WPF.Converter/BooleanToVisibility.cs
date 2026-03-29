using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cs4rsa.WPF.Converter
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /**
             * Nếu là bool thì
             * True: Hiện
             * False: Ẩn
             */
            if (value is bool b)
            {
                return b ? Visibility.Visible : Visibility.Collapsed;
            }

            /**
             * Nếu là object thì
             * Khác null: Hiện
             * Bằng null: Ẩn
             */
            if (value is object obj)
            {
                return obj != null ? Visibility.Visible : Visibility.Collapsed;
            }

            // Trường hợp khác: Luôn ẩn
            return Visibility.Collapsed;
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
