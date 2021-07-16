using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using cs4rsa.Enums;

namespace cs4rsa.BasicData
{

    /// <summary>
    /// Đại diện cho thời gian học của một SchoolClass.
    /// </summary>
    public class Schedule
    {
        private Dictionary<DayOfWeek, List<StudyTime>> scheduleTime;
        public Dictionary<DayOfWeek, List<StudyTime>> ScheduleTime { get { return scheduleTime; } }
        
        public Schedule(Dictionary<DayOfWeek, List<StudyTime>> scheduleTime)
        {
            this.scheduleTime = scheduleTime;
        }
        
        public List<DayOfWeek> GetSchoolDays()
        {
            return scheduleTime.Keys.ToList();
        }

        public List<StudyTime> GetStudyTimesAtDay(DayOfWeek DayOfWeek)
        {
            return scheduleTime[DayOfWeek];
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

        /// <summary>
        /// Lấy ra tất cả StudyTime bất kế DayOfWeek.
        /// </summary>
        /// <returns></returns>
        public List<StudyTime> GetStudyTimes()
        {
            List<StudyTime> studyTimes = new List<StudyTime>();
            foreach (List<StudyTime> item in scheduleTime.Values)
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
        public LearnState GetLearnState(DayOfWeek dayOfWeek, Session session)
        {
            if (!scheduleTime.ContainsKey(dayOfWeek)) return LearnState.Free;
            List<StudyTime> studyTimes = scheduleTime[dayOfWeek]
                                         .Where(item => item.GetSession()==session).ToList();
            return studyTimes.Count == 0 ? LearnState.Free : LearnState.Learn;
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
            Dictionary<DayOfWeek, List<StudyTime>> DayOfWeekStudyTimePairs = new Dictionary<DayOfWeek, List<StudyTime>>();
            foreach(Schedule item in schedules)
            {
                List<KeyValuePair<DayOfWeek, List<StudyTime>>> dayAndStudyTimes = item.ScheduleTime.ToList();
                foreach (KeyValuePair<DayOfWeek, List<StudyTime>> pair in dayAndStudyTimes)
                {
                    if (!DayOfWeekStudyTimePairs.ContainsKey(pair.Key))
                        DayOfWeekStudyTimePairs.Add(pair.Key, pair.Value);
                }
            }
            Schedule schedule = new Schedule(DayOfWeekStudyTimePairs);
            return schedule;
        }
    }
}
