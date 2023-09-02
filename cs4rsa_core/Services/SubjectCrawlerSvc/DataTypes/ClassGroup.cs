﻿using Cs4rsa.Constants;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Utils;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Cs4rsa.Services.SubjectCrawlerSvc.DataTypes
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
            _schoolClasses = new();
            _registerCodes = new();
            _mergedSchoolClasses = new();
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
            List<SchoolClass> newSchoolClasses = new();
            IEnumerable<string> schoolClassNames = GetSchoolClassNames();
            foreach (string schoolClassName in schoolClassNames)
            {
                List<SchoolClass> schoolClassesWithName = GetSchoolClassesWithName(schoolClassName);
                List<TeacherModel> teachers = GetTeachersOfSchoolClasses(schoolClassesWithName);
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
        private static List<TeacherModel> GetTeachersOfSchoolClasses(IEnumerable<SchoolClass> schoolClasses)
        {
            if (DinstictSchoolClassName(schoolClasses).Count() > 1)
            {
                Exception ex = new(VmConstants.SchoolClassNameIsDiffException);
                throw ex;
            }
            else
            {
                List<TeacherModel> teachers = new();
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
            if (DinstictSchoolClassName(schoolClasses).Count() > 1)
                throw new Exception(VmConstants.SchoolClassNameIsDiffException);
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
        private static IEnumerable<string> DinstictSchoolClassName(IEnumerable<SchoolClass> schoolClasses)
        {
            return schoolClasses.Select(item => item.ClassGroupName).Distinct();
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
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                IEnumerable<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = schoolClass.Schedule.ScheduleTime;
                foreach (KeyValuePair<DayOfWeek, List<StudyTime>> pair in dayAndStudyTimes)
                {
                    if (!DayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                        DayOfWeekStudyTimePairs.Add(pair.Key, pair.Value);
                }
            }
            Schedule schedule = new(DayOfWeekStudyTimePairs);
            return schedule;
        }

        /// <summary>
        /// Hợp nhất Schedule của các SchoolClass được truyền vào này thành một.
        /// </summary>
        /// <returns>Trả về một Schedule.</returns>
        public static Schedule GetSchedule(IEnumerable<SchoolClass> schoolClasses)
        {
            Dictionary<DayOfWeek, List<StudyTime>> dayOfWeekStudyTimePairs = new();
            foreach (SchoolClass schoolClass in schoolClasses)
            {
                IEnumerable<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = schoolClass.Schedule.ScheduleTime;
                foreach (KeyValuePair<DayOfWeek, List<StudyTime>> pair in dayAndStudyTimes)
                {
                    if (!dayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                    {
                        dayOfWeekStudyTimePairs[pair.Key] = pair.Value;
                    }
                }
            }
            Schedule schedule = new(dayOfWeekStudyTimePairs);
            return schedule;
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
            List<Place> places = new();
            foreach (SchoolClass schoolClass in _schoolClasses)
            {
                places.AddRange(schoolClass.Places);
            }
            return places.Distinct();
        }

        private void GetRegisterCode()
        {
            foreach (SchoolClass schoolClass in _schoolClasses)
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
            foreach (SchoolClass schoolClass in _schoolClasses)
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
