using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers
{
    /// <summary>
    /// Tham khảo chương trình học dự kiến:
    /// https://mydtu.duytan.edu.vn/sites/index.aspx?p=home_listcurriculumforsemester&academicleveltypeid=3&curriculumid=629
    /// </summary>
    public class StudentPlanCrawler : IStudentPlanCrawler
    {
        private static readonly string PLAN_TABLE_XPATH = "//*[@class=\"tb_course\"]";
        private static readonly string TABLE_NAME_XPATH = "//*[@class=\"nobg nobd\"]";
        private static readonly string PLAN_RECORD_XPATH = "//tbody/tr";

        public async Task<IEnumerable<PlanTable>> GetPlanTables(
            int curriculumId,
            string sessionId)
        {
            string planFilePath = Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PLANS, curriculumId + ".html");
            string html;
            if (File.Exists(planFilePath))
            {
                html = await File.ReadAllTextAsync(planFilePath);
            }
            else if (sessionId != null)
            {
                string url = @$"https://mydtu.duytan.edu.vn/sites/index.aspx?p=home_listcurriculumforsemester&academicleveltypeid=3&curriculumid={curriculumId}";
                html = await DtuPageCrawler.GetHtml(sessionId, url);
                await File.WriteAllTextAsync(planFilePath, html);

            }
            else
            {
                return null;
            }

            HtmlDocument doc = new();
            doc.LoadHtml(html);
            HtmlNodeCollection planTableTags = doc.DocumentNode.SelectNodes(PLAN_TABLE_XPATH);
            List<PlanTable> planTables = new();
            foreach (HtmlNode planTableNode in planTableTags)
            {
                PlanTable planTable = new();

                HtmlDocument planTableNodeDoc = new HtmlDocument();
                planTableNodeDoc.LoadHtml(planTableNode.InnerHtml);
                HtmlNode tableNameTag = planTableNodeDoc.DocumentNode.SelectSingleNode(TABLE_NAME_XPATH);
                string tableName = StringHelper.SuperCleanString(tableNameTag.InnerText);
                planTable.Name = tableName;

                HtmlNodeCollection planRecordTags = planTableNodeDoc.DocumentNode.SelectNodes(PLAN_RECORD_XPATH);
                planTable.PlanRecords = planRecordTags.Select(ptt => GetPlanRecord(ptt)).ToList();
                planTables.Add(planTable);
            }
            return planTables;
        }

        private static PlanRecord GetPlanRecord(HtmlNode tr)
        {
            HtmlDocument trDoc = new HtmlDocument();
            trDoc.LoadHtml(tr.InnerHtml);
            HtmlNodeCollection tds = trDoc.DocumentNode.SelectNodes("//td");

            HtmlNode aTag = tds[0].SelectSingleNode("//a");
            string url = aTag.Attributes["href"].Value;
            string subjectName = aTag.Attributes["title"].Value;
            string subjectCode = StringHelper.SuperCleanString(aTag.InnerText);

            int studyUnit = int.Parse(StringHelper.SuperCleanString(tds[2].InnerText));

            return new PlanRecord()
            {
                SubjectCode = subjectCode,
                SubjectName = subjectName,
                Url = url,
                StudyUnit = studyUnit
            };
        }

        public async Task<IEnumerable<PlanTable>> GetPlanTables(int curriculumId)
        {
            return await GetPlanTables(curriculumId, null);
        }
    }
}
