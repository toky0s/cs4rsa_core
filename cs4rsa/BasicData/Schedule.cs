using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho các thứ trong một tuần học.
    /// </summary>
    public enum WeekDate
    {
        MONDAY,
        TUSEDAY,
        WEDNESDAY,
        THURDAY,
        FRIDAY,
        SATURDAY,
        SUNDAY
    }

    /// <summary>
    /// Đại diện cho thời gian học của một SchoolClass.
    /// </summary>
    public class Schedule
    {
        private Dictionary<WeekDate, List<StudyTime>> scheduleTime;
        public Dictionary<WeekDate, List<StudyTime>> ScheduleTime { get { return scheduleTime; } }
        
        public Schedule(Dictionary<WeekDate, List<StudyTime>> scheduleTime)
        {
            this.scheduleTime = scheduleTime;
        }
        
        public List<WeekDate> GetSchoolDays()
        {
            return scheduleTime.Keys.ToList();
        }

        public List<StudyTime> GetStudyTimesAtDay(WeekDate weekDate)
        {
            return scheduleTime[weekDate];
        }

        /// <summary>
        /// Tổng thời gian học.
        /// </summary>
        /// <returns></returns>
        public double TotalHours()
        {
            double total = 0;
            foreach(List<StudyTime> studyTimes in scheduleTime.Values)
            {
                foreach(StudyTime studyTime in studyTimes)
                {
                    total += studyTime.TotalHours();
                }
            }
            return total;
        }

        public List<Session> GetSessions()
        {
            List<Session> sessions = new List<Session>();
            foreach(List<StudyTime> studyTimes in scheduleTime.Values)
            {
                sessions.AddRange(studyTimes.Select(studyTime => studyTime.GetSession()).ToList());
            }
            sessions = sessions.Distinct().ToList();
            return sessions;
        }
    }

    /// <summary>
    /// Class này bao gồm các phương thức để thao tác với Schedule.
    /// </summary>
    public class ScheduleManipulation
    {

        /// <summary>
        /// Giao hai các thứ của hai Schedule. Dùng để phát hiện xung đột giữa hai Schedule.
        /// </summary>
        /// <returns>Trả về WeekDate mà cả hai Schedule cùng có.</returns>
        public static List<WeekDate> GetIntersectDate(Schedule schedule1, Schedule schedule2)
        {
            return schedule1.GetSchoolDays().Intersect(schedule2.GetSchoolDays()).ToList();
        }
    }
}
