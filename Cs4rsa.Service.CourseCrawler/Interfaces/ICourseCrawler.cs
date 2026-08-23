namespace Cs4rsa.Service.CourseCrawler.Interfaces
{
    public interface ICourseCrawler
    {
        void GetInfo(out string yearInfo
            , out string yearValue
            , out string semesterInfo
            , out string semesterValue);
    }
}