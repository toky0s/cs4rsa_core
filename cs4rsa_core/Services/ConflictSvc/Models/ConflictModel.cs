using cs4rsa_core.Commons.Enums;
using cs4rsa_core.Commons.Interfaces;
using cs4rsa_core.Commons.Models;
using cs4rsa_core.Services.ConflictSvc.DataTypes;
using cs4rsa_core.Services.ConflictSvc.DataTypes.Enums;
using cs4rsa_core.Services.ConflictSvc.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;
using cs4rsa_core.Services.SubjectCrawlerSvc.Utils;

using System;
using System.Collections.Generic;

namespace cs4rsa_core.Services.ConflictSvc.Models
{
    public class ConflictModel : IConflictModel, IScheduleTableItem
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

        public ConflictType ConflictType { get => GetConflictType(); }

        public ConflictModel(Conflict conflict)
        {
            _schoolClass1 = conflict.FirstSchoolClass;
            _schoolClass2 = conflict.SecondSchoolClass;
            _conflictTime = conflict.GetConflictTime();
        }

        public ConflictModel(Conflict conflict, ConflictTime conflictTime)
        {
            _schoolClass1 = conflict.FirstSchoolClass;
            _schoolClass2 = conflict.SecondSchoolClass;
            _conflictTime = conflictTime;
        }

        /// <summary>
        /// Lấy ra thông tin dạng chuỗi để hiển thị lên giao diện của một xung đột về thời gian.
        /// </summary>
        public string GetConflictInfo()
        {
            List<string> resultTimes = new();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
            {
                string day = BasicDataConverter.ToDayOfWeekText(item.Key);
                List<string> times = new();
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

        /// <summary>
        /// Lấy ra đầy đủ thông tin của xung đột.
        /// </summary>
        public string GetFullConflictInfo()
        {
            List<string> resultTimes = new();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
            {
                string day = BasicDataConverter.ToDayOfWeekText(item.Key);
                List<string> times = new();
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    string time = $"Từ {studyTimeIntersect.StartString} đến {studyTimeIntersect.EndString}";
                    times.Add(time);
                }
                string timeString = string.Join("\n", times);
                resultTimes.Add(day + "\n" + timeString);
            }
            string schoolClassesInfo = _schoolClass1.SchoolClassName + " x " + _schoolClass2.SchoolClassName;
            string timesInfo = string.Join("\n", resultTimes);
            return "Xung đột: " + schoolClassesInfo + "\n" + timesInfo;
        }

        public ConflictType GetConflictType()
        {
            return ConflictType.Time;
        }

        public Phase GetPhase()
        {
            if (_schoolClass1.GetPhase() == Phase.First && _schoolClass2.GetPhase() == Phase.First ||
                    _schoolClass1.GetPhase() == Phase.First && _schoolClass2.GetPhase() == Phase.All ||
                    _schoolClass1.GetPhase() == Phase.All && _schoolClass2.GetPhase() == Phase.First)
                return Phase.First;
            if (_schoolClass1.GetPhase() == Phase.Second && _schoolClass2.GetPhase() == Phase.Second ||
                    _schoolClass1.GetPhase() == Phase.Second && _schoolClass2.GetPhase() == Phase.All ||
                    _schoolClass1.GetPhase() == Phase.All && _schoolClass2.GetPhase() == Phase.Second)
                return Phase.Second;
            return Phase.All;
        }

        public IEnumerable<TimeBlock> GetBlocks()
        {
            const string BACKGROUND = "#e74c3c";
            foreach (var item in ConflictTime.ConflictTimes)
            {
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    TimeBlock timeBlock = new(
                        BACKGROUND,
                        GetFullConflictInfo(),
                        item.Key,
                        studyTimeIntersect.Start,
                        studyTimeIntersect.End,
                        BlockType.TimeConflict,
                        _schoolClass1.SchoolClassName + " x " + _schoolClass2.SchoolClassName,
                        class1: _schoolClass1.ClassGroupName,
                        class2: _schoolClass2.ClassGroupName
                    );
                    yield return timeBlock;
                }
            }
        }

        public object GetValue()
        {
            return this;
        }

        public ContextType GetContextType()
        {
            return ContextType.Conflict;
        }

        public string GetId()
        {
            return _schoolClass1.SchoolClassName + _schoolClass2.SchoolClassName;
        }
    }
}
