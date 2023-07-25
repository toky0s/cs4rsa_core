using System.Text.RegularExpressions;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;
using CwebizAPI.Utils;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.Utils
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
        public static IEnumerable<string> GetSubjectCodes(string text, GetFrom from)
        {
            if (text.Equals("(Không có Môn học Tiên quyết)") ||
                text.Equals("(Không có Môn học Song hành)") ||
                text.Equals("(Không có môn song hành)") ||
                text.Equals("(Không có môn tiên quyết)"))
                yield break;
            // CS 211 - Lập Trình Cơ Sở, IS 301 - Cơ Sở Dữ Liệu
            // CS 366 - L.A.M.P. (Linux, Apache, MySQL, PHP), CS 372 - Quản Trị Mạng, CS 376 - Giới Thiệu An Ninh Mạng
            Regex regex = new("\\((.*?)\\)");
            if (from == GetFrom.Course)
            {
                MatchCollection matchList = regex.Matches(text);
                IEnumerable<string> stringList = matchList.Cast<Match>().Select(match => match.Value);
                List<string> output = new();
                foreach (string item in stringList)
                {
                    int start = item.IndexOf("(", StringComparison.Ordinal) + 1;
                    int length = item.IndexOf(")", StringComparison.Ordinal) - start;
                    string subjectCode = item.Substring(start, length);
                    yield return subjectCode;
                }
            }
            else
            {
                text = regex.Replace(text, string.Empty);
                if (text.Contains(','))
                {
                    string[] textSplits = text.Split(new char[] { ',' });
                    foreach (string slice in textSplits)
                    {
                        string rawSubjectCode = slice.Split(new char[] { '-' })[0];
                        string subjectCode = rawSubjectCode.SuperCleanString();
                        yield return subjectCode;
                    }
                }
                else
                {
                    string rawSubjectCode = text.Split(new char[] { '-' })[0];
                    string subjectCode = rawSubjectCode.SuperCleanString();
                    yield return subjectCode;
                }
            }
        }
    }
}
