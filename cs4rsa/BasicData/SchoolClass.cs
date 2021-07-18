using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public class SchoolClass
    {
        private readonly string _subjectCode;
        private readonly string _classGroupName;
        private readonly string _name;
        private readonly string _registerCode;
        private readonly string _type;
        private readonly string _emptySeat;
        private readonly string _registrationTermEnd;
        private readonly string _registrationTermStart;
        private readonly StudyWeek _studyWeek;
        private readonly Schedule _schedule;
        private readonly string[] _rooms;
        private readonly List<Place> _places;
        private readonly Teacher _teacher;
        private readonly string _registrationStatus;
        private readonly string _implementationStatus;
        private readonly DayPlaceMetaData _dayPlaceMetaData;

        public string Name => _name;
        public string SubjectCode => _subjectCode;
        public string ClassGroupName => _classGroupName;
        public string RegisterCode => _registerCode;
        public string EmptySeat => _emptySeat;
        public Schedule Schedule => _schedule;
        public string[] Rooms => _rooms;
        public StudyWeek StudyWeek => _studyWeek;
        public Teacher Teacher => _teacher;
        public List<Place> Places => _places;
        public string RegistrationStatus => _registrationStatus;
        public string ImplementationStatus => _implementationStatus;
        public DayPlaceMetaData DayPlaceMetaData => _dayPlaceMetaData;

        public SchoolClass(string subjectCode, string classGroupName, string name, string registerCode, string type, 
            string emptySeat, string registrationTermEnd, string registrationTermStart, StudyWeek studyWeek, Schedule schedule, 
            string[] rooms, List<Place> places, Teacher teacher, string registrationStatus, string implementationStatus, DayPlaceMetaData dayPlaceMetaData)
        {
            _subjectCode = subjectCode;
            _classGroupName = classGroupName;
            _name = name;
            _registerCode = registerCode;
            _type = type;
            _emptySeat = emptySeat;
            _registrationTermStart = registrationTermStart;
            _registrationTermEnd = registrationTermEnd;
            _studyWeek = studyWeek;
            _schedule = schedule;
            _rooms = rooms;
            _places = places;
            _teacher = teacher;
            _registrationStatus = registrationStatus;
            _implementationStatus = implementationStatus;
            _dayPlaceMetaData = dayPlaceMetaData;
        }
    }
}
