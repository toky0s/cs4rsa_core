using cs4rsa.BasicData;

namespace cs4rsa.Models
{
    public class TeacherModel
    {
        private readonly string id;
        private readonly string name;
        private readonly string sex;
        private readonly string place;
        private readonly string degree;
        private readonly string workUnit;
        private readonly string position;
        private readonly string subject;
        private readonly string form;
        private readonly string[] teachedSubjects;

        public string Id => id;
        public string Name => name;
        public string Sex => sex;
        public string Place => place;
        public string Degree => degree;
        public string WorkUnit => workUnit;
        public string Position => position;
        public string Subject => subject;
        public string Form => form;
        public string[] TeachedSubjects => teachedSubjects;

        public TeacherModel(Teacher teacher)
        {
            if (teacher != null)
            {
                id = teacher.Id;
                name = teacher.Name;
                sex = teacher.Sex;
                place = teacher.Place;
                degree = teacher.Degree;
                workUnit = teacher.WorkUnit;
                position = teacher.Position;
                subject = teacher.Subject;
                form = teacher.Form;
                teachedSubjects = teacher.TeachedSubjects;
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is TeacherModel teacher))
            {
                return false;
            }
            return id == teacher.Id;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
