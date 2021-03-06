using Cs4rsaCommon.Enums;
using Cs4rsaCommon.Interfaces;
using Cs4rsaCommon.Models;

using Cs4rsaDatabaseService.Models;

using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;

using System.Collections.Generic;

namespace SubjectCrawlService1.Models
{
    public class SchoolClassModel : IScheduleTableItem
    {
        private readonly SchoolClass _schoolClass;

        public SchoolClass SchoolClass
        {
            get { return _schoolClass; }
        }

        private string _subjectCode;
        public string SubjectCode
        {
            get { return _subjectCode; }
            set { _subjectCode = value; }
        }

        private string _schoolClassName;
        public string SchoolClassName
        {
            get { return _schoolClassName; }
            set { _schoolClassName = value; }
        }

        private string _subjectName;
        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value; }
        }

        private string _registerCode;
        public string RegisterCode
        {
            get { return _registerCode; }
            set { _registerCode = value; }
        }

        private string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _emptySeat;
        public string EmptySeat
        {
            get { return _emptySeat; }
            set { _emptySeat = value; }
        }

        private string _registrationTermEnd;
        public string RegistrationTermEnd
        {
            get { return _registrationTermEnd; }
            set { _registrationTermEnd = value; }
        }

        private string _registrationTermStart;
        public string RegistrationTermStart
        {
            get { return _registrationTermStart; }
            set { _registrationTermStart = value; }
        }

        private StudyWeek _studyWeek;
        public StudyWeek StudyWeek
        {
            get { return _studyWeek; }
            set { _studyWeek = value; }
        }

        private Schedule _schedule;
        public Schedule Schedule
        {
            get { return _schedule; }
            set { _schedule = value; }
        }

        private IEnumerable<string> _rooms;
        public IEnumerable<string> Rooms
        {
            get => _rooms;
            set => _rooms = value;
        }

        private IEnumerable<Place> _places;
        public IEnumerable<Place> Places
        {
            get { return _places; }
            set { _places = value; }
        }

        private IEnumerable<Teacher> _teachers;
        public IEnumerable<Teacher> Teachers
        {
            get { return _teachers; }
            set { _teachers = value; }
        }

        private IEnumerable<string> _tempTeachers;
        public IEnumerable<string> TempTeachers
        {
            get { return _tempTeachers; }
            set { _tempTeachers = value; }
        }

        private string _registrationStatus;
        public string RegistrationStatus
        {
            get { return _registrationStatus; }
            set { _registrationStatus = value; }
        }

        private string _implementationStatus;
        public string ImplementationStatus
        {
            get { return _implementationStatus; }
            set { _implementationStatus = value; }
        }

        private DayPlaceMetaData _dayPlaceMetaData;
        public DayPlaceMetaData DayPlaceMetaData
        {
            get { return _dayPlaceMetaData; }
            set { _dayPlaceMetaData = value; }
        }

        public string Color { get; set; }

        public SchoolClassModel(SchoolClass schoolClass)
        {
            _schoolClass = schoolClass;
            _schoolClassName = schoolClass.SchoolClassName;
            _registerCode = schoolClass.RegisterCode;
            _type = schoolClass.Type;
            _emptySeat = schoolClass.EmptySeat;
            _registrationTermEnd = schoolClass.RegistrationTermEnd;
            _registrationTermStart = schoolClass.RegistrationTermStart;
            _studyWeek = schoolClass.StudyWeek;
            _schedule = schoolClass.Schedule;
            _rooms = schoolClass.Rooms;
            _places = schoolClass.Places;
            _teachers = schoolClass.Teachers;
            _tempTeachers = schoolClass.TempTeachers;
            _registrationStatus = schoolClass.RegistrationStatus;
            _implementationStatus = schoolClass.ImplementationStatus;
            _dayPlaceMetaData = schoolClass.DayPlaceMetaData;
            _subjectName = schoolClass.SubjectName;
        }

        public IEnumerable<TimeBlock> GetBlocks()
        {

            foreach (SchoolClassUnit item in _schoolClass.GetSchoolClassUnits())
            {
                string description = $"{SchoolClassName} | {SubjectName} | {item.Room.Place.ToActualPlace()} | Phòng {item.Room.Name}";
                TimeBlock timeBlock = new(
                            Color,
                            description,
                            item.DayOfWeek,
                            item.Start,
                            item.End,
                            BlockType.SchoolClass,
                            _schoolClassName,
                            code: _schoolClass.ClassGroupName
                        );
                yield return timeBlock;
            }
        }

        public object GetValue()
        {
            return this;
        }

        public ContextType GetContextType()
        {
            return ContextType.Class;
        }

        public string GetId()
        {
            return _schoolClassName;
        }
    }
}
