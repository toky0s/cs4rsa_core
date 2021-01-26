using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    class SchoolClass
    {
        private string classGroupName;
        private string name;
        private string registerCode;
        private string type;
        private int emptySeat;
        private DateTime[] registrationTerm;
        private int[] studyWeek;
        private Schedule schedule;
        private string[] rooms;
        private string[] places;
        private string[] teacher;
        private string registrationStatus;
        private string implementationStatus;

        public SchoolClass(string classGroupName, string name, string registerCode, string type, 
            int emptySeat, DateTime[] registrationTerm, int[] studyWeek, Schedule schedule, 
            string[] rooms, string[] places, string[] teacher, string registrationStatus, string implementationStatus)
        {
            this.classGroupName = classGroupName;
            this.name = name;
            this.registerCode = registerCode;
            this.type = type;
            this.emptySeat = emptySeat;
            this.registrationTerm = registrationTerm;
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
