using Cs4rsa.Services.TeacherCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Cs4rsa.Converters
{
    public class GetTeacherNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return parameter;
            List<TeacherModel> teachers = (List<TeacherModel>)value;
            return teachers.Count == 0
                    ? parameter // (1)
                    : teachers.Count == 1
                        ? teachers[0].Name // (2)
                        : string.Join(", ", teachers); // (3)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
