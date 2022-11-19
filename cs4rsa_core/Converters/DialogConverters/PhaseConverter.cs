using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters.DialogConverters
{
    internal class PhaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Phase.First => "GD 1",
                Phase.Second => "GD 2",
                Phase.All => "GD 1 & 2",
                _ => "Không xác định"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
