using Cs4rsaDatabaseService.Models;
using SubjectCrawlService1.DataTypes.Enums;
using System.Collections.Generic;

namespace SubjectCrawlService1.DataTypes
{
    public class SchoolClass
    {
        public string ClassGroupName { get; set; }
        public string SchoolClassName { get; set; }
        public string RegisterCode { get; set; }
        public string Type { get; set; }
        public string RegistrationTermEnd { get; set; }
        public string RegistrationTermStart { get; set; }
        public string EmptySeat { get; set; }
        public Schedule Schedule { get; set; }
        public string[] Rooms { get; set; }
        public StudyWeek StudyWeek { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<string> TempTeachers { get; set; }
        public List<Place> Places { get; set; }
        public string RegistrationStatus { get; set; }
        public string ImplementationStatus { get; set; }
        public string Url { get; set; }
        public DayPlaceMetaData DayPlaceMetaData { get; set; }

        public string Color { get; set; }

        public SchoolClass(string schoolClassName, string registerCode, string type,
            string emptySeat, string registrationTermEnd, string registrationTermStart, StudyWeek studyWeek, Schedule schedule,
            string[] rooms, List<Place> places, List<Teacher> teachers, List<string> tempTeachers, string registrationStatus, string implementationStatus,
            string url, DayPlaceMetaData dayPlaceMetaData)
        {
            SchoolClassName = schoolClassName;
            RegisterCode = registerCode;
            Type = type;
            EmptySeat = emptySeat;
            RegistrationTermStart = registrationTermStart;
            RegistrationTermEnd = registrationTermEnd;
            StudyWeek = studyWeek;
            Schedule = schedule;
            Rooms = rooms;
            Places = places;
            Teachers = teachers;
            TempTeachers = tempTeachers;
            RegistrationStatus = registrationStatus;
            ImplementationStatus = implementationStatus;
            Url = url;
            DayPlaceMetaData = dayPlaceMetaData;
        }

        public Phase GetPhase()
        {
            return StudyWeek.GetPhase();
        }

        public MetaDataMap GetMetaDataMap()
        {
            return new MetaDataMap(Schedule, DayPlaceMetaData);
        }
    }
}
