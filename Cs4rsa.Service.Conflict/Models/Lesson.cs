using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.Service.Conflict.Models
{
    public class Lesson
    {
        public Schedule Schedule { get; }
        public StudyWeek StudyWeek { get; }
        public DayPlaceMetaData MetaData { get; }
        public Cs4rsaMetaData Cs4rsaMetaData { get; }
        public Phase Phase { get; }
        public string SchoolClassName { get; }
        public string ClassGroupName { get; }
        public string SubjectCode { get; }

        public Lesson(
            StudyWeek studyWeek, 
            Schedule schedule, 
            DayPlaceMetaData metaData, 
            Cs4rsaMetaData cs4rsaMetaData, 
            Phase phase,
            string schoolClassName,
            string classGroupName,
            string subjectCode)
        {
            StudyWeek = studyWeek;
            Schedule = schedule;
            SchoolClassName = schoolClassName;
            ClassGroupName = classGroupName;
            MetaData = metaData;
            Cs4rsaMetaData = cs4rsaMetaData;
            Phase = phase;
            SubjectCode = subjectCode;
        }
    }
}
