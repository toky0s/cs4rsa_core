using System;
using CourseSearchService.Crawlers;

namespace CourseSearchService
{
    class Program
    {
        static void Main(string[] args)
        {
            CourseCrawler courseCrawler = new CourseCrawler();
            Console.WriteLine(courseCrawler.CurrentYearInfo);
        }
    }
}
