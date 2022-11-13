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
         * FIXME: Xem lại chiến lược xác định giai đoạn học 
         * và phát hiện xung đột thời gian.
         * 
         * Mô tả:
         *      Phân tích tuần bắt đầu và tuần kết thúc để xác định 
         *      được giai đoạn 1, giai đoạn 2 hay là cả hai giai đoạn.
         *      
         *      Tham khảo: http://pdaotao.duytan.edu.vn/quydinh_detail/?id=7&lang=VN#:~:text=M%E1%BB%99t%20n%C4%83m%20h%E1%BB%8Dc%20c%C3%B3%20hai%20h%E1%BB%8Dc%20k%E1%BB%B3%20ch%C3%ADnh%2C%20m%E1%BB%97i%20h%E1%BB%8Dc%20k%E1%BB%B3%20ch%C3%ADnh%20c%C3%B3%20%C3%ADt%20nh%E1%BA%A5t%2015%20tu%E1%BA%A7n%20th%E1%BB%B1c%20h%E1%BB%8Dc%20v%C3%A0%203%20tu%E1%BA%A7n%20thi.
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
