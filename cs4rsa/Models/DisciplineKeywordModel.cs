using cs4rsa.Crawler;

namespace cs4rsa.Models
{
    /// <summary>
    /// Class này đại diện cho thông tin của một Discipline-Keyword1 của một Subject từ file JSON.
    /// </summary>
    public class DisciplineKeywordModel
    {
        private string courseId;
        private string subjectName;
        private string keyword1;

        public string CourseID
        {
            get
            {
                return courseId;
            }
            set
            {
                courseId = value;
            }
        }
        public string SubjectName
        {
            get
            {
                return subjectName;
            }
            set
            {
                subjectName = value;
            }
        }
        public string Keyword1
        {
            get
            {
                return keyword1;
            }
            set
            {
                keyword1 = value;
            }
        }

        public DisciplineKeywordModel(DisciplineKeywordInfo disciplineKeywordInfo)
        {
            this.courseId = disciplineKeywordInfo.CourseID;
            this.subjectName = disciplineKeywordInfo.SubjectName;
            this.keyword1 = disciplineKeywordInfo.Keyword1;
        }

        public DisciplineKeywordModel(string courseId, string subjectName, string keyword1)
        {
            this.courseId = courseId;
            this.subjectName = subjectName;
            this.keyword1 = keyword1;
        }
    }
}

