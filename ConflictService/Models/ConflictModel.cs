using ConflictService.DataTypes;
using ConflictService.DataTypes.Enums;
using ConflictService.Interfaces;
using Cs4rsaCommon.Enums;
using Cs4rsaCommon.Interfaces;
using Cs4rsaCommon.Models;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Utils;
using System;
using System.Collections.Generic;

namespace ConflictService.Models
{
    public class ConflictModel : IConflictModel, ICanShowOnScheduleTable
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
            foreach (KeyValuePair<DayOfWeek, List<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
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
            foreach (KeyValuePair<DayOfWeek, List<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
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

        public IEnumerable<TimeBlock> GetBlocks()
        {
            foreach (var item in ConflictTime.ConflictTimes)
            {
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    TimeBlock timeBlock = new()
                    {
                        Start = studyTimeIntersect.Start,
                        End = studyTimeIntersect.End,
                        Desciption = GetFullConflictInfo(),
                        Background = "Red",
                        DayOfWeek = item.Key,
                        BlockType = BlockType.TimeConflict
                    };
                    yield return timeBlock;
                }
            }
        }
    }
}
