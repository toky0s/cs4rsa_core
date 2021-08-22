using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cs4rsa.Converters
{
    class StudyStateConverter : IValueConverter
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
