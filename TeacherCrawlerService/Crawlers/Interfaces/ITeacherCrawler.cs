using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs4rsaDatabaseService.Models;

namespace TeacherCrawlerService.Crawlers.Interfaces
{
    public interface ITeacherCrawler
    {
        Teacher Crawl();
    }
}
