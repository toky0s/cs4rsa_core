using Cs4rsaCommon.Enums;
using Cs4rsaCommon.Models;

namespace Cs4rsaCommon.Interfaces
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
