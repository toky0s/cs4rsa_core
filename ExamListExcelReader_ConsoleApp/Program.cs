using System;
using System.Collections.Generic;

namespace ExamListExcelReader_ConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            IEnumerable<int> result = ExamListExcelReader.StudentReader.Read("E:\\DS thi TN T9.xlsx");
            foreach (int value in result)
            {
                Console.WriteLine(value);
            }
        }
    }
}