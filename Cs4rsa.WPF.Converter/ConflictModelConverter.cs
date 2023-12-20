using System;
using System.Globalization;
using System.Windows.Data;
using Cs4rsa.UI.ScheduleTable.Interfaces;

namespace Cs4rsa.WPF.Converter
{
    /// <summary>
    /// Chuyển một Conflict Model thành một chuỗi thông tin hiển thị.
    /// </summary>
    public class ConflictModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                IConflictModel conflict = (IConflictModel)value;
                return conflict.GetConflictInfo();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
