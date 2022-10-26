using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;

namespace cs4rsa_core.ModelExtensions
{
    public class SessionExtension
    {
        private readonly ICourseCrawler _courseCrawler;
        public SessionExtension(ICourseCrawler courseCrawler)
        {
            _courseCrawler = courseCrawler;
        }

        public bool IsValid(Session session)
        {
            return session.SemesterValue.Equals(_courseCrawler.GetCurrentSemesterValue())
                && session.YearValue.Equals(_courseCrawler.GetCurrentYearValue());
        }
    }
}
