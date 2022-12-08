using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils.Models;

using System.Collections.Generic;
using System.Windows.Markup;

namespace Cs4rsa.Interfaces
{
    public enum ScheduleTableItemType
    {
        SchoolClass,
        TimeConflict,
        PlaceConflict,
    }

    /**
     * Mô tả:
     *      Buộc phải triển khai Interface này nếu muốn hiển thị một 
     *      khối thời gian trên ScheduleTable.
     */
    public interface IScheduleTableItem
    {
        /// <summary>
        /// Trả về một danh sách khối thời gian sẽ được vẽ trên bảng mô phỏng.
        /// </summary>
        /// <returns><see cref="IEnumerable{TimeBlock}"/></returns>
        IEnumerable<TimeBlock> GetBlocks();

        /// <summary>
        /// Implement phương thức này để xác định khối thời gian sẽ thuộc giai đoạn nào.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        ///     <item>Phase.First: Khối thời gian sẽ được vẽ trên bảng đầu tiên.</item>
        ///     <item>Phase.Second: Khối thời gian sẽ được vẽ trên bảng thứ hai.</item>
        ///     <item>Phase.All: Khối thời gian sẽ được vẽ trên cả hai bảng.</item>
        /// </list>
        /// </returns>
        Phase GetPhase();

        ScheduleTableItemType GetScheduleTableItemType();

        string GetId();
    }
}
