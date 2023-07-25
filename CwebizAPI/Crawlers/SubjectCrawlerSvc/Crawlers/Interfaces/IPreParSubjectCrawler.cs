namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.Crawlers.Interfaces
{
    public interface IPreParSubjectCrawler
    {
        Task<Tuple<IEnumerable<string>, IEnumerable<string>>> Run(string courseId, bool isUseCache);
    }
}
