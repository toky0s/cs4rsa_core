using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.DataTypes.Enums;
using Cs4rsa.Service.Conflict.Interfaces;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.SubjectCrawler.Utils;
using Cs4rsa.UI.ScheduleTable.Interfaces;

using System;
using System.Collections.Generic;

namespace Cs4rsa.UI.ScheduleTable.Models
{
    public class ConflictModel : IConflictModel, IScheduleTableItem
    {
        private Lesson _lessonA;
        private Lesson _lessonB;
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

        public Lesson LessonA { get => _lessonA; set => _lessonA = value; }
        public Lesson LessonB { get => _lessonB; set => _lessonB = value; }

        public ConflictModel(Conflict conflict)
        {
            _lessonA = conflict.LessonA;
            _lessonB = conflict.LessonB;
            _conflictTime = conflict.GetConflictTime();
        }

        /// <summary>
        /// Lấy ra thông tin dạng chuỗi để hiển thị lên giao diện của một xung đột về thời gian.
        /// </summary>
        public string GetConflictInfo()
        {
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
            {
                string day = item.Key.ToCs4rsaVietnamese();
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

        /// <summary>
        /// Lấy ra đầy đủ thông tin của xung đột.
        /// </summary>
        public string GetFullConflictInfo()
        {
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<StudyTimeIntersect>> item in _conflictTime.ConflictTimes)
            {
                string day = item.Key.ToCs4rsaVietnamese();
                List<string> times = new List<string>();
                foreach (StudyTimeIntersect studyTimeIntersect in item.Value)
                {
                    string time = $"Từ {studyTimeIntersect.StartString} đến {studyTimeIntersect.EndString}";
                    times.Add(time);
                }
                string timeString = string.Join("\n", times);
                resultTimes.Add(day + "\n" + timeString);
            }
            string schoolClassesInfo = _lessonA.SchoolClassName + " x " + _lessonB.SchoolClassName;
            string timesInfo = string.Join("\n", resultTimes);
            return "Trùng lịch\n" + schoolClassesInfo + "\n" + timesInfo;
        }

        public ConflictType GetConflictType()
        {
            return ConflictType.Time;
        }

        public Phase GetPhase()
        {
            if (_lessonA.Phase == Phase.First && _lessonB.Phase == Phase.First ||
                    _lessonA.Phase == Phase.First && _lessonB.Phase == Phase.All ||
                    _lessonA.Phase == Phase.All && _lessonB.Phase == Phase.First)
                return Phase.First;
            if (_lessonA.Phase == Phase.Second && _lessonB.Phase == Phase.Second ||
                    _lessonA.Phase == Phase.Second && _lessonB.Phase == Phase.All ||
                    _lessonA.Phase == Phase.All && _lessonB.Phase == Phase.Second)
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
                    CfBlock timeBlock = new CfBlock(
                        studyTimeIntersect,
                        GetId(),
                        background,
                        _lessonA.SchoolClassName + " x " + _lessonB.SchoolClassName,
                        item.Key,
                        ScheduleTableItemType.TimeConflict,
                        _lessonA,
                        _lessonB
                    );
                    yield return timeBlock;
                }
            }
        }

        public string GetId()
        {
            return "tc" + ' ' + LessonA.SubjectCode + ' ' + LessonB.SubjectCode;
        }
    }
}
