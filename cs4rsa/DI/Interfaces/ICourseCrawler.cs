using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.DI.Interfaces
{
    public interface ICourseCrawler
    {
        string GetCurrentSemesterValue();
        string GetCurrentSemesterInfo();
        string GetCurrentYearValue();
        string GetCurrentYearInfo();
    }
}
