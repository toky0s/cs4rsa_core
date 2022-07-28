using OfficeOpenXml;

using PdtService.DataTypes;
using PdtService.Implements;
using PdtService.Interfaces;

using System.Text.RegularExpressions;

namespace PdtService
{
    /// <summary>
    /// Module này hỗ trợ việc thao tác với trang PhongDaoTao của DTU
    /// liên quan tới các tác vụ tải excel và thu thập thông tin sinh viên
    /// 
    /// Author: AXin
    /// Date: 2022/07/23
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Console.WriteLine("Hello, World!");
            IPdtExcelDownloader pdtExcelDownloader = new PdtExcelDownloader();
            var url = "http://pdaotao.duytan.edu.vn/uploads/Exam/20220702_15h30_KOR201_HAN%20NGU%20TRUNG%20CAP%201.xlsx";
            var filePath = "C:\\Users\\truon\\Desktop\\cs4rsa_excels\\file.xlsx";
            Task<string> task = pdtExcelDownloader.DownloadExcel(url, filePath);
            task.Wait();

            FileInfo fileInfo = new(filePath);
            if (!fileInfo.Exists)
            {
                Console.WriteLine("File not exist");
            }

            using ExcelPackage package = new(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            

            PdtExcelParser pdtExcelParser = new();

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
                            Console.WriteLine(actualValue);

                    }
                }
                
            }

        }

    }
}