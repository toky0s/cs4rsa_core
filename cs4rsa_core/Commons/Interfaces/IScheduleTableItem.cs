using cs4rsa_core.Commons.Enums;
using cs4rsa_core.Commons.Models;

using System.Collections.Generic;

namespace cs4rsa_core.Commons.Interfaces
{
    /// <summary>
    /// Buộc phải triển khai Interface này nếu muốn hiển thị một
    /// khối thời gian trên ScheduleTable.
    /// </summary>
    public interface IScheduleTableItem
    {
        IEnumerable<TimeBlock> GetBlocks();

        object GetValue();

        ContextType GetContextType();

        string GetId();
    }
}
