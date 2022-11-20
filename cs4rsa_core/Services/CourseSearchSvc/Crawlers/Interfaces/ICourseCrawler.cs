namespace Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces
{
    public interface ICourseCrawler
    {
        string GetCurrentSemesterValue();
        string GetCurrentSemesterInfo();
        string GetCurrentYearValue();
        string GetCurrentYearInfo();
    }
}
