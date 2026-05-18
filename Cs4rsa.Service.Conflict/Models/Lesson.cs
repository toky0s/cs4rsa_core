using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.Service.Conflict.Models
{
    public class Lesson
    {
        public Schedule Schedule { get; }
        public StudyWeek StudyWeek { get; }
        public DayPlaceMetadata DayPlaceMetadata { get; }
        public Metadata Metadata { get; }
        public Phase Phase { get; }
        public string SchoolClassName { get; }
        public string ClassGroupName { get; }
        public string SubjectCode { get; }
        public string SubjectName { get; }

        public Lesson(
            StudyWeek studyWeek, 
            Schedule schedule, 
            DayPlaceMetadata dayPlaceMetadata, 
            Metadata cs4rsaMetaData, 
            Phase phase,
            string schoolClassName,
            string classGroupName,
            string subjectCode,
            string subjectName)
        {
            StudyWeek = studyWeek;
            Schedule = schedule;
            SchoolClassName = schoolClassName;
            ClassGroupName = classGroupName;
            DayPlaceMetadata = dayPlaceMetadata;
            Metadata = cs4rsaMetaData;
            Phase = phase;
            SubjectCode = subjectCode;
            SubjectName = subjectName;
        }
    }
}
