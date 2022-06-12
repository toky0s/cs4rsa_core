using ConflictService.DataTypes.Enums;

using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters
{
    /// <summary>
    /// Converter này chuyển một ConflictType thành số nguyên, phục vụ hiển thị giao diện với trigger.
    /// </summary>
    class ConflictTypeConverter : IValueConverter
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
