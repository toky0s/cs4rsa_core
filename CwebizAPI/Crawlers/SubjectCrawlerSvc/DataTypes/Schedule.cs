using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.Utils;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes
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

        public IEnumerable<StudyTime> GetStudyTimesAtDay(DayOfWeek DayOfWeek)
        {
            return ScheduleTime[DayOfWeek];
        }

        public IEnumerable<Session> GetSessions()
        {
            List<Session> sessions = new();
            foreach (IEnumerable<StudyTime> studyTimes in ScheduleTime.Values)
            {
                sessions.AddRange(studyTimes.Select(studyTime => studyTime.GetSession()));
            }
            return sessions.Distinct();
        }

        public override string ToString()
        {
            List<string> results = new();
            foreach (KeyValuePair<DayOfWeek, List<StudyTime>> item in ScheduleTime)
            {
                string thu = item.Key.ToCs4rsaThu();
                string timesRange = string.Empty;
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
