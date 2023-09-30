using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExamListExcelReader.Models;
using OfficeOpenXml;

namespace ExamListExcelReader
{
    public static class StudentReader
    {
        public static IEnumerable<Student> Read(string path)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    
            using(ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                // Chỉ lấy những sheet được show
                foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets.Where(ws => ws.Hidden == eWorkSheetHidden.Visible))
                {
                    (int sttRow, int sttColumn) = GetRowColumn_STT(worksheet);
                    List<Header> headers = GetHeaders(worksheet, sttRow, sttColumn);
                    
                }
            }
        }

        private static List<Header> GetHeaders(ExcelWorksheet worksheet, int sttRow, int sttColumn)
        {
            List<Header> headers = new List<Header>();
            while (true)
            {
                object cellValue = worksheet.Cells[sttRow, sttColumn + 1].Value;
                if (cellValue is null || cellValue.ToString().Trim().Equals(string.Empty))
                {
                    return headers;
                }

                string text = cellValue.ToString();
                if (text.HeaderMatcher_Is_MSV())
                {
                    headers.Add(Header.Code);
                }
                else if (text.HeaderMatcher_Is_LastName())
                {
                    headers.Add(Header.LastName);
                }
                else if (text.HeaderMatcher_Is_FirstName())
                {
                    headers.Add(Header.FirstName);
                }
                else if (text.HeaderMatcher_Is_FullName())
                {
                    headers.Add(Header.FullName);
                }
                else if (text.HeaderMatcher_Is_BirthDate())
                {
                    headers.Add(Header.BirthDate);
                }
                else if (text.HeaderMatcher_Is_BornPlace())
                {
                    headers.Add(Header.BornPlace);
                }
                else if (text.HeaderMatcher_Is_Class())
                {
                    headers.Add(Header.Class);
                }
                else
                {
                    headers.Add(Header.Sex);
                }

                sttColumn++;
            }
        }

        private static Tuple<int, int> GetRowColumn_STT(ExcelWorksheet worksheet)
        {
            // Console.WriteLine(worksheet.Name);
            int rows = worksheet.Dimension.Rows;
            int columns = worksheet.Dimension.Columns;
            // loop through the worksheet rows and columns
            for (int i = 1; i <= rows; i++) {
                for (int j = 1; j <= columns; j++)
                {
                    if (worksheet.Cells[i, j].Value is null) continue;
                    string content = worksheet.Cells[i, j].Value.ToString();
                    if (content.HeaderMatcher_Is_STT())
                    {
                        return new Tuple<int, int>(i, j);
                    }
                }
            }

            throw new Exception("STT row cannot be found");
        }
        
    }
}