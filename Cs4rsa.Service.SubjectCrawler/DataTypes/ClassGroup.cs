using System;
using System.Collections.Generic;
using System.Linq;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.SubjectCrawler.Utils;
using Cs4rsa.Service.TeacherCrawler.Models;

namespace Cs4rsa.Service.SubjectCrawler.DataTypes
{
    public class ClassGroup
    {
        private readonly List<SchoolClass> _schoolClasses;
        private readonly List<string> _registerCodes;

        public readonly string Name;
        public readonly string SubjectCode;
        public string SubjectName { get; }

        public List<string> RegisterCodes
        {
            get
            {
                return _registerCodes;
            }
        }

        private List<SchoolClass> _mergedSchoolClasses;
        public List<SchoolClass> SchoolClasses
        {
            get
            {
                if (_mergedSchoolClasses != null || !_mergedSchoolClasses.Any())
                {
                    _mergedSchoolClasses = MergeTeacherInfoInSchoolClasses();
                }
                return _mergedSchoolClasses;
            }
        }

        public ClassGroup(string name, string subjectCode, string subjectName)
        {
            _schoolClasses = new List<SchoolClass>();
            _registerCodes = new List<string>();
            _mergedSchoolClasses = new List<SchoolClass>();
            Name = name;
            SubjectCode = subjectCode;
            SubjectName = subjectName;
        }

        public void AddRangeSchoolClass(IEnumerable<SchoolClass> schoolClasses)
        {
            _schoolClasses.AddRange(schoolClasses);
            GetRegisterCode();
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
            var newSchoolClasses = new List<SchoolClass>();
            var schoolClassNames = GetSchoolClassNames();
            foreach (var schoolClassName in schoolClassNames)
            {
                var schoolClassesWithName = GetSchoolClassesWithName(schoolClassName);
                // List<TeacherModel> teachers = GetTeachersOfSchoolClasses(schoolClassesWithName);
                // schoolClassesWithName[0].Teachers = teachers;
                schoolClassesWithName[0].TempTeachers = GetTempTeachersOfSchoolClasses(schoolClassesWithName).ToList();
                schoolClassesWithName[0].SubjectName = SubjectName;
                newSchoolClasses.Add(schoolClassesWithName[0]);
            }

            return newSchoolClasses;
        }

        /// <summary>
        /// Lấy ra tất cả các TempTeacher có trong các SchoolClass, nhằm thực hiện 
        /// gộp các SchoolClass cùng tên lại với nhau, nhưng chỉ khác mỗi TempTeacher. 
        /// Tương tự GetTeachersOfSchoolClasses.
        /// </summary>
        /// <param name="schoolClasses">Danh sách các SchoolClass cùng tên.</param>
        /// <returns>Danh sách các TempTeacher của các SchoolClass trên.</returns>
        private static IEnumerable<string> GetTempTeachersOfSchoolClasses(IEnumerable<SchoolClass> schoolClasses)
        {
            return schoolClasses
                .SelectMany(schoolClass => schoolClass.TempTeachers);
        }

        /// <summary>
        /// Lấy ra danh sách tên riêng biệt của danh sách các SchoolClass được truyền vào.
        /// </summary>
        /// <param name="schoolClasses"></param>
        /// <returns></returns>
        private static IEnumerable<string> DistinctSchoolClassName(IEnumerable<SchoolClass> schoolClasses)
        {
            return schoolClasses.Select(item => item.ClassGroupName).Distinct();
        }

        private List<SchoolClass> GetSchoolClassesWithName(string schoolClassName)
        {
            var schoolClasses = new List<SchoolClass>();
            foreach (var schoolClass in _schoolClasses)
            {
                if (schoolClass.SchoolClassName.Equals(schoolClassName))
                    schoolClasses.Add(schoolClass);
            }
            return schoolClasses;
        }

        private IEnumerable<string> GetSchoolClassNames()
        {
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
            var DayOfWeekStudyTimePairs = new Dictionary<DayOfWeek, List<StudyTime>>();
            foreach (var schoolClass in _schoolClasses)
            {
                IEnumerable<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = schoolClass.Schedule.ScheduleTime;
                foreach (var pair in dayAndStudyTimes)
                {
                    if (!DayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                        DayOfWeekStudyTimePairs.Add(pair.Key, pair.Value);
                }
            }
            var schedule = new Schedule(DayOfWeekStudyTimePairs);
            return schedule;
        }

        /// <summary>
        /// Hợp nhất Schedule của các SchoolClass được truyền vào này thành một.
        /// </summary>
        /// <returns>Trả về một Schedule.</returns>
        public static Schedule GetSchedule(IEnumerable<SchoolClass> schoolClasses)
        {
            var DayOfWeekStudyTimePairs = new Dictionary<DayOfWeek, List<StudyTime>>();
            foreach (var schoolClass in schoolClasses)
            {
                IEnumerable<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = schoolClass.Schedule.ScheduleTime;
                foreach (var pair in dayAndStudyTimes)
                {
                    if (!DayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                        DayOfWeekStudyTimePairs.Add(pair.Key, pair.Value);
                }
            }
            var schedule = new Schedule(DayOfWeekStudyTimePairs);
            return schedule;
        }

        public IEnumerable<string> GetTempTeachers()
        {
            var tempTeachers = new List<string>();
            foreach (var schoolClass in _schoolClasses)
            {
                tempTeachers.AddRange(schoolClass.TempTeachers);
            }
            return tempTeachers;
        }

        public Phase GetPhase()
        {
            var phases = new List<Phase>();
            foreach (var schoolClass in _schoolClasses)
            {
                var phase = schoolClass.StudyWeek.GetPhase();
                if (!phases.Contains(phase))
                {
                    phases.Add(phase);
                }
            }
            return phases.Count == 2 ? Phase.All : phases[0];
        }

        public IEnumerable<TeacherModel> GetTeachers()
        {
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                foreach (TeacherModel teacher in schoolClass.Teachers)
                {
                    yield return teacher;
                }
            }
        }

        public IEnumerable<Place> GetPlaces()
        {
            var places = new List<Place>();
            foreach (var schoolClass in _schoolClasses)
            {
                places.AddRange(schoolClass.Places);
            }
            return places.Distinct();
        }

        private void GetRegisterCode()
        {
            foreach (var schoolClass in _schoolClasses)
            {
                // Trường hợp hai mã giống nhau mà khác giảng viên
                if (schoolClass.RegisterCode.Trim() != string.Empty && !_registerCodes.Contains(schoolClass.RegisterCode))
                {
                    _registerCodes.Add(schoolClass.RegisterCode);
                }
            }
        }

        /**
         * Mô tả:
         *      Lấy ra số chỗ còn trống.
         * 
         * Trả về:
         *      Trả về 0 nếu "Hết chỗ" hoặc số chỗ cào được âm, 
         *      ngược lại trả về giá trị parse được.
         */
        public int GetEmptySeat()
        {
            return _schoolClasses[0].EmptySeat.Equals("Hết chỗ") || int.Parse(_schoolClasses[0].EmptySeat) < 0
                ? 0
                : int.Parse(_schoolClasses[0].EmptySeat);
        }

        public string GetUrl()
        {
            foreach (var schoolClass in _schoolClasses)
            {
                if (schoolClass.Url.Trim() != string.Empty)
                {
                    return schoolClass.Url;
                }
            }
            return string.Empty;
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
