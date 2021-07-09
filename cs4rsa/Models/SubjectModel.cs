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

        public List<TeacherModel> Teachers
        {
            get
            {
                List<TeacherModel> teacherModels = subject.Teachers
                    .Select(teacher => new TeacherModel(teacher))
                    .ToList();
                return teacherModels;
            }
        }

        public List<ClassGroupModel> ClassGroupModels
        {
            get
            {
                List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();
                foreach(ClassGroup classGroup in subject.ClassGroups)
                {
                    ClassGroupModel classGroupModel = new ClassGroupModel(classGroup);
                    classGroupModels.Add(classGroupModel);
                }
                return classGroupModels;
            }
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
