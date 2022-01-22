using cs4rsa_core.Models.Bases;
using System;
using System.Globalization;
using System.Windows.Data;

namespace cs4rsa_core.Converters
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
