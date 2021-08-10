using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Models.Enums;
using cs4rsa.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace cs4rsa.Models
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
        private SchoolClassModel _firstSchoolClassModel;
        public SchoolClassModel FirstSchoolClassModel
        {
            get { return _firstSchoolClassModel; }
            set { _firstSchoolClassModel = value; }
        }

        private SchoolClassModel _secondSchoolClassModel;
        public SchoolClassModel SecondSchoolClassModel
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
            _firstSchoolClassModel = new SchoolClassModel(conflict.FirstSchoolClass);
            _secondSchoolClassModel = new SchoolClassModel(conflict.SecondSchoolClass);
            _conflictTime = conflict.GetConflictTime();
        }

        public ConflictModel(Conflict conflict, ConflictTime conflictTime)
        {
            _schoolClass1 = conflict.FirstSchoolClass;
            _schoolClass2 = conflict.SecondSchoolClass;
            _firstSchoolClassModel = new SchoolClassModel(conflict.FirstSchoolClass);
            _secondSchoolClassModel = new SchoolClassModel(conflict.SecondSchoolClass);
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
            if ((_schoolClass1.GetPhase() == Phase.FIRST && _schoolClass2.GetPhase() == Phase.FIRST) ||
                    (_schoolClass1.GetPhase() == Phase.FIRST && _schoolClass2.GetPhase() == Phase.ALL) ||
                    (_schoolClass1.GetPhase() == Phase.ALL && _schoolClass2.GetPhase() == Phase.FIRST))
                return Phase.FIRST;
            if ((_schoolClass1.GetPhase() == Phase.SECOND && _schoolClass2.GetPhase() == Phase.SECOND) ||
                    (_schoolClass1.GetPhase() == Phase.SECOND && _schoolClass2.GetPhase() == Phase.ALL) ||
                    (_schoolClass1.GetPhase() == Phase.ALL && _schoolClass2.GetPhase() == Phase.SECOND))
                return Phase.SECOND;
            return Phase.ALL;
        }
    }
}
