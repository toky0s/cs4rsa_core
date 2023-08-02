using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes
{
    public class StudyTime
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public string StartAsString { get; }
        public string EndAsString { get; }

        public StudyTime(string start, string end)
        {
            StartAsString = start;
            EndAsString = end;
            Start = DateTime.ParseExact(start, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            End = DateTime.ParseExact(end, "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        public Session GetSession()
        {
            if (IsInMorning()) return Session.Morning;
            return IsInAfternoon() ? Session.Afternoon : Session.Night;
        }


        /// <summary>
        /// Kiểm tra đây có phải là buổi sáng hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInMorning()
        {
            DateTime[] morningTime =  {
                DateTime.ParseExact("07:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("11:15", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            return Start <= morningTime[1];
        }

        /// <summary>
        /// Kiểm tra đây có phải là buổi chiều hay không.
        /// </summary>
        /// <returns></returns>
        private bool IsInAfternoon()
        {
            DateTime[] afternoonTime =  {
                DateTime.ParseExact("13:00", "HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                DateTime.ParseExact("17:15", "HH:mm", System.Globalization.CultureInfo.InvariantCulture)
            };
            return Start >= afternoonTime[0] && Start <= afternoonTime[1];
        }
    }
}
