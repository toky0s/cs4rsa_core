using System.Collections.Generic;

namespace cs4rsa_core.Models.Interfaces
{
    public interface ICanShowOnScheduleTable
    {
        List<TimeBlock> GetBlocks();
    }
}
