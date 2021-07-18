using cs4rsa.Crawler;

namespace cs4rsa.Models
{
    /// <summary>
    /// Class này đại diện cho thông tin của một Discipline-Keyword1 của một Subject từ file JSON.
    /// </summary>
    public class DisciplineKeywordModel
    {
        private string _courseId;
        private string _subjectName;
        private string _discipline;
        private string _keyword1;

        public string CourseID
        {
            get
            {
                return _courseId;
            }
            set
            {
                _courseId = value;
            }
        }
        public string SubjectName
        {
            get
            {
                return _subjectName;
            }
            set
            {
                _subjectName = value;
            }
        }
        public string Keyword1
        {
            get
            {
                return _keyword1;
            }
            set
            {
                _keyword1 = value;
            }
        }

        public DisciplineKeywordModel(string courseId, string subjectName, string discipline, string keyword1)
        {
            _courseId = courseId;
            _subjectName = subjectName;
            _discipline = discipline;
            _keyword1 = keyword1;
        }
    }
}

