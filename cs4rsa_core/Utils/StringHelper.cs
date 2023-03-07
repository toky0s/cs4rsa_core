using Cs4rsa.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs4rsa.Utils
{
    public abstract class StringHelper
    {
        public static string[] SplitAndRemoveAllSpace(string text)
        {
            char[] separatingStrings = { VmConstants.CharSpace, '\n', '\r' };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> SplitAndRemoveNewLine(string text)
        {
            char[] separatingStrings = { '\n', '\r' };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => item.Trim());
        }

        public static string SuperCleanString(string text)
        {
            char[] separatingStrings = { VmConstants.CharSpace, '\n', '\r' };
            string[] sliceStrings = text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(VmConstants.StrSpace, sliceStrings);
        }

        public static string ParseDateTime(string text)
        {
            text = text.Replace("\r\n", string.Empty);
            return text.Trim();
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return Encoding.UTF8.GetString(encodedDataAsBytes);
        }
    }
}
