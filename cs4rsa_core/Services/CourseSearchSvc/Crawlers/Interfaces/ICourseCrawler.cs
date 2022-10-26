namespace cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces
{
    public interface ICourseCrawler
    {
        string GetCurrentSemesterValue();
        string GetCurrentSemesterInfo();
        string GetCurrentYearValue();
        string GetCurrentYearInfo();
    }
}
