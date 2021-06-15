using cs4rsa.Crawler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

/// <summary>
/// Đây là nơi chưa tất cả các Converter cho các View của Dialog.
/// </summary>
namespace cs4rsa.Converters.DialogConverters
{
    class ExpiryConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string semester = values[0] as string;
            string year = values[1] as string;
            HomeCourseSearch hcs = HomeCourseSearch.GetInstance();
            if (semester.Equals(hcs.CurrentSemesterValue) && year.Equals(hcs.CurrentYearValue))
                return "1";
            return "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
