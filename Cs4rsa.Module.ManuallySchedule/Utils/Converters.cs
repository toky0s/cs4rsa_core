using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Utils
{
    internal static class Converters
    {
        public static Lesson ConvertToLesson(this SchoolClassModel schoolClassModel)
        {
            return new Lesson(
                schoolClassModel.StudyWeek,
                schoolClassModel.Schedule,
                schoolClassModel.DayPlaceMetaData,
                schoolClassModel.SchoolClass.Metadata,
                schoolClassModel.Phase,
                schoolClassModel.SchoolClassName,
                schoolClassModel.SchoolClass.ClassGroupName,
                schoolClassModel.SubjectCode
            );
        }
    }
}
