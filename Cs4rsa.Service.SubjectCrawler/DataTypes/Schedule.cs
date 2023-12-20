using System;
using System.Collections.Generic;
using System.Linq;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.SubjectCrawler.Utils;

namespace Cs4rsa.Service.SubjectCrawler.DataTypes
{
    public class Schedule
    {
        public Dictionary<DayOfWeek, List<StudyTime>> ScheduleTime { get; }

        public Schedule(Dictionary<DayOfWeek, List<StudyTime>> scheduleTime)
        {
            ScheduleTime = scheduleTime;
        }

        public IEnumerable<DayOfWeek> GetSchoolDays()
        {
            return ScheduleTime.Keys;
        }

        public IEnumerable<StudyTime> GetStudyTimesAtDay(DayOfWeek dayOfWeek)
        {
            return ScheduleTime[dayOfWeek];
        }

        public IEnumerable<Session> GetSessions()
        {
            var sessions = new List<Session>();
            foreach (IEnumerable<StudyTime> studyTimes in ScheduleTime.Values)
            {
                sessions.AddRange(studyTimes.Select(studyTime => studyTime.Session));
            }
            return sessions.Distinct();
        }

        public override string ToString()
        {
            var results = (
                from item in ScheduleTime 
                let thu = item.Key.ToCs4rsaVietnamese() 
                let timesRange = item.Value.Aggregate(
                    string.Empty, 
                    (current, studyTime) => current + $"\nTừ {studyTime.StartAsString} đến {studyTime.EndAsString}") 
                select $"{thu}{timesRange}"
            ).ToList();
            return string.Join("\n", results);
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
            var dayOfWeekStudyTimePairs = new Dictionary<DayOfWeek, List<StudyTime>>();
            foreach (var item in schedules)
            {
                IEnumerable<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = item.ScheduleTime;
                foreach (var pair in dayAndStudyTimes)
                {
                    if (!dayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                    {
                        dayOfWeekStudyTimePairs[pair.Key] = pair.Value;
                    }
                    // dayOfWeekStudyTimePairs.TryAdd(pair.Key, pair.Value);
                }
            }
            var schedule = new Schedule(dayOfWeekStudyTimePairs);
            return schedule;
        }
    }
}
