using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Models.Interfaces
{
    public interface ICanShowOnScheduleTable
    {
        List<TimeBlock> GetBlocks();
    }
}
