using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes
{
    public struct StudyWeek
    {
        public readonly int StartWeek;
        public readonly int EndWeek;

        /// <summary>
        /// Một StudyWeek đại diện cho khoảng tuần học của một Lớp.
        /// </summary>
        /// <param name="studyWeek">Một chuỗi như match với pattern ^[1-9]*--[1-9]*$</param>
        public StudyWeek(string studyWeek)
        {
            string[] separatingStrings = { "--" };
            string[] startAndEnd = studyWeek.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            StartWeek = int.Parse(startAndEnd[0]);

            /// MED 362:  Y Học Cổ Truyền
            /// Trường hợp parse: 49--
            /// Không có tuần kết thúc ở một ClassGroup quăng lỗi IndexOutOfRangeException
            /// Phát hiện 19/6/2022 trước ngày đi bảo vệ NCKH cấp trường một hôm
            /// XinTA
            try
            {
                EndWeek = int.Parse(startAndEnd[1]);
            }
            catch
            {
                EndWeek = 0;
            }
        }

        /**
         * Mô tả:
         *      Phân tích tuần bắt đầu và tuần kết thúc để xác định 
         *      được giai đoạn 1, giai đoạn 2 hay là cả hai giai đoạn.
         *  
         *  
         * Trả về:
         *      Delta độ dài 1 Phase = EndWeek - StartWeek >= 7
         *      Delta độ dài 2 Phase = EndWeek - StartWeek >= 14
         *      
         *      Phase.First:
         *          Bắt đầu từ tuần 1 hoặc tuần 18.
         *      
         *      Phase.Second:
         *          Bắt đầu từ tuần 8 hoặc tuần 34.
         *      
         *      Phase.All
         *          Khác hai trường hợp trên thì các trường hợp còn lại được xem là hai giai đoạn.
         */
        public Phase GetPhase()
        {
            bool isOnePhaseDelta = EndWeek - StartWeek >= 7;
            if (StartWeek >= 1 && EndWeek - StartWeek >= 14)
            {
                return Phase.All;
            }
            else if (
                (StartWeek >= 8 || StartWeek >= 34)
                && isOnePhaseDelta
            )
            {
                return Phase.Second;
            }
            else if (
                (StartWeek >= 1 || StartWeek >= 18)
                && isOnePhaseDelta
            )
            {
                return Phase.First;
            }
            return Phase.All;
        }
    }
}
