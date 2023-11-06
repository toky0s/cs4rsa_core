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
        /// <summary>
        /// Lấy thông tin sinh viên bằng file Excel lấy từ Danh sách thi
        /// https://pdaotao.duytan.edu.vn/home/Default.aspx?lang=VN
        /// </summary>
        /// <param name="path">Đường dẫn file.</param>
        /// <returns>Danh sách thông tin sinh viên.</returns>
        public static IEnumerable<Student> Read(string path)
        {
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                // Chỉ lấy những sheet được Visible hoặc có tên khác TONGHOP
                IEnumerable<ExcelWorksheet> sheets = package
                    .Workbook
                    .Worksheets
                    .Where(ws => ws.Hidden == eWorkSheetHidden.Visible && !"TONGHOP".Equals(ws.Name))
                    .ToList();
                foreach (ExcelWorksheet worksheet in sheets)
                {
                    (int sttRow, int sttColumn) = GetRowColumn_STT(worksheet);
                    List<Header> outHeaders = new List<Header>();
                    List<int> outValidCols = new List<int>();
                    GetHeaders(worksheet, sttRow, sttColumn, outHeaders, outValidCols);
                    IEnumerable<Student> students = GetStudents(worksheet, outHeaders, sttRow, outValidCols);
                    foreach (Student student in students)
                    {
                        yield return student;
                    }
                }
            }
        }

        private static IEnumerable<Student> GetStudents(
            ExcelWorksheet worksheet
            , List<Header> headers
            , int sttRow
            , List<int> validCols)
        {
            bool isStartedFetch = false;
            sttRow++;
            while (true)
            {
                if (isStartedFetch && IsEmptyRow(worksheet, sttRow, validCols[0]))
                {
                    break;
                }
                if (IsEmptyRow(worksheet, sttRow, validCols[0]))
                {
                    sttRow++;
                    continue;
                }
                else
                {
                    isStartedFetch = true;
                }

                yield return GetStudent(worksheet, headers, sttRow, validCols);
                sttRow++;
            }
        }

        private static Student GetStudent(
            ExcelWorksheet worksheet
            , List<Header> headers
            , int row
            , List<int> columns)
        {
            Student student = new Student();
            int headerCount = headers.Count;
            string value;
            for (int i = 0; i < headerCount; i++)
            {
                value = worksheet.Cells[row, columns[i]].Value.ToString();
                if (headers[i] == Header.Code)
                {
                    student.Code = value;
                }
                else if (headers[i] == Header.LastName)
                {
                    student.LastName = value;
                }
                else if (headers[i] == Header.FirstName)
                {
                    student.FirstName = value;
                }
                else if (headers[i] == Header.FullName)
                {
                    student.FullName = value;
                }
                else if (headers[i] == Header.MainClass)
                {
                    student.MainClass = value;
                }
                else if (headers[i] == Header.SubjectClass)
                {
                    student.SubjectClass = value;
                }
                else
                {
                    student.Sex = value;
                }
            }
            return student;
        }

        /// <summary>
        /// Kiểm tra đã đến row kết thúc hay chưa.
        /// </summary>
        /// <param name="worksheet">ExcelWorksheet</param>
        /// <param name="row">Row hiện tại</param>
        /// <param name="codeColumnIdx">Column Index mã sinh viên</param>
        /// <returns></returns>
        private static bool IsEmptyRow(ExcelWorksheet worksheet, int row, int codeColumnIdx)
        {
            object cellValue = worksheet.Cells[row, codeColumnIdx].Value;
            if (cellValue is null || cellValue.ToString().Trim().Equals(string.Empty))
            {
                return true;
            }
            return false;
        }

        private static void GetHeaders(
            ExcelWorksheet worksheet
            , int sttRow
            , int sttColumn
            , List<Header> outHeaders
            , List<int> outValidColumns)
        {
            while (true)
            {
                object cellValue = worksheet.Cells[sttRow, sttColumn + 1].Value;
                if (cellValue is null || cellValue.ToString().Trim().Equals(string.Empty))
                {
                    return;
                }

                string text = cellValue.ToString();
                if (text.HeaderMatcher_Is_MSV())
                {
                    outHeaders.Add(Header.Code);
                }
                else if (text.HeaderMatcher_Is_LastName())
                {
                    outHeaders.Add(Header.LastName); 
                }
                else if (text.HeaderMatcher_Is_FirstName())
                {
                    outHeaders.Add(Header.FirstName); 
                }
                else if (text.HeaderMatcher_Is_FullName())
                {
                    outHeaders.Add(Header.FullName); 
                }
                else if (text.HeaderMatcher_Is_SubjectClass())
                {
                    outHeaders.Add(Header.SubjectClass); 
                }
                else if (text.HeaderMatcher_Is_MainClass())
                {
                    outHeaders.Add(Header.MainClass); 
                }
                else if (text.HeaderMatcher_Is_Sex())
                {
                    outHeaders.Add(Header.Sex); 
                }
                else
                {
                    return;
                }

                outValidColumns.Add(sttColumn + 1);
                sttColumn++;
            }
        }

        /// <summary>
        /// Tìm vị trí cell STT trong sheet.
        /// </summary>
        /// <param name="worksheet">ExcelWorksheet</param>
        /// <returns>(Row, Col)</returns>
        /// <exception cref="Exception">STT row cannot be found</exception>
        private static Tuple<int, int> GetRowColumn_STT(ExcelWorksheet worksheet)
        {
            int rows = worksheet.Dimension.Rows;
            int columns = worksheet.Dimension.Columns;
            for (int i = 1; i <= rows; i++)
            {
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
