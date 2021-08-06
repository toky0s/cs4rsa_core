using cs4rsa.Enums;
using cs4rsa.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho nơi học của một lớp học.
    /// </summary>
    public enum Place
    {
        QUANGTRUNG,
        VIETTIN,
        PHANTHANH,
        HOAKHANH,
        NVL_254,
        NVL_137
    }

    /// <summary>
    /// Đại diện cho một nhóm lớp của một môn nào đó, thường sẽ bao gồm các lớp học LEC và LAB.
    /// Class này không được khởi tạo trực tiếp, mà được khởi tạo trong Subject vì theo kỹ thuật
    /// ClassGroup phải từ một Subject ra.
    /// </summary>
    public class ClassGroup
    {
        private readonly string _name;
        public string Name => _name;

        private readonly string _subjectCode;
        public string SubjectCode => _subjectCode;

        private readonly List<SchoolClass> _schoolClasses = new List<SchoolClass>();
        public List<SchoolClass> SchoolClasses => _schoolClasses;

        public ClassGroup(string name, string subjectCode)
        {
            _name = name;
            _subjectCode = subjectCode;
        }

        public void AddSchoolClass(SchoolClass schoolClass)
        {
            _schoolClasses.Add(schoolClass);
        }

        public DayPlaceMetaData GetDayPlaceMetaData()
        {
            return _schoolClasses[0].DayPlaceMetaData;
        }

        /// <summary>
        /// Hợp nhất Schedule của các SchoolClass trong ClassGroup này thành một.
        /// </summary>
        /// <returns>Trả về một Schedule.</returns>
        public Schedule GetSchedule()
        {
            Dictionary<DayOfWeek, List<StudyTime>> DayOfWeekStudyTimePairs = new Dictionary<DayOfWeek, List<StudyTime>>();
            foreach(SchoolClass schoolClass in _schoolClasses)
            {
                List<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = schoolClass.Schedule.ScheduleTime.ToList();
                foreach (KeyValuePair<DayOfWeek, List<StudyTime>> pair in dayAndStudyTimes)
                {
                    if (!DayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                        DayOfWeekStudyTimePairs.Add(pair.Key, pair.Value);
                }
            }
            Schedule schedule = new Schedule(DayOfWeekStudyTimePairs);
            return schedule;
        }

        public List<string> GetTempTeachers()
        {
            List<string> tempTeachers = new List<string>();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                tempTeachers.Add(schoolClass.TempTeacher);
            }
            return tempTeachers;
        }

        public Phase GetPhase()
        {
            List<Phase> phases = new List<Phase>();
            foreach(SchoolClass schoolClass in _schoolClasses)
            {
                Phase phase = schoolClass.StudyWeek.GetPhase();
                phases.Add(phase);
            }
            phases = phases.Distinct().ToList();
            return phases.Count == 2 ? Phase.ALL : phases[0];
            //return _schoolClasses[0].StudyWeek.GetPhase();
        }

        public List<Teacher> GetTeachers()
        {
            List<Teacher> teachers = new List<Teacher>();
            foreach(SchoolClass schoolClass in _schoolClasses)
            {
                teachers.Add(schoolClass.Teacher);
            }
            return teachers;
        }

        public List<DayOfWeek> GetDayOfWeeks()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>();
            foreach(DayOfWeek DayOfWeek in DayOfWeeks)
            {
                if (!DayOfWeeks.Contains(DayOfWeek))
                {
                    DayOfWeeks.Add(DayOfWeek);
                }
            }
            return DayOfWeeks;
        }

        public List<Session> GetSession()
        {
            List<Session> sessions = new List<Session>();
            foreach(SchoolClass schoolClass in _schoolClasses)
            {
                sessions.AddRange(schoolClass.Schedule.GetSessions());
            }
            sessions = sessions.Distinct().ToList();
            return sessions;
        }

        public List<Place> GetPlaces()
        {
            List<Place> places = new List<Place>();
            foreach(SchoolClass schoolClass in _schoolClasses)
            {
                places.AddRange(schoolClass.Places);
            }
            return places.Distinct().ToList();
        }

        public bool IsHaveThisTeacher(Teacher teacher)
        {
            return true;
        }

        public bool IsHaveThisPhase(Phase phase)
        {
            if (GetPhase() == phase)
                return true;
            return false;
        }

        public string GetRegisterCode()
        {
            foreach(SchoolClass schoolClass in _schoolClasses)
            {
                if (schoolClass.RegisterCode.Trim() != "")
                    return schoolClass.RegisterCode;
            }
            return "";
        }

        /// <summary>
        /// Hợp nhất hai StudyWeek của các SchoolClass trong ClassGroup này.
        /// </summary>
        /// <returns>Trả về StudyWeek của ClassGroup này.</returns>
        private StudyWeek GetStudyWeeks()
        {
            List<int> studyWeekValues = new List<int>();
            foreach(SchoolClass schoolClass in SchoolClasses)
            {
                studyWeekValues.Add(schoolClass.StudyWeek.StartWeek);
                studyWeekValues.Add(schoolClass.StudyWeek.EndWeek);
            }
            StudyWeek studyWeek = new StudyWeek(studyWeekValues.Min(), studyWeekValues.Max());
            return studyWeek;
        }

        /// <summary>
        /// Chỗ còn trống.
        /// </summary>
        /// <returns></returns>
        public int GetEmptySeat()
        {
            if (_schoolClasses[0].EmptySeat.Equals("Hết chỗ"))
                return 0;
            return int.Parse(_schoolClasses[0].EmptySeat);
        }

        public MetaDataMap GetMetaDataMap()
        {
            return new MetaDataMap(GetSchedule(), GetDayPlaceMetaData());
        }

        public ImplementType GetImplementType()
        {
            return BasicDataConverter.ToImplementType(_schoolClasses[0].ImplementationStatus);
        }

        public RegistrationType GetRegistrationType()
        {
            return BasicDataConverter.ToRegistrationType(_schoolClasses[0].RegistrationStatus);
        }
    }
}
