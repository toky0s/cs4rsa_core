using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    public class StudyStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StudyState studyStateNeedCheck = (StudyState)value;
            StudyState studyStateCheckWith = (StudyState)Enum.Parse(typeof(StudyState), parameter.ToString());
            return studyStateCheckWith == studyStateNeedCheck;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
