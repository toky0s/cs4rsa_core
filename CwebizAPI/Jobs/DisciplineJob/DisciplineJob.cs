/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Crawlers.DisciplineCrawlerSvc.Crawlers;
using Quartz;

namespace CwebizAPI.Jobs.DisciplineJob
{
    /// <summary>
    /// Thực hiện cập nhật thông tin mã ngành mã môn mỗi 23h hằng ngày.
    /// </summary>
    /// <remarks>
    /// Created Date: 06/10/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    [DisallowConcurrentExecution]
    public class DisciplineJob : IJob
    {
        private readonly DisciplineCrawler _disciplineCrawler;
        public static readonly JobKey JobKey = new("CrawlDisciplineKeywordJob");
        public const string Identity = "CrawlDisciplineKeywordJob-trigger";

        public DisciplineJob(DisciplineCrawler disciplineCrawler)
        {
            _disciplineCrawler = disciplineCrawler;
        }

        public Task Execute(IJobExecutionContext context)
        {
            return _disciplineCrawler.GetDisciplineAndKeyword();
        }
    }
}
