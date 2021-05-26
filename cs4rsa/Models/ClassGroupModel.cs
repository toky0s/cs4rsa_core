using System.Collections.Generic;
using cs4rsa.BasicData;
using cs4rsa.Models;

namespace cs4rsa.Models
{
    public class ClassGroupModel
    {
        public ClassGroup classGroup;

        public string Name => classGroup.Name;
        public string SubjectCode => classGroup.SubjectCode;
        public Phase Phase => classGroup.GetPhase();
        public Schedule Schedule => classGroup.GetSchedule();
        public List<Place> Places => classGroup.GetPlaces();

        public ClassGroupModel(ClassGroup classGroup)
        {
            this.classGroup = classGroup;
        }

        public List<TeacherModel> GetTeacherModels()
        {
            List<TeacherModel> teacherModels = new List<TeacherModel>();
            foreach (Teacher teacher in classGroup.GetTeachers())
                teacherModels.Add(new TeacherModel(teacher));
            return teacherModels;
        }
    }
}
