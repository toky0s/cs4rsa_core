namespace cs4rsa.BasicData
{
    public class Teacher
    {
        private string id;
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
            set
            {
                id = value;
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

        public Teacher(string name)
        {
            this.name = name;
        }

        public Teacher(string id, string name, string sex, string place,
            string degree, string workUnit, string position, string subject,
            string form, string[] teachedSubjects)
        {
            this.id = id;
            this.name = name;
            this.sex = sex;
            this.place = place;
            this.degree = degree;
            this.workUnit = workUnit;
            this.position = position;
            this.subject = subject;
            this.form = form;
            this.teachedSubjects = teachedSubjects;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Teacher teacher))
            {
                return false;
            }
            return this.id == teacher.id;
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}
