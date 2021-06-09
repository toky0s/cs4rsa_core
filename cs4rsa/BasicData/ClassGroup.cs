using System;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.BasicData
{
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
    /// </summary>
    public class ClassGroup
    {
        private readonly string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private readonly string subjectCode;
        public string SubjectCode
        {
            get
            {
                return subjectCode;
            }
        }

        private readonly List<SchoolClass> schoolClasses = new List<SchoolClass>();
        public List<SchoolClass> SchoolClasses { get { return schoolClasses; } }

        public ClassGroup(string name, string subjectCode)
        {
            this.name = name;
            this.subjectCode = subjectCode;
        }

        public void AddSchoolClass(SchoolClass schoolClass)
        {
            schoolClasses.Add(schoolClass);
        }

        /// <summary>
        /// Hợp nhất Schedule của các SchoolClass trong ClassGroup này thành một.
        /// </summary>
        /// <returns>Trả về một Schedule.</returns>
        public Schedule GetSchedule()
        {
            Dictionary<DayOfWeek, List<StudyTime>> DayOfWeekStudyTimePairs = new Dictionary<DayOfWeek, List<StudyTime>>();
            foreach(SchoolClass schoolClass in schoolClasses)
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

        public Phase GetPhase()
        {
            return schoolClasses[0].StudyWeek.GetPhase();
        }

        public List<Teacher> GetTeachers()
        {
            List<Teacher> teachers = new List<Teacher>();
            foreach(SchoolClass schoolClass in schoolClasses)
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
            foreach(SchoolClass schoolClass in schoolClasses)
            {
                sessions.AddRange(schoolClass.Schedule.GetSessions());
            }
            sessions = sessions.Distinct().ToList();
            return sessions;
        }

        public List<Place> GetPlaces()
        {
            List<Place> places = new List<Place>();
            foreach(SchoolClass schoolClass in schoolClasses)
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
            return schoolClasses[0].RegisterCode;
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
    }
}
