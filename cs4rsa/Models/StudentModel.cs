using cs4rsa.BasicData;
using cs4rsa.Enums;

namespace cs4rsa.Models
{
    public class StudentModel
    {
        private StudentInfo _studentInfo;
        public StudentInfo StudentInfo
        {
            get
            {
                return _studentInfo;
            }
            set
            {
                _studentInfo = value;
            }
        }

        public StudentModel(Student student)
        {
            _studentInfo = student.Info;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is StudentModel)
            {
                StudentModel other = obj as StudentModel;
                return _studentInfo.StudentId.Equals(other.StudentInfo.StudentId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _studentInfo.StudentId.GetHashCode();
        }
    }
}
