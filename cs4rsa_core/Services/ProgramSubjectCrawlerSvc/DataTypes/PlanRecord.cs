namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes
{
    public class PlanRecord
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// Số tín chỉ
        /// </summary>
        public int StudyUnit { get; set; }
        public string Color { get; set; }
    }
}
