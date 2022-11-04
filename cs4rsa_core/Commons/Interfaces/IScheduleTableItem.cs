using cs4rsa_core.Commons.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

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
        Phase GetPhase();
    }
}
