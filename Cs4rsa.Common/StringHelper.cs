/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cs4rsa.Infrastructure.Common
{
    /// <summary>
    /// Lớp tiện ích cho string.
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public static class StringHelper
    {
        public static string[] SplitAndRemoveAllSpace(this string text)
        {
            char[] separatingStrings = { ' ', '\n', '\r' };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> SplitAndRemoveNewLine(string text)
        {
            char[] separatingStrings = { '\n', '\r' };
            return text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => item.Trim());
        }

        public static string SuperCleanString(this string text)
        {
            char[] separatingStrings = { ' ', '\n', '\r' };
            var sliceStrings = text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", sliceStrings);
        }

        public static string ParseDateTime(string text)
        {
            text = text.Replace("\r\n", string.Empty);
            return text.Trim();
        }

        public static string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string DecodeFrom64(string encodedData)
        {
            var encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        public static string ReplaceVietnamese(string text)
        {
            var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            var temp = text.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, string.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        /// <summary>
        /// Tách các Email từ một chuỗi.
        /// </summary>
        /// <param name="text">Chuỗi đầu vào.</param>
        /// <returns>Danh sách Email.</returns>
        public static IEnumerable<string> GetEmails(this string text)
        {
            var emailRegex = new Regex(@"[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*");
            var matchCollection = emailRegex.Matches(text);
            foreach (Match m in matchCollection)
            {
                yield return m.Value;
            }
        }
    }
}
