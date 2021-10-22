using ConflictService.DataTypes;
using ConflictService.DataTypes.Enums;
using cs4rsa_core.Models.Interfaces;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Utils;
using System;
using System.Collections.Generic;

namespace cs4rsa_core.Models
{
    public class ConflictModel : IConflictModel
    {
        private SchoolClass _schoolClass1;
        private SchoolClass _schoolClass2;
        private ConflictTime _conflictTime;

        public ConflictTime ConflictTime
        {
            get
            {
                return _conflictTime;
            }
            set
            {
                _conflictTime = value;
            }
        }

        public SchoolClass FirstSchoolClass { get => _schoolClass1; set => _schoolClass1 = value; }
        public SchoolClass SecondSchoolClass { get => _schoolClass2; set => _schoolClass2 = value; }

        #region Models
        private SchoolClass _firstSchoolClassModel;
        public SchoolClass FirstSchoolClassModel
        {
            get { return _firstSchoolClassModel; }
            set { _firstSchoolClassModel = value; }
        }

        private SchoolClass _secondSchoolClassModel;
        public SchoolClass SecondSchoolClassModel
        {
            get { return _secondSchoolClassModel; }
            set { _secondSchoolClassModel = value; }
        }
        #endregion


        public ConflictType ConflictType { get => GetConflictType(); }

        public ConflictModel(Conflict conflict)
        {
            _schoolClass1 = conflict.FirstSchoolClass;
            _schoolClass2 = conflict.SecondSchoolClass;
            _firstSchoolClassModel = conflict.FirstSchoolClass;
            _secondSchoolClassModel = conflict.SecondSchoolClass;
            _conflictTime = conflict.GetConflictTime();
        }

        public ConflictModel(Conflict conflict, ConflictTime conflictTime)
        {
            _schoolClass1 = conflict.FirstSchoolClass;
            _schoolClass2 = conflict.SecondSchoolClass;
            _firstSchoolClassModel = conflict.FirstSchoolClass;
            _secondSchoolClassModel = conflict.SecondSchoolClass;
            _conflictTime = conflictTime;
        }

        /// <summary>
        /// Lấy ra thông tin dạng chuỗi để hiển thị lên giao diện của một xung đột về thời gian.
        /// </summary>
        /// <returns></returns>
        public string GetConflictInfo()
        {
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, List<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
            {
                string day = BasicDataConverter.ToDayOfWeekText(item.Key);
                List<string> times = new List<string>();
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    string time = $"Từ {studyTimeIntersect.StartString} đến {studyTimeIntersect.EndString}";
                    times.Add(time);
                }
                string timeString = string.Join("\n", times);
                resultTimes.Add(day + "\n" + timeString);
            }
            return string.Join("\n", resultTimes);
        }

        public ConflictType GetConflictType()
        {
            return ConflictType.Time;
        }

        public Phase GetPhase()
        {
            if ((_schoolClass1.GetPhase() == Phase.First && _schoolClass2.GetPhase() == Phase.First) ||
                    (_schoolClass1.GetPhase() == Phase.First && _schoolClass2.GetPhase() == Phase.All) ||
                    (_schoolClass1.GetPhase() == Phase.All && _schoolClass2.GetPhase() == Phase.First))
                return Phase.First;
            if ((_schoolClass1.GetPhase() == Phase.Second && _schoolClass2.GetPhase() == Phase.Second) ||
                    (_schoolClass1.GetPhase() == Phase.Second && _schoolClass2.GetPhase() == Phase.All) ||
                    (_schoolClass1.GetPhase() == Phase.All && _schoolClass2.GetPhase() == Phase.Second))
                return Phase.Second;
            return Phase.All;
        }
    }
}
