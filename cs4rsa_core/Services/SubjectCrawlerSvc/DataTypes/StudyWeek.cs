using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.ViewModels.Interfaces;

using System.Windows;

namespace Cs4rsa.Services.SubjectCrawlerSvc.DataTypes
{
    public readonly struct StudyWeek
    {
        public readonly int StartWeek { get; }
        public readonly int EndWeek { get; }

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

        /// <summary>
        /// Tuỳ thuộc vào điểm giữa hiện tại của Phase Store,
        /// Giai đoạn sẽ được xác định tương ứng.
        /// 
        /// BetweenPoint là tuần kết thúc của giai đoạn 1.
        /// </summary>
        public Phase GetPhase()
        {
            IPhaseStore phaseStore = (IPhaseStore)((App)Application.Current).Container.GetService(typeof(IPhaseStore));
            int betweenPointValue = phaseStore.BetweenPoint;
            if (EndWeek <= betweenPointValue)
            {
                return Phase.First;
            }
            else if (StartWeek > betweenPointValue)
            {
                return Phase.Second;
            }
            else if (StartWeek <= betweenPointValue && EndWeek > betweenPointValue)
            {
                return Phase.All;
            }
            else
            {
                return Phase.Non;
            }
        }
    }
}
