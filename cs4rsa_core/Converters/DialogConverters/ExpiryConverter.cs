using CourseSearchService.Crawlers;
using CourseSearchService.Crawlers.Interfaces;
using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Đây là nơi chưa tất cả các Converter cho các View của Dialog.
/// </summary>
namespace cs4rsa_core.Converters.DialogConverters
{
    /// <summary>
    /// Converter này xác định một phiên còn hợp lệ hay không dựa theo
    /// year value và semester value của chúng.
    /// </summary>
    public class ExpiryConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string semester = values[0] as string;
            string year = values[1] as string;
            ICourseCrawler courseCrawler = new CourseCrawler();
            return semester.Equals(courseCrawler.GetCurrentSemesterValue(), StringComparison.Ordinal) && year.Equals(courseCrawler.GetCurrentYearValue(), StringComparison.Ordinal) ? "1" : "0";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
