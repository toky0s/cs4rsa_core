using ExamListExcelReader.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamListExcelReader_ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            IEnumerable<Student> result = ExamListExcelReader.StudentReader
                .Read("C:\\Users\\truon\\Downloads\\20231027_18h00_ENG422_DICH THUAT VAN CHUONG.xlsx");
            var orderResult = result.OrderBy(r => r.Code);
            foreach (var item in orderResult)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
    }
}