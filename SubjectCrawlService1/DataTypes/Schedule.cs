using HelperService;
using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubjectCrawlService1.DataTypes
{
    public class Schedule
    {
        private Dictionary<DayOfWeek, List<StudyTime>> _scheduleTime;
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

        /// <summary>
        /// Tổng thời gian học.
        /// </summary>
        /// <returns></returns>
        public double TotalHours()
        {
            double total = 0;
            foreach (IEnumerable<StudyTime> studyTimes in _scheduleTime.Values)
            {
                foreach (StudyTime studyTime in studyTimes)
                {
                    total += studyTime.TotalHours();
                }
            }
            return total;
        }

        public IEnumerable<Session> GetSessions()
        {
            List<Session> sessions = new();
            foreach (IEnumerable<StudyTime> studyTimes in _scheduleTime.Values)
            {
                sessions.AddRange(studyTimes.Select(studyTime => studyTime.GetSession()).ToList());
            }
            return sessions.Distinct();
        }

        /// <summary>
        /// Lấy ra tất cả StudyTime bất kế DayOfWeek.
        /// </summary>
        /// <returns></returns>
        public List<StudyTime> GetStudyTimes()
        {
            List<StudyTime> studyTimes = new List<StudyTime>();
            foreach (List<StudyTime> item in _scheduleTime.Values)
            {
                studyTimes.AddRange(item);
            }
            return studyTimes.Distinct().ToList();
        }

        /// <summary>
        /// Lấy về LearnState xác định bạn học hay rảnh buổi nào đó.
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public LearnState GetLearnState(DayOfWeek dayOfWeek, Enums.Session session)
        {
            if (!_scheduleTime.ContainsKey(dayOfWeek)) return LearnState.Free;
            List<StudyTime> studyTimes = _scheduleTime[dayOfWeek]
                                         .Where(item => item.GetSession() == session).ToList();
            return studyTimes.Count == 0 ? LearnState.Free : LearnState.Learn;
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
        public static List<DayOfWeek> GetIntersectDate(Schedule schedule1, Schedule schedule2)
        {
            return schedule1.GetSchoolDays().Intersect(schedule2.GetSchoolDays()).ToList();
        }


        /// <summary>
        /// Gộp danh sách các Schedule lại thành 1.
        /// </summary>
        /// <param name="schedules"></param>
        /// <returns></returns>
        public static Schedule MergeSchedule(List<Schedule> schedules)
        {
            Dictionary<DayOfWeek, List<StudyTime>> DayOfWeekStudyTimePairs = new();
            foreach (Schedule item in schedules)
            {
                List<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = item.ScheduleTime.ToList();
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
