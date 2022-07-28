using OfficeOpenXml;

using PdtService.Commons;
using PdtService.DataTypes;
using PdtService.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PdtService.Implements
{
    internal class PdtExcelParser : IPdtExcelParser
    {
        public IEnumerable<StudentRecord> Get(string excelFilePath)
        {
            //FileInfo fileInfo = new(excelFilePath);
            //if (!fileInfo.Exists)
            //{
            //    return new List<StudentRecord>();
            //}
            //using ExcelPackage package = new(fileInfo);
            //ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            //// Get max rows and cols
            //int rowCount = worksheet.Dimension.Rows;
            //int colCount = worksheet.Dimension.Columns;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Tìm ra vị trí hàng chứa một trong các Header
        /// STT, MSV, HỌ VÀ, TÊN, LỚP MÔN HỌC, LỚP SINH HOẠT
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PdtExcelOrderedColumns> FindBaseRowLocation(ExcelWorksheet worksheet)
        {
            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;
            for (int iRow = 1; iRow <= rowCount; iRow++)
            {
                PdtExcelOrderedColumns pdtExcelOrderedColumns = new(iRow);
                for (int iCol = 1; iCol <= colCount; iCol++)
                {
                    ExcelRange excelRange = worksheet.Cells[iRow, iCol];
                    if (excelRange.Value != null)
                    {
                        string actualValue = excelRange.Text.Trim();
                        Regex regex = new Regex("\\d{6,}");
                        if (actualValue.Length == 11)
                        {
                            Console.WriteLine(actualValue);
                        }
                        if (BaseCells.STT.Equals(actualValue))
                        {
                            pdtExcelOrderedColumns.SetColIndex(excelRange.Start.Column, BaseCells.STT);
                        }
                        else if (BaseCells.MSV.Equals(actualValue))
                        {
                            pdtExcelOrderedColumns.SetColIndex(excelRange.Start.Column, BaseCells.MSV);
                        }
                        else if (BaseCells.FIRST_NAME.Equals(actualValue))
                        {
                            pdtExcelOrderedColumns.SetColIndex(excelRange.Start.Column, BaseCells.FIRST_NAME);
                        }
                        else if (BaseCells.LAST_NAME.Equals(actualValue))
                        {
                            pdtExcelOrderedColumns.SetColIndex(excelRange.Start.Column, BaseCells.LAST_NAME);
                        }
                        else if (BaseCells.CLASS.Equals(actualValue))
                        {
                            pdtExcelOrderedColumns.SetColIndex(excelRange.Start.Column, BaseCells.CLASS);
                        }
                    }
                }
                if (pdtExcelOrderedColumns.IsValid)
                {
                    yield return pdtExcelOrderedColumns;
                }
            }
        }
    }
}
