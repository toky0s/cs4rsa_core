using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes
{
    public class StudyWeek
    {
        public int StartWeek { get; private set; }
        public int EndWeek { get; private set; }

        /// <summary>
        /// Một StudyWeek đại diện cho khoảng tuần học của một Lớp.
        /// </summary>
        /// <param name="studyWeek">Một chuỗi như match với pattern ^[1-9]*--[1-9]*$</param>
        public StudyWeek(string studyWeek)
        {
            string[] separatingStrings = { "--" };
            string[] startAndEnd = studyWeek.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            StartWeek = int.TryParse(startAndEnd[0], out int result) ? result : 0;

            // MED 362:  Y Học Cổ Truyền
            // Trường hợp parse: 49--
            // Không có tuần kết thúc ở một ClassGroup quăng lỗi IndexOutOfRangeException
            // Phát hiện 19/6/2022 trước ngày đi bảo vệ NCKH cấp trường một hôm
            // XinTA
            try
            {
                EndWeek = int.Parse(startAndEnd[1]);
            }
            catch
            {
                EndWeek = 0;
            }
        }

        /// <summary>
        /// Tuỳ thuộc vào điểm giữa hiện tại của Phase Store,
        /// Giai đoạn sẽ được xác định tương ứng.
        /// 
        /// BetweenPoint là tuần kết thúc của giai đoạn 1.
        /// </summary>
        public Phase GetPhase()
        {
            if (IsSummerSmt(StartWeek)) return Phase.Summer;
            if (IsFirstSmtAllPhase(StartWeek, EndWeek) || IsSecondSmtAllPhase(StartWeek, EndWeek)) return Phase.All;
            if (IsFirstSmtFirstPhase(StartWeek, EndWeek) || IsSecondSmtFirstPhase(StartWeek, EndWeek))
                return Phase.First;
            if (IsFirstSmtSecondPhase(StartWeek, EndWeek) || IsSecondSmtSecondPhase(StartWeek, EndWeek))
                return Phase.Second;
            return Phase.Unknown;
        }

        /// <summary>
        /// Lấy ra số tuần học.
        /// </summary>
        /// <param name="start">Tuần bắt đầu.</param>
        /// <param name="end">Tuần kết thúc.</param>
        /// <returns>Số tuần học.</returns>
        /// <exception cref="ArgumentException">Không xác định được tuần bắt đầu hoặc tuần kết thúc.</exception>
        private static int GetLearnWeekAmount(int start, int end)
        {
            if (start == 0) throw new ArgumentException("Không xác định được tuần bắt đầu");
            if (end == 0) throw new ArgumentException("Không xác định được tuần kết thúc");
            return end - start + 1;
        }
        
        #region First semester

        private static bool IsFirstSmtFirstPhase(int start, int end)
        {
            int lwAmount = GetLearnWeekAmount(start, end);
            return lwAmount is >= 5 and < 10 && start == 1;
        }

        private static bool IsFirstSmtSecondPhase(int start, int end)
        {
            return !IsFirstSmtFirstPhase(start, end);
        }
        
        private static bool IsFirstSmtAllPhase(int start, int end)
        {
            return GetLearnWeekAmount(start, end) >= 10;
        }
        
        #endregion

        #region Second Semester

        private static bool IsSecondSmtFirstPhase(int start, int end)
        {
            int[] startWeeks = { 27, 28 };
            return startWeeks.Contains(start) && GetLearnWeekAmount(start, end) < 10;
        }
        
        private static bool IsSecondSmtSecondPhase(int start, int end)
        {
            return !IsSecondSmtFirstPhase(start, end);
        }
        
        private static bool IsSecondSmtAllPhase(int start, int end)
        {
            return GetLearnWeekAmount(start, end) >= 10;
        }

        #endregion

        #region Summer semester
        
        private static bool IsSummerSmt(int start)
        {
            return start >= 43;
        }
        
        #endregion
        
    }
}
