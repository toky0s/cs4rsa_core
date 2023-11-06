using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamListExcelReader
{
    public static class HeaderMatcher
    {
        // Mã số sinh viên match cases
        private static readonly string[] _matchCasesMsv = {
            "msv",
            "mssv",
            "mã sinh viên",
            "mã số sinh viên"
        };
        
        // Số thứ tự match cases
        private static readonly string[] _matchCasesStt = {
            "stt",
            "số thứ tự"
        };

        private static readonly string[] _matchCasesLastName =
        {
            "họ",
            "họ và"
        };
        
        private static readonly string[] _matchCasesFirstName =
        {
            "tên"
        };
        
        private static readonly string[] _matchCasesFullName =
        {
            "họ và tên"
        };

        private static readonly string[] _matchCasesSubjectClass =
        {
            "lớp",
            "lớp môn học"
        };

        private static readonly string[] _matchCasesMainClass =
        {
            "lớp sinh hoạt"
        };

        private static readonly string[] _matchCasesBirthDate =
        {
            "ngày sinh"
        };
        
        private static readonly string[] _matchCasesBornPlace =
        {
            "nơi sinh"
        };
        
        private static readonly string[] _matchCasesSex =
        {
            "giới tính"
        };
        
        private static void ProtectParam(string text)
        {
            if (text is null)
            {
                throw new ArgumentException("text is null");
            }

            if (text.Trim().Equals(string.Empty))
            {
                throw new ArgumentException("text is empty");
            }
        }

        private static bool MatchCase(IEnumerable<string> matchCases, string text)
        {
            text = text.Trim().ToLower();
            return matchCases.Any(matchCase => matchCase.Equals(text));
        }
        
        public static bool HeaderMatcher_Is_MSV(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesMsv, text);
        }
        
        public static bool HeaderMatcher_Is_FirstName(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesFirstName, text);
        }
        
        public static bool HeaderMatcher_Is_LastName(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesLastName, text);
        }
        
        public static bool HeaderMatcher_Is_FullName(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesFullName, text);
        }
        
        public static bool HeaderMatcher_Is_BirthDate(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesBirthDate, text);
        }
        
        public static bool HeaderMatcher_Is_BornPlace(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesBornPlace, text);
        }
        
        public static bool HeaderMatcher_Is_Sex(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesSex, text);
        }
        
        public static bool HeaderMatcher_Is_SubjectClass(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesSubjectClass, text);
        }

        public static bool HeaderMatcher_Is_MainClass(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesMainClass, text);
        }

        public static bool HeaderMatcher_Is_STT(this string text)
        {
            ProtectParam(text);
            return MatchCase(_matchCasesStt, text);
        }
    }
}