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

        public static string CacheGenAddStyle(string cache)
        {
            return @"
                <style>
                    body {
                    padding: 2% 7%;
                    background-color: #ffffee;
                    font-family: 'Segoe UI', Tahoma, Verdana, sans-serif, Tahoma, sans-serif;
                }

                #ctdt-title > table {
                    width: 100%;
                }

                .ico-namnganhhoc {
                    background-color: aqua;
                    border-radius: 10px;
                    padding: 20px 30px;
                    font-weight: bold;
                    font-size: x-large;

                }


                #ctdt-title::before {
                    content: 'CS4RSA - All rights reserved | Tệp tin này được tạo tự động từ ứng dụng CS4RSA';
                    font-size: 10px;
                    font-style: italic;
                    color: gray;
                }

                body > div.calendar::after {
                    content: 'CS4RSA - All rights reserved | Tệp tin này được tạo tự động từ ứng dụng CS4RSA';
                    font-size: 10px;
                    font-style: italic;
                    color: gray;
                }

                body > table > tbody > tr:nth-child(2) > td > table {
                    background-color: antiquewhite;
                    padding: 10px;
                    border-radius: 10px;
                    border: 1px solid black;
                }

                /* CS 311:  Lập Trình Hướng Đối Tượng */
                /* Lịch học của Môn học: */
                .title-1 {
                    background-color: aquamarine;
                    border-radius: 10px;
                    padding: 10px 20px;
                    font-weight: bold;
                    font-size: large;
                    font-family: Verdana, Tahoma, sans-serif;
                    margin-bottom: 5px;
                }

                .tb_coursedetail {
                    margin-bottom: 10px;
                }

                .tb_coursedetail .info {
                   width: 200px;
                   font-weight: bold;
                }


                body > div.calendar > table {
                    width: 100%;
                    border-radius: 10px;
                    border: 1px solid black;
                    padding: 5px;
                    background-color: antiquewhite;
                }

                .nhom-lop {
                    background-color: rgb(223, 180, 174);
                    padding: 5px;
                    border-radius: 10px;
                    font-weight: bold;
                }

                .nhom-lop div::before {
                    content: 'Nhóm lớp';
                }

                a {
                    text-decoration: none;
                }
                </style>
                "
            + cache;
        }
    }
}
