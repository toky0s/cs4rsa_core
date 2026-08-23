using System;
using System.Globalization;
using System.Windows.Data;
using Cs4rsa.Service.Conflict.DataTypes.Enums;

namespace Cs4rsa.WPF.Converter
{
    /// <summary>
    /// Converter này chuyển một ConflictType thành số nguyên, phục vụ hiển thị giao diện với trigger.
    /// </summary>
    public class ConflictTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConflictType conflictType = (ConflictType)value;
            if (conflictType == ConflictType.Time)
                return 0;
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
