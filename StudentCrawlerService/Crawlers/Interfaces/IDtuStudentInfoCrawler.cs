using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentCrawlerService.Crawlers.Interfaces
{
    public interface IDtuStudentInfoCrawler
    {
        Task<Student> Crawl(string specialString);
    }
}
