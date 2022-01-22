using cs4rsa_core.Models;
using System;
using System.Globalization;
using System.Windows.Data;


/// <summary>
/// Namespace này chứa các converter cho phần giao diện.
/// </summary>
namespace cs4rsa_core.Converters
{
    public class ClassGroupModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TimeBlock classGroupModel ? classGroupModel.Text : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
