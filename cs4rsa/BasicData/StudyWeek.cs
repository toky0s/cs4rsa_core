using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public enum Phase
    {
        NON, FIRST, SECOND, ALL
    }

    public class StudyWeek
    {
        private readonly int startWeek;
        private int endWeek;

        public int StartWeek { get { return startWeek; } }
        public int EndWeek { get { return endWeek; } set { endWeek = value; } }

        /// <summary>
        /// Một StudyWeek đại diện cho khoảng tuần học của một Lớp.
        /// </summary>
        /// <param name="startWeek">Tuần bắt đầu.</param>
        /// <param name="endWeek">Tuần kết thúc.</param>
        public StudyWeek(int startWeek, int endWeek)
        {
            this.startWeek = startWeek;
            this.endWeek = endWeek;
        }

        /// <summary>
        /// Một StudyWeek đại diện cho khoảng tuần học của một Lớp.
        /// </summary>
        /// <param name="studyWeek">Một chuỗi như match với pattern ^[1-9]*--[1-9]*$</param>
        public StudyWeek(string studyWeek)
        {
            string[] separatingStrings = { "--" };
            string[] startAndEnd = studyWeek.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            startWeek = int.Parse(startAndEnd[0]);
            endWeek = int.Parse(startAndEnd[1]);
        }

        public StudyWeek(int startWeek)
        {
            this.startWeek = startWeek;
        }

        /// <summary>
        /// Phương thức này trả về giai đoạn của khoảng tuần này.
        /// </summary>
        public Phase GetPhase()
        {
            if ((startWeek >= 1 && endWeek <= 8) || (startWeek >= 20 && endWeek <= 33))
            {
                return Phase.FIRST;
            }
            if ((startWeek >= 9 && endWeek <= 18) || startWeek >= 34)
            {
                return Phase.SECOND;
            }
            return Phase.ALL;
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
