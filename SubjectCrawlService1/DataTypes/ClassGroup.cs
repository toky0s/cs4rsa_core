using SubjectCrawlService1.DataTypes.Enums;
using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubjectCrawlService1.Utils;

namespace SubjectCrawlService1.DataTypes
{
    public class ClassGroup
    {
        private readonly string _name;
        public string Name => _name;

        private readonly string _subjectCode;
        public string SubjectCode => _subjectCode;

        private readonly List<SchoolClass> _schoolClasses = new List<SchoolClass>();
        public List<SchoolClass> SchoolClasses => GetSchoolClasses();

        public ClassGroup(string name, string subjectCode)
        {
            _name = name;
            _subjectCode = subjectCode;
        }

        public void AddSchoolClass(SchoolClass schoolClass)
        {
            _schoolClasses.Add(schoolClass);
        }

        public List<SchoolClass> GetSchoolClasses()
        {
            return MergeTeacherInfoInSchoolClasses();
        }

        /// <summary>
        /// Vì sẽ có một số lớp có nhiều school class trùng tên, nhưng chỉ khác ở tên giáo viên dạy
        /// điển hình là môn Machine Learning with Large Datasets (DS 423) lần phát hiện gần nhất 9/8/2021
        /// đợt đăng ký tín chỉ học kỳ I khiến cho việc xử lý xung đột trở nên phức tạp. Thành ra sẽ phải
        /// thực hiện gộp tất cả các school class trùng tên thành 1.
        /// </summary>
        /// <returns></returns>
        private List<SchoolClass> MergeTeacherInfoInSchoolClasses()
        {
            List<SchoolClass> newSchoolClasses = new List<SchoolClass>();
            List<string> schoolClassNames = GetSchoolClassNames();
            foreach (string schoolClassName in schoolClassNames)
            {
                List<SchoolClass> schoolClassesWithName = GetSchoolClassesWithName(schoolClassName);
                List<Teacher> teachers = GetTeachersOfSchoolClasses(schoolClassesWithName);
                List<string> tempTeachers = GetTempTeachersOfSchoolClasses(schoolClassesWithName);
                schoolClassesWithName[0].Teachers = teachers;
                schoolClassesWithName[0].TempTeachers = tempTeachers;
                newSchoolClasses.Add(schoolClassesWithName[0]);
            }
            return newSchoolClasses;
        }


        /// <summary>
        /// Lấy ra tất cả các Teacher có trong các SchoolClass, nhằm thực hiện 
        /// gộp các SchoolClass cùng tên lại với nhau, nhưng chỉ khác mỗi Teacher.
        /// </summary>
        /// <param name="schoolClasses">Danh sách các SchoolClass cùng tên.</param>
        /// <returns>Danh sách các Teacher của các SchoolClass trên.</returns>
        private List<Teacher> GetTeachersOfSchoolClasses(IEnumerable<SchoolClass> schoolClasses)
        {
            if (DinstictSchoolClassName(schoolClasses).Length > 1)
            {
                Exception ex = new("SchoolClass's Name is difference!");
                throw ex;
            }
            else
            {
                List<Teacher> teachers = new List<Teacher>();
                foreach (SchoolClass schoolClass in schoolClasses)
                {
                    teachers.AddRange(schoolClass.Teachers);
                }
                return teachers;
            }
        }


        /// <summary>
        /// Lấy ra tất cả các TempTeacher có trong các SchoolClass, nhằm thực hiện 
        /// gộp các SchoolClass cùng tên lại với nhau, nhưng chỉ khác mỗi TempTeacher. 
        /// Tương tự GetTeachersOfSchoolClasses.
        /// </summary>
        /// <param name="schoolClasses">Danh sách các SchoolClass cùng tên.</param>
        /// <returns>Danh sách các TempTeacher của các SchoolClass trên.</returns>
        private List<string> GetTempTeachersOfSchoolClasses(IEnumerable<SchoolClass> schoolClasses)
        {
            if (DinstictSchoolClassName(schoolClasses).Length > 1)
                throw new Exception("SchoolClass's Name is difference!");
            else
            {
                List<string> tempTeachers = new List<string>();
                foreach (SchoolClass schoolClass in schoolClasses)
                {
                    tempTeachers.AddRange(schoolClass.TempTeachers);
                }
                return tempTeachers;
            }
        }

        /// <summary>
        /// Lấy ra danh sách tên riêng biệt của danh sách các SchoolClass được truyền vào.
        /// </summary>
        /// <param name="schoolClasses"></param>
        /// <returns></returns>
        private string[] DinstictSchoolClassName(IEnumerable<SchoolClass> schoolClasses)
        {
            return schoolClasses.Select(item => item.ClassGroupName).Distinct().ToArray();
        }

        private List<SchoolClass> GetSchoolClassesWithName(string schoolClassName)
        {
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                if (schoolClass.SchoolClassName.Equals(schoolClassName))
                    schoolClasses.Add(schoolClass);
            }
            return schoolClasses;
        }

        private List<string> GetSchoolClassNames()
        {
            return _schoolClasses.Select(item => item.SchoolClassName).Distinct().ToList();
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
            foreach (SchoolClass schoolClass in _schoolClasses)
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
                tempTeachers.AddRange(schoolClass.TempTeachers);
            }
            return tempTeachers;
        }

        public Phase GetPhase()
        {
            List<Phase> phases = new List<Phase>();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                Phase phase = schoolClass.StudyWeek.GetPhase();
                phases.Add(phase);
            }
            phases = phases.Distinct().ToList();
            return phases.Count == 2 ? Phase.All : phases[0];
        }

        public List<Teacher> GetTeachers()
        {
            List<Teacher> teachers = new List<Teacher>();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                teachers.AddRange(schoolClass.Teachers);
            }
            return teachers;
        }


        /// <summary>
        /// Lấy ra các thứ học tại một giai đoạn.
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        public List<DayOfWeek> GetDayOfWeeks(Phase phase)
        {
            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                if (schoolClass.StudyWeek.GetPhase() == phase)
                    dayOfWeeks.AddRange(schoolClass.Schedule.GetSchoolDays());
            }
            return dayOfWeeks;
        }

        public List<Enums.Session> GetSession()
        {
            List<Enums.Session> sessions = new List<Enums.Session>();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                sessions.AddRange(schoolClass.Schedule.GetSessions());
            }
            sessions = sessions.Distinct().ToList();
            return sessions;
        }

        public List<Place> GetPlaces()
        {
            List<Place> places = new List<Place>();
            foreach (SchoolClass schoolClass in _schoolClasses)
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
            foreach (SchoolClass schoolClass in _schoolClasses)
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
            foreach (SchoolClass schoolClass in SchoolClasses)
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

        public string GetUrl()
        {
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                if (schoolClass.Url.Trim() != "")
                {
                    return schoolClass.Url;
                }
            }
            return "";
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
