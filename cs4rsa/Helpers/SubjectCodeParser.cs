using System.Collections.Generic;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// Trình phân tích mã môn.
    /// Trình phân tích này dùng để lấy ra mã môn tiên quyết va môn song hành
    /// trong thông tin của một môn học.
    /// </summary>
    public class SubjectCodeParser
    {
        public static List<string> GetSubjectCodes(string text)
        {
            if (text.Equals("(Không có Môn học Tiên quyết)") ||
                text.Equals("(Không có Môn học Song hành)") ||
                text.Equals("(Không có môn song hành)") ||
                text.Equals("(Không có môn tiên quyết)"))
                return null;
            //  CS 211 - Lập Trình Cơ Sở, IS 301 - Cơ Sở Dữ Liệu
            if (text.Contains(","))
            {
                List<string> results = new List<string>();
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
