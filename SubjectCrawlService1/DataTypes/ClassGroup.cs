using Cs4rsaDatabaseService.Models;

using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SubjectCrawlService1.DataTypes
{
    public class ClassGroup
    {
        private readonly List<SchoolClass> _schoolClasses;
        private readonly List<SchoolClass> _reRenderSchoolClasses;
        private string _currentRegisterCodeForBelongSpecialSubject;
        public string Name { get; }
        public string SubjectCode { get; }
        public string SubjectName { get; }
        public List<SchoolClass> SchoolClasses => MergeTeacherInfoInSchoolClasses();

        public ClassGroup(string name, string subjectCode, string subjectName)
        {
            _schoolClasses = new();
            _reRenderSchoolClasses = new();
            Name = name;
            SubjectCode = subjectCode;
            SubjectName = subjectName;
        }

        public void ReRenderScheduleRequest(string registerCode)
        {
            _currentRegisterCodeForBelongSpecialSubject = registerCode;
            SchoolClass lecSchoolClass = _schoolClasses
                .Where(schoolClass => schoolClass.Type == "LEC")
                .FirstOrDefault();
            _reRenderSchoolClasses.Add(lecSchoolClass);
            SchoolClass labSchoolClass = _schoolClasses
                .Where(schoolClass => schoolClass.RegisterCode == registerCode)
                .FirstOrDefault();
            _reRenderSchoolClasses.Add(labSchoolClass);
        }

        public void AddSchoolClass(SchoolClass schoolClass)
        {
            _schoolClasses.Add(schoolClass);
        }

        public void AddRangeSchoolClass(IEnumerable<SchoolClass> schoolClasses)
        {
            _schoolClasses.AddRange(schoolClasses);
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
            List<SchoolClass> newSchoolClasses = new();
            IEnumerable<string> schoolClassNames = GetSchoolClassNames();
            foreach (string schoolClassName in schoolClassNames)
            {
                List<SchoolClass> schoolClassesWithName = GetSchoolClassesWithName(schoolClassName);
                List<Teacher> teachers = GetTeachersOfSchoolClasses(schoolClassesWithName);
                List<string> tempTeachers = GetTempTeachersOfSchoolClasses(schoolClassesWithName);
                schoolClassesWithName[0].Teachers = teachers;
                schoolClassesWithName[0].TempTeachers = tempTeachers;
                schoolClassesWithName[0].SubjectName = SubjectName;
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
        private static List<Teacher> GetTeachersOfSchoolClasses(IEnumerable<SchoolClass> schoolClasses)
        {
            if (DinstictSchoolClassName(schoolClasses).Length > 1)
            {
                Exception ex = new("SchoolClass's Name is difference!");
                throw ex;
            }
            else
            {
                List<Teacher> teachers = new();
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
        private static List<string> GetTempTeachersOfSchoolClasses(IEnumerable<SchoolClass> schoolClasses)
        {
            if (DinstictSchoolClassName(schoolClasses).Length > 1)
                throw new Exception("SchoolClass's Name is difference!");
            else
            {
                List<string> tempTeachers = new();
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
        private static string[] DinstictSchoolClassName(IEnumerable<SchoolClass> schoolClasses)
        {
            return schoolClasses.Select(item => item.ClassGroupName).Distinct().ToArray();
        }

        private List<SchoolClass> GetSchoolClassesWithName(string schoolClassName)
        {
            List<SchoolClass> schoolClasses = new();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                if (schoolClass.SchoolClassName.Equals(schoolClassName))
                    schoolClasses.Add(schoolClass);
            }
            return schoolClasses;
        }

        private IEnumerable<string> GetSchoolClassNames()
        {
            if (_currentRegisterCodeForBelongSpecialSubject != null)
            {
                return _reRenderSchoolClasses.Select(item => item.SchoolClassName).Distinct();
            }
            return _schoolClasses.Select(item => item.SchoolClassName).Distinct();
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
            Dictionary<DayOfWeek, List<StudyTime>> DayOfWeekStudyTimePairs = new();
            foreach (SchoolClass schoolClass in GetSchoolClasses())
            {
                List<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = schoolClass.Schedule.ScheduleTime.ToList();
                foreach (KeyValuePair<DayOfWeek, List<StudyTime>> pair in dayAndStudyTimes)
                {
                    if (!DayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                        DayOfWeekStudyTimePairs.Add(pair.Key, pair.Value);
                }
            }
            Schedule schedule = new(DayOfWeekStudyTimePairs);
            return schedule;
        }

        private List<SchoolClass> GetSchoolClasses()
        {
            if (_currentRegisterCodeForBelongSpecialSubject != null)
            {
                return _reRenderSchoolClasses;
            }
            return _schoolClasses;
        }

        public IEnumerable<string> GetTempTeachers()
        {
            List<string> tempTeachers = new();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                tempTeachers.AddRange(schoolClass.TempTeachers);
            }
            return tempTeachers;
        }

        public Phase GetPhase()
        {
            List<Phase> phases = new();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                Phase phase = schoolClass.StudyWeek.GetPhase();
                if (!phases.Contains(phase))
                {
                    phases.Add(phase);
                }
            }
            return phases.Count == 2 ? Phase.All : phases[0];
        }

        public IEnumerable<Teacher> GetTeachers()
        {
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                foreach (Teacher teacher in schoolClass.Teachers)
                {
                    yield return teacher;
                }
            }
        }

        public IEnumerable<Place> GetPlaces()
        {
            List<Place> places = new();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                places.AddRange(schoolClass.Places);
            }
            return places.Distinct();
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
        /// Lấy ra số chỗ còn trống. 
        /// <list type="bullet">LƯU Ý: Số chỗ còn trống có thể là số âm</list> 
        /// </summary>
        /// <returns>Số chỗ còn trống</returns>
        public short GetEmptySeat()
        {
            if (_schoolClasses[0].EmptySeat.Equals("Hết chỗ"))
                return 0;
            return short.Parse(_schoolClasses[0].EmptySeat);
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
