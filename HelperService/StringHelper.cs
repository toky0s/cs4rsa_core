using System;
using System.Text;

namespace HelperService
{
    public class StringHelper
    {
        public static string[] SplitAndRemoveAllSpace(string text)
        {
            char[] separatingStrings = { ' ', '\n', '\r' };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitAndRemoveNewLine(string text)
        {
            char[] separatingStrings = { '\n', '\r' };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string SuperCleanString(string text)
        {
            char[] separatingStrings = { ' ', '\n', '\r' };
            string[] sliceStrings = text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            string ouput = string.Join(" ", sliceStrings);
            return ouput;
        }

        public static string ParseDateTime(string text)
        {
            text = text.Replace("\r\n", string.Empty);
            return text.Trim();
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAints = Encoding.UTF8.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAints);
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAints = Convert.FromBase64String(encodedData);
            return Encoding.UTF8.GetString(encodedDataAints);
        }
    }
}
