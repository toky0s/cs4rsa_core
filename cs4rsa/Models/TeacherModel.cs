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

        public string Id
        {
            get
            {
                return id;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public string Sex
        {
            get
            {
                return sex;
            }
        }
        public string Place
        {
            get
            {
                return place;
            }
        }
        public string Degree
        {
            get
            {
                return degree;
            }
        }
        public string WorkUnit
        {
            get
            {
                return workUnit;
            }
        }
        public string Position
        {
            get
            {
                return position;
            }
        }
        public string Subject
        {
            get
            {
                return subject;
            }
        }
        public string Form
        {
            get
            {
                return form;
            }
        }
        public string[] TeachedSubjects
        {
            get
            {
                return teachedSubjects;
            }
        }

        public TeacherModel(Teacher teacher)
        {
            if (teacher != null)
            {
                this.id = teacher.Id;
                this.name = teacher.Name;
                this.sex = teacher.Sex;
                this.place = teacher.Place;
                this.degree = teacher.Degree;
                this.workUnit = teacher.WorkUnit;
                this.position = teacher.Position;
                this.subject = teacher.Subject;
                this.form = teacher.Form;
                this.teachedSubjects = teacher.TeachedSubjects;
            }
        }
    }
}
