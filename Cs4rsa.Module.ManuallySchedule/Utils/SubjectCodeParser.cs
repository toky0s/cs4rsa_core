using Cs4rsa.Infrastructure.Common;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cs4rsa.Module.ManuallySchedule.Utils
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
            var regex = new Regex("\\((.*?)\\)");
            if (from == GetFrom.Course)
            {
                var matchList = regex.Matches(text);
                var stringList = matchList.Cast<Match>().Select(match => match.Value);
                var output = new List<string>();
                foreach (var item in stringList)
                {
                    var start = item.IndexOf("(") + 1;
                    var length = item.IndexOf(")") - start;
                    var subjectCode = item.Substring(start, length);
                    yield return subjectCode;
                }
            }
            else
            {
                text = regex.Replace(text, string.Empty);
                if (text.Contains(','))
                {
                    var textSplits = text.Split(new char[] { ',' });
                    foreach (var slice in textSplits)
                    {
                        var rawSubjectCode = slice.Split(new char[] { '-' })[0];
                        var subjectCode = StringHelper.SuperCleanString(rawSubjectCode);
                        yield return subjectCode;
                    }
                }
                else
                {
                    var rawSubjectCode = text.Split(new char[] { '-' })[0];
                    var subjectCode = StringHelper.SuperCleanString(rawSubjectCode);
                    yield return subjectCode;
                }
            }
        }
    }
}
