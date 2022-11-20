using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;

namespace Cs4rsa.ModelExtensions
{
    public class SessionExtension
    {
        private readonly ICourseCrawler _courseCrawler;
        public SessionExtension(ICourseCrawler courseCrawler)
        {
            _courseCrawler = courseCrawler;
        }

        public bool IsValid(UserSchedule session)
        {
            return session.SemesterValue.Equals(_courseCrawler.GetCurrentSemesterValue())
                && session.YearValue.Equals(_courseCrawler.GetCurrentYearValue());
        }
    }
}
