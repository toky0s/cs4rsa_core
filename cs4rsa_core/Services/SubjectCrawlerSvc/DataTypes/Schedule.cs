using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Services.SubjectCrawlerSvc.DataTypes
{
    public class Schedule
    {
        private readonly Dictionary<DayOfWeek, List<StudyTime>> _scheduleTime;
        public Dictionary<DayOfWeek, List<StudyTime>> ScheduleTime => _scheduleTime;

        public Schedule(Dictionary<DayOfWeek, List<StudyTime>> scheduleTime)
        {
            _scheduleTime = scheduleTime;
        }

        public IEnumerable<DayOfWeek> GetSchoolDays()
        {
            return _scheduleTime.Keys;
        }

        public IEnumerable<StudyTime> GetStudyTimesAtDay(DayOfWeek DayOfWeek)
        {
            return _scheduleTime[DayOfWeek];
        }

        public IEnumerable<Session> GetSessions()
        {
            List<Session> sessions = new();
            foreach (IEnumerable<StudyTime> studyTimes in _scheduleTime.Values)
            {
                sessions.AddRange(studyTimes.Select(studyTime => studyTime.GetSession()));
            }
            return sessions.Distinct();
        }

        public override string ToString()
        {
            List<string> results = new();
            foreach (KeyValuePair<DayOfWeek, List<StudyTime>> item in _scheduleTime)
            {
                string thu = item.Key.ToCs4rsaThu();
                string timesRange = "";
                foreach (StudyTime studyTime in item.Value)
                {
                    timesRange += $"\nTừ {studyTime.StartAsString} đến {studyTime.EndAsString}";
                }
                string result = $"{thu}{timesRange}";
                results.Add(result);
            }
            return string.Join('\n', results.ToArray());
        }
    }

    /// <summary>
    /// Class này bao gồm các phương thức để thao tác với Schedule.
    /// </summary>
    public class ScheduleManipulation
    {

        /// <summary>
        /// Giao các thứ của hai Schedule. Dùng để phát hiện xung đột giữa hai Schedule.
        /// </summary>
        /// <returns>Trả về DayOfWeek mà cả hai Schedule cùng có.</returns>
        public static IEnumerable<DayOfWeek> GetIntersectDate(Schedule schedule1, Schedule schedule2)
        {
            return schedule1.GetSchoolDays().Intersect(schedule2.GetSchoolDays());
        }


        /// <summary>
        /// Gộp danh sách các Schedule lại thành 1.
        /// </summary>
        /// <param name="schedules"></param>
        /// <returns></returns>
        public static Schedule MergeSchedule(IEnumerable<Schedule> schedules)
        {
            Dictionary<DayOfWeek, List<StudyTime>> DayOfWeekStudyTimePairs = new();
            foreach (Schedule item in schedules)
            {
                IEnumerable<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = item.ScheduleTime;
                foreach (KeyValuePair<DayOfWeek, List<StudyTime>> pair in dayAndStudyTimes)
                {
                    if (!DayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                        DayOfWeekStudyTimePairs.Add(pair.Key, pair.Value);
                }
            }
            Schedule schedule = new(DayOfWeekStudyTimePairs);
            return schedule;
        }
    }
}
