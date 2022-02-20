using System;

namespace HelperService
{
    public class StringHelper
    {
        public static string CleanString(string text)
        {
            return text.Trim();
        }

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
            return CleanString(text);
        }
    }
}
