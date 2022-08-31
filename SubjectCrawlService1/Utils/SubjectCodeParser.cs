using HelperService;

using SubjectCrawlService1.DataTypes.Enums;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// Trình phân tích mã môn.
    /// Trình phân tích này dùng để lấy ra mã môn tiên quyết và môn song hành
    /// trong thông tin của một môn học.
    /// </summary>
    public class SubjectCodeParser
    {
        /// <summary>
        /// Phân tích tất cả các mã môn có trong một chuỗi văn bản.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from">Một enum xác định chuỗi này get từ Course hay là từ MyDTU.</param>
        /// <returns></returns>
        public static List<string> GetSubjectCodes(string text, GetFrom from)
        {
            if (text.Equals("(Không có Môn học Tiên quyết)") ||
                text.Equals("(Không có Môn học Song hành)") ||
                text.Equals("(Không có môn song hành)") ||
                text.Equals("(Không có môn tiên quyết)"))
                return new List<string>();
            //  CS 211 - Lập Trình Cơ Sở, IS 301 - Cơ Sở Dữ Liệu
            // CS 366 - L.A.M.P. (Linux, Apache, MySQL, PHP), CS 372 - Quản Trị Mạng, CS 376 - Giới Thiệu An Ninh Mạng
            Regex regex = new("\\((.*?)\\)");
            if (from == GetFrom.Course)
            {
                MatchCollection matchList = regex.Matches(text);
                IEnumerable<string> stringList = matchList.Cast<Match>().Select(match => match.Value);
                List<string> output = new();
                foreach (string item in stringList)
                {
                    byte start = (byte)(item.IndexOf("(") + 1);
                    byte length = (byte)(item.IndexOf(")") - start);
                    string subjectCode = item.Substring(start, length);
                    output.Add(subjectCode);
                }
                return output;
            }
            else
            {
                text = regex.Replace(text, "");
                if (text.Contains(','))
                {
                    List<string> results = new();
                    string[] textSplits = text.Split(new char[] { ',' });
                    foreach (string slice in textSplits)
                    {
                        string rawSubjectCode = slice.Split(new char[] { '-' })[0];
                        string subjectCode = StringHelper.SuperCleanString(rawSubjectCode);
                        results.Add(subjectCode);
                    }
                    return results;
                }
                else
                {
                    string rawSubjectCode = text.Split(new char[] { '-' })[0];
                    string subjectCode = StringHelper.SuperCleanString(rawSubjectCode);
                    return new List<string>() { subjectCode };
                }
            }
        }
    }
}
