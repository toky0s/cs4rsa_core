using SubjectCrawlService1.DataTypes.Enums;

namespace SubjectCrawlService1.DataTypes
{
    public class StudyWeek
    {
        private readonly byte startWeek;
        private readonly byte endWeek;

        public byte StartWeek { get { return startWeek; } }
        public byte EndWeek { get { return endWeek; } }

        /// <summary>
        /// Một StudyWeek đại diện cho khoảng tuần học của một Lớp.
        /// </summary>
        /// <param name="studyWeek">Một chuỗi như match với pattern ^[1-9]*--[1-9]*$</param>
        public StudyWeek(string studyWeek)
        {
            string[] separatingStrings = { "--" };
            string[] startAndEnd = studyWeek.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            startWeek = byte.Parse(startAndEnd[0]);

            /// MED 362:  Y Học Cổ Truyền
            /// Trường hợp parse: 49--
            /// Không có tuần kết thúc ở một ClassGroup quăng lỗi IndexOutOfRangeException
            /// Phát hiện 19/6/2022 trước ngày đi bảo vệ NCKH cấp trường một hôm
            /// XinTA
            try
            {
                endWeek = byte.Parse(startAndEnd[1]);
            }
            catch
            {
                endWeek = 0;
            }
        }

        /// <summary>
        /// Phương thức này trả về giai đoạn của khoảng tuần này.
        /// </summary>
        public Phase GetPhase()
        {
            if (startWeek >= 34 && endWeek == 0)
            {
                return Phase.Second;
            }
            if ((startWeek >= 1 && endWeek <= 8) || (startWeek >= 20 && endWeek <= 33))
            {
                return Phase.First;
            }
            if ((startWeek >= 9 && endWeek <= 18) || startWeek >= 34)
            {
                return Phase.Second;
            }
            return Phase.All;
        }

        /// <summary>
        /// Trả về số tuần phải học.
        /// </summary>
        public int GetStudyNumberOfWeeks()
        {
            return endWeek - startWeek + 1;
        }
    }
}
