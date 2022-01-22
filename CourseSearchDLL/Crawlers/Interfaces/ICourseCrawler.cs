using CourseSearchDLL.DataTypes;
using System.Collections.Generic;

namespace CourseSearchDLL.Crawlers.Interfaces
{
    public interface ICourseCrawler
    {
        string GetCurrentSemesterValue();
        string GetCurrentSemesterInfo();
        string GetCurrentYearValue();
        string GetCurrentYearInfo();

        List<CourseYear> GetCourseYears();
    }
}
