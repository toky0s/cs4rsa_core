using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public class SchoolClass
    {
        private readonly string subjectCode;
        private string classGroupName;
        private string name;
        private string registerCode;
        private string type;
        private string emptySeat;
        private string registrationTermEnd;
        private string registrationTermStart;
        private StudyWeek studyWeek;
        private Schedule schedule;
        private string[] rooms;
        private List<Place> places;
        private Teacher teacher;
        private string registrationStatus;
        private string implementationStatus;
        private DayPlaceMetaData dayPlaceMetaData;

        public string Name => name;
        public string SubjectCode => subjectCode;
        public string ClassGroupName => classGroupName;
        public string RegisterCode => registerCode;
        public string EmptySeat => emptySeat;
        public Schedule Schedule => schedule;
        public StudyWeek StudyWeek => studyWeek;
        public Teacher Teacher => teacher;
        public List<Place> Places => places;
        public DayPlaceMetaData DayPlaceMetaData => dayPlaceMetaData;

        public SchoolClass(string subjectCode, string classGroupName, string name, string registerCode, string type, 
            string emptySeat, string registrationTermEnd, string registrationTermStart, StudyWeek studyWeek, Schedule schedule, 
            string[] rooms, List<Place> places, Teacher teacher, string registrationStatus, string implementationStatus, DayPlaceMetaData dayPlaceMetaData)
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
            this.dayPlaceMetaData = dayPlaceMetaData;
        }
    }
}
