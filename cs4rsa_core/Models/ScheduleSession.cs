using System;
using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;

namespace cs4rsa_core.Models
{
    /// <summary>
    /// Class này đại diện cho một Session được lưu trong database.
    /// </summary>
    public class ScheduleSession
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public string Semester { get; set; }
        public string Year { get; set; }

        private ICourseCrawler _courseCrawler;

        public ScheduleSession(ICourseCrawler courseCrawler)
        {
            _courseCrawler = courseCrawler;
        }

        public bool IsValid()
        {
            return Semester.Equals(_courseCrawler.GetCurrentSemesterValue(), StringComparison.Ordinal)
                && Year.Equals(_courseCrawler.GetCurrentYearValue(), StringComparison.Ordinal);
        }
    }
}
