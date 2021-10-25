using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            string[] separatingStrings = { " ", "\n", "\r" };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitAndRemoveNewLine(string text)
        {
            string[] separatingStrings = { "\n", "\r" };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string SuperCleanString(string text)
        {
            string[] separatingStrings = { " ", "\n", "\r" };
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
