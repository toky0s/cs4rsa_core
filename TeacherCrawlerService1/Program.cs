using System;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using TeacherCrawlerService1.Crawlers;

namespace TeacherCrawlerService1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Cs4rsaDbContext cs4RsaDbContext = new Cs4rsaDbContext())
            {
                cs4RsaDbContext.Database.EnsureCreated();
                string url = @"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan=71&intructorid=221111108&classid=132070&academicleveltypeid=&curriculumid=";
                TeacherCrawler teacherCrawler = new TeacherCrawler(url, cs4RsaDbContext);
                Teacher teacher = teacherCrawler.Crawl();
                Console.WriteLine(teacher.Name);
            }
        }
    }
}
