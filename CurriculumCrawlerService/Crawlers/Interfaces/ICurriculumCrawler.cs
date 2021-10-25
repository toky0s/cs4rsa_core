using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumCrawlerService.Crawlers.Interfaces
{
    public interface ICurriculumCrawler
    {
        Task<Curriculum> GetCurriculum(string specialString);
    }
}
