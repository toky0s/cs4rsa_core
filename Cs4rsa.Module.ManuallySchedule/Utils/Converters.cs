using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Utils
{
    internal static class Converters
    {
        public static Lesson ConvertToLesson(this SchoolClass schoolClass)
        {
            return new Lesson(
                schoolClass.StudyWeek,
                schoolClass.Schedule,
                schoolClass.DayPlaceMetaData,
                schoolClass.Metadata,
                schoolClass.CurrentPhase,
                schoolClass.SchoolClassName,
                schoolClass.ClassGroupName,
                schoolClass.Subject.SubjectCode,
                schoolClass.SubjectName
            );
        }
    }
}
