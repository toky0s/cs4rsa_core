using Cs4rsa.Database.Models;
using Cs4rsa.Service.CourseCrawler.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Utils
{
    internal class ValidateUserSchedule
    {
        public static bool CheckIsAvailableSession(ICourseCrawler courseCrawler, UserSchedule session)
        {
            courseCrawler.GetInfo(out var _, out var yearValue, out var _, out var semesterValue);
            if (session != null)
            {
                var semester = session.SemesterValue;
                var year = session.YearValue;
                return semester.Equals(semesterValue, StringComparison.Ordinal) && year.Equals(yearValue, StringComparison.Ordinal);
            }
            else
            {
                return false;
            }
        }
    }
}
