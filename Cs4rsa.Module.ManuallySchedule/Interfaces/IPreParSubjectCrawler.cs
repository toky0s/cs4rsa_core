using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Interfaces
{
    public interface IPreParSubjectCrawler
    {
        Task<Tuple<IEnumerable<string>, IEnumerable<string>>> Run(string courseId, bool isUseCache);
    }
}
