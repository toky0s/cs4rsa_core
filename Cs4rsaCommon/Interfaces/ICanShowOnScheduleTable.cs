using Cs4rsaCommon.Models;

namespace Cs4rsaCommon.Interfaces
{
    /// <summary>
    /// Buộc phải Implement Interface này nếu muốn hiển thị một
    /// khối thời gian trên ScheduleTable.
    /// </summary>
    public interface ICanShowOnScheduleTable
    {
        IEnumerable<TimeBlock> GetBlocks();
    }
}
