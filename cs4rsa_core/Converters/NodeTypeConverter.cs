using Cs4rsa.Models.Bases;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    public class NodeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TreeItem treeItem = value as TreeItem;
            return treeItem.NodeType.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
