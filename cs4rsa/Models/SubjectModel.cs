using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Database;

namespace cs4rsa.Models
{
    public class SubjectModel
    {
        private Subject subject;

        public List<Teacher> Teachers
        {
            get
            {
                List<Teacher> teacherModels = subject.Teachers
                    .Select(teacher => new Teacher(teacher))
                    .ToList();
                return teacherModels;
            }
        }

        public List<string> TempTeachers => subject.TempTeachers;

        private List<ClassGroupModel> _classGroupModels;
        public List<ClassGroupModel> ClassGroupModels
        {
            get => _classGroupModels;
            set => _classGroupModels = value;
        }

        public string SubjectName => subject.Name;
        public string SubjectCode => subject.SubjectCode;

        private int _studyUnit;
        public int StudyUnit
        {
            get
            {
                return _studyUnit;
            }
            set
            {
                _studyUnit = value;
            }
        }

        public string CourseId => subject.CourseId;
        public string Color { get; set; }

        public SubjectModel(Subject subject)
        {
            this.subject = subject;
            _studyUnit = subject.StudyUnit;
            _classGroupModels = subject.ClassGroups.Select(item => new ClassGroupModel(item)).ToList();
            Color = ColorGenerator.GetColor(subject.CourseId);
        }

        /// <summary>
        /// Trả về ClassGroupModel theo tên được truyền vào, nếu không tồn tại ClassGroupModel
        /// theo tên đã truyền vào trả về null.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ClassGroupModel GetClassGroupModelWithName(string name)
        {
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.Name.Equals(name))
                    return classGroupModel;
            }
            return null;
        }
    }
}
