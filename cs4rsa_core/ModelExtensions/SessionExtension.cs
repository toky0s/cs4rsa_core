using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.ModelExtensions
{
    public class SessionExtension
    {
        private ICourseCrawler _courseCrawler;
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
