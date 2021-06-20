using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Models;
using cs4rsa.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa.Converters
{
    /// <summary>
    /// Chuyển một Conflict Model thành một chuỗi thông tin hiển thị.
    /// </summary>
    class ConflictModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IConflictModel conflict = value as IConflictModel;
            return conflict.GetConflictInfo();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
