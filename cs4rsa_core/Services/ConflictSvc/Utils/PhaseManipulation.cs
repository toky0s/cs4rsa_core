using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

namespace Cs4rsa.Services.ConflictSvc.Utils
{
    public class PhaseManipulation
    {
        /// <summary>
        /// [ fStartWeek -- fEndWeek -- sStartWeek -- sEndWeek ]
        /// 
        /// Lấy ra khoảng giao nhau giữa hai cặp tuần.
        /// </summary>
        /// <returns>PhaseIntersect - Phục vụ cho việc phát hiện xung đột.</returns>
        public static PhaseIntersect GetPhaseIntersect(in StudyWeek studyWeekF, in StudyWeek studyWeekS)
        {
            int fStartWeek = studyWeekF.StartWeek;
            int fEndWeek = studyWeekF.EndWeek;
            int sStartWeek = studyWeekS.StartWeek;
            int sEndWeek = studyWeekS.EndWeek;

            // Early exit: Case 8, 9
            if ((fStartWeek > sEndWeek) || (sStartWeek > fEndWeek))
            {
                return PhaseIntersect.NullInstance;
            }
            // Case 2
            else if (fStartWeek > sStartWeek && fEndWeek < sEndWeek)
            {
                return new PhaseIntersect()
                {
                    StartWeek = fStartWeek,
                    EndWeek = fEndWeek
                };
            }
            // Case 3
            else if (sStartWeek > fStartWeek && sEndWeek < fEndWeek)
            {
                return new PhaseIntersect()
                {
                    StartWeek = sStartWeek,
                    EndWeek = sEndWeek
                };
            }
            else if (fStartWeek == sStartWeek)
            {
                // Case 1 and case 4
                if ((fEndWeek == sEndWeek) || (fEndWeek < sEndWeek))
                {
                    return new PhaseIntersect()
                    {
                        StartWeek = fStartWeek,
                        EndWeek = fEndWeek
                    };
                }
                // Case 5
                else
                {
                    return new PhaseIntersect()
                    {
                        StartWeek = sStartWeek,
                        EndWeek = sEndWeek
                    };
                }
            }
            else if (fEndWeek == sEndWeek)
            {
                // Case 7
                if (fStartWeek > sStartWeek)
                {
                    return new PhaseIntersect()
                    {
                        StartWeek = fStartWeek,
                        EndWeek = fEndWeek
                    };
                }
                // Case 6
                else
                {
                    return new PhaseIntersect()
                    {
                        StartWeek = sStartWeek,
                        EndWeek = sEndWeek
                    };
                }
            }
            // Case 10
            else if (fStartWeek > sStartWeek
                && fEndWeek > sEndWeek
                && sEndWeek >= fStartWeek)
            {
                return new PhaseIntersect()
                {
                    StartWeek = fStartWeek,
                    EndWeek = sEndWeek
                };
            }
            // Case 11
            else
            {
                return new PhaseIntersect()
                {
                    StartWeek = sStartWeek,
                    EndWeek = fEndWeek
                };
            }
        }
    }

    public class PhaseIntersect
    {
        public static readonly PhaseIntersect NullInstance = new() { StartWeek = 0, EndWeek = 0 };
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
    }
}
