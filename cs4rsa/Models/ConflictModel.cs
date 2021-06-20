using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Models.Enums;
using cs4rsa.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace cs4rsa.Models
{
    class ConflictModel : IConflictModel
    {
        private ClassGroup _classGroup1;
        private ClassGroup _classGroup2;
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

        public ClassGroup FirstClassGroup { get => _classGroup1; set => _classGroup1 = value; }
        public ClassGroup SecondClassGroup { get => _classGroup2; set => _classGroup2 = value; }

        public ConflictModel(Conflict conflict)
        {
            _classGroup1 = conflict.FirstClassGroup;
            _classGroup2 = conflict.SecondClassGroup;
            _conflictTime = conflict.GetConflictTime();
        }

        public ConflictModel(Conflict conflict, ConflictTime conflictTime)
        {
            _classGroup1 = conflict.FirstClassGroup;
            _classGroup2 = conflict.SecondClassGroup;
            _conflictTime = conflictTime;
        }

        public string GetConflictInfo()
        {
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, List<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
            {
                string day = BasicDataConverter.ToDayOfWeekText(item.Key);
                List<string> times = new List<string>();
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    string time = $"Từ {studyTimeIntersect.Start} đến {studyTimeIntersect.End}";
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
    }
}
