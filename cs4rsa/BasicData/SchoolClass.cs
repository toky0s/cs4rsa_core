using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    class SchoolClass
    {
        private string subjectCode;
        private string classGroupName;
        private string name;
        private string registerCode;
        private string type;
        private int emptySeat;
        private string registrationTermEnd;
        private string registrationTermStart;
        private StudyWeek studyWeek;
        private Schedule schedule;
        private string[] rooms;
        private string[] places;
        private string[] teacher;
        private string registrationStatus;
        private string implementationStatus;

        public string Name { get { return name; } }
        public string SubjectCode { get { return subjectCode; } }
        public string ClassGroupName { get { return classGroupName; } }

        public SchoolClass(string subjectCode, string classGroupName, string name, string registerCode, string type, 
            int emptySeat, string registrationTermEnd, string registrationTermStart, StudyWeek studyWeek, Schedule schedule, 
            string[] rooms, string[] places, string[] teacher, string registrationStatus, string implementationStatus)
        {
            this.subjectCode = subjectCode;
            this.classGroupName = classGroupName;
            this.name = name;
            this.registerCode = registerCode;
            this.type = type;
            this.emptySeat = emptySeat;
            this.registrationTermStart = registrationTermStart;
            this.registrationTermEnd = registrationTermEnd;
            this.studyWeek = studyWeek;
            this.schedule = schedule;
            this.rooms = rooms;
            this.places = places;
            this.teacher = teacher;
            this.registrationStatus = registrationStatus;
            this.implementationStatus = implementationStatus;
        }
    }
}
