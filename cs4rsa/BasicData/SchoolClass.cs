using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Một School Class đại diện cho lớp thành phần của một Class Group có thể
    /// là LEC LAB Studio Dã Ngoại. Đây là đơn vị nhỏ nhất của một lớp. Chứa tất cả thông tin về lớp.
    /// </summary>
    public class SchoolClass
    {
        private string _schoolClassName;
        private string _registerCode;
        private string _type;
        private string _registrationTermEnd;
        private string _registrationTermStart;
        private string _emptySeat;
        private Schedule _schedule;
        private string[] _rooms;
        private StudyWeek _studyWeek;
        private List<Teacher> _teachers;
        private List<string> _tempTeachers;
        private List<Place> _places;
        private string _registrationStatus;
        private string _implementationStatus;
        private string _url;
        private DayPlaceMetaData _dayPlaceMetaData;

        public string ClassGroupName { get; set; }
        public string SchoolClassName { get => _schoolClassName; set => _schoolClassName = value; }
        public string RegisterCode { get => _registerCode; set => _registerCode = value; }
        public string Type { get => _type; set => _type = value; }
        public string RegistrationTermEnd { get => _registrationTermEnd; set => _registrationTermEnd = value; }
        public string RegistrationTermStart { get => _registrationTermStart; set => _registrationTermStart = value; }
        public string EmptySeat { get => _emptySeat; set => _emptySeat = value; }
        public Schedule Schedule { get => _schedule; set => _schedule = value; }
        public string[] Rooms { get => _rooms; set => _rooms = value; }
        public StudyWeek StudyWeek { get => _studyWeek; set => _studyWeek = value; }
        public List<Teacher> Teachers { get => _teachers; set => _teachers = value; }
        public List<string> TempTeachers { get => _tempTeachers; set => _tempTeachers = value; }
        public List<Place> Places { get => _places; set => _places = value; }
        public string RegistrationStatus { get => _registrationStatus; set => _registrationStatus = value; }
        public string ImplementationStatus { get => _implementationStatus; set => _implementationStatus = value; }
        public string Url { get => _url; set => _url = value; }
        public DayPlaceMetaData DayPlaceMetaData { get => _dayPlaceMetaData; set => _dayPlaceMetaData = value; }

        public SchoolClass(string schoolClassName, string registerCode, string type,
            string emptySeat, string registrationTermEnd, string registrationTermStart, StudyWeek studyWeek, Schedule schedule,
            string[] rooms, List<Place> places, List<Teacher> teachers, List<string> tempTeachers, string registrationStatus, string implementationStatus,
            string url, DayPlaceMetaData dayPlaceMetaData)
        {
            _schoolClassName = schoolClassName;
            _registerCode = registerCode;
            _type = type;
            _emptySeat = emptySeat;
            _registrationTermStart = registrationTermStart;
            _registrationTermEnd = registrationTermEnd;
            _studyWeek = studyWeek;
            _schedule = schedule;
            _rooms = rooms;
            _places = places;
            _teachers = teachers;
            _tempTeachers = tempTeachers;
            _registrationStatus = registrationStatus;
            _implementationStatus = implementationStatus;
            _url = url;
            _dayPlaceMetaData = dayPlaceMetaData;
        }

        public SchoolClass()
        {

        }

        public Phase GetPhase()
        {
            return _studyWeek.GetPhase();
        }

        public MetaDataMap GetMetaDataMap()
        {
            return new MetaDataMap(_schedule, _dayPlaceMetaData);
        }
    }

}
