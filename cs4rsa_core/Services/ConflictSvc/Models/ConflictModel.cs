using Cs4rsa.Interfaces;
using Cs4rsa.Models;
using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.DataTypes.Enums;
using Cs4rsa.Services.ConflictSvc.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.Utils;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.Models
{
    public class ConflictModel : IConflictModel, IScheduleTableItem
    {
        private SchoolClassModel _schoolClass1;
        private SchoolClassModel _schoolClass2;
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

        public SchoolClassModel FirstSchoolClass { get => _schoolClass1; set => _schoolClass1 = value; }
        public SchoolClassModel SecondSchoolClass { get => _schoolClass2; set => _schoolClass2 = value; }

        public ConflictModel(Conflict conflict)
        {
            _schoolClass1 = conflict.FirstSchoolClass;
            _schoolClass2 = conflict.SecondSchoolClass;
            _conflictTime = conflict.GetConflictTime();
        }

        /// <summary>
        /// Lấy ra thông tin dạng chuỗi để hiển thị lên giao diện của một xung đột về thời gian.
        /// </summary>
        public string GetConflictInfo()
        {
            List<string> resultTimes = new();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
            {
                string day = item.Key.ToDayOfWeekText();
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
                string day = item.Key.ToDayOfWeekText();
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
            return "Trùng lịch\n" + schoolClassesInfo + "\n" + timesInfo;
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
            const string background = "#e74c3c";
            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in ConflictTime.ConflictTimes)
            {
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    CfBlock timeBlock = new(
                        studyTimeIntersect,
                        GetId(),
                        background,
                        _schoolClass1.SchoolClassName + " x " + _schoolClass2.SchoolClassName,
                        item.Key,
                        ScheduleTableItemType.TimeConflict,
                        _schoolClass1,
                        _schoolClass2
                    );
                    yield return timeBlock;
                }
            }
        }
        
        public string GetId()
        {
            return "tc" + ' ' + FirstSchoolClass.SubjectCode + ' ' + SecondSchoolClass.SubjectCode;
        }
    }
}
