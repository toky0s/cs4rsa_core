using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;

namespace Cs4rsa.ModelExtensions
{
    public class SessionExtension
    {
        private readonly CourseCrawler _courseCrawler;
        public SessionExtension(CourseCrawler courseCrawler)
        {
            _courseCrawler = courseCrawler;
        }

        public bool IsValid(UserSchedule session)
        {
            return session.SemesterValue.Equals(_courseCrawler.CurrentSemesterValue)
                && session.YearValue.Equals(_courseCrawler.CurrentYearValue);
        }
    }
}
