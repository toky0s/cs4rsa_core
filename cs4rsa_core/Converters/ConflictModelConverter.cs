using Cs4rsa.Services.ConflictSvc.Interfaces;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
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
                IConflictModel conflict = value as IConflictModel;
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
