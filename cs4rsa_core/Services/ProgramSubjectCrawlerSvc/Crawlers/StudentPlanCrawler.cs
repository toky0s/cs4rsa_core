using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using HtmlAgilityPack;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers
{
    /// <summary>
    /// StudentPlanCrawler
    /// Tham khảo KHUNG CHƯƠNG TRÌNH DỰ KIẾN: K-24 - DƯỢC SĨ (ĐẠI HỌC)
    /// https://mydtu.duytan.edu.vn/sites/index.aspx?p=home_listcurriculumforsemester&academicleveltypeid=3&curriculumid=629
    /// 
    /// 1. Kiểm tra file JSON chứa chương trình học dữ kiến (nếu có), tới bước 4
    /// 2. Cào thông tin chương trình học dự kiến
    /// 3. Lưu dưới dạng file JSON curriculumId.json
    /// 4. Trừu tượng hoá thành các đối tượng PlanTable và trả về
    /// </summary>
    public class StudentPlanCrawler : IStudentPlanCrawler
    {
        private static readonly string PLAN_TABLE_XPATH = "//*[@class=\"tb_course\"]";
        private static readonly string TABLE_NAME_XPATH = "//*[@class=\"nobg nobd\"]";
        private static readonly string PLAN_RECORD_XPATH = "//tbody/tr";

        public async Task<List<PlanTable>> GetPlanTables(int curriculumId, string sessionId)
        {
            string url = @$"https://mydtu.duytan.edu.vn/sites/index.aspx?p=home_listcurriculumforsemester&academicleveltypeid=3&curriculumid={curriculumId}";
            string html = await DtuPageCrawler.GetHtml(sessionId, url);
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            HtmlNodeCollection planTableTags = doc.DocumentNode.SelectNodes(PLAN_TABLE_XPATH);
            List<PlanTable> planTables = new();
            foreach (HtmlNode planTableNode in planTableTags)
            {
                PlanTable planTable = new();

                HtmlDocument planTableNodeDoc = new();
                planTableNodeDoc.LoadHtml(planTableNode.InnerHtml);
                HtmlNode tableNameTag = planTableNodeDoc.DocumentNode.SelectSingleNode(TABLE_NAME_XPATH);
                string tableName = StringHelper.SuperCleanString(tableNameTag.InnerText);
                planTable.Name = tableName;

                HtmlNodeCollection planRecordTags = planTableNodeDoc.DocumentNode.SelectNodes(PLAN_RECORD_XPATH);
                planTable.PlanRecords = planRecordTags.Select(ptt => GetPlanRecord(ptt)).ToList();
                planTables.Add(planTable);
            }

            string planFilePath = Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PLANS, curriculumId + ".json");
            using StreamWriter sw = new(planFilePath);
            using JsonWriter writer = new JsonTextWriter(sw);
            JsonSerializer serializer = new();
            serializer.Serialize(writer, planTables);
            return planTables;
        }

        public async Task<List<PlanTable>> GetPlanTables(int curriculumId)
        {
            string planFilePath = Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PLANS, curriculumId + ".json");
            if (File.Exists(planFilePath))
            {
                using StreamReader sr = new(planFilePath);
                using JsonReader reader = new JsonTextReader(sr);
                string json = await reader.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PlanTable>>(json);
            }
            return null;
        }

        private static PlanRecord GetPlanRecord(HtmlNode tr)
        {
            HtmlDocument trDoc = new();
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
    }
}
