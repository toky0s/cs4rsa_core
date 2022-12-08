using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils.Models;

using System.Collections.Generic;
using System.Windows.Markup;

namespace Cs4rsa.Utils.Interfaces
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
        /// Mô tả:
        ///      Implement phương thức này để xác định khối thời gian sẽ thuộc giai đoạn nào.
        /// 
        /// 
        /// Trả về:
        ///      Phase.First:
        ///          Khối thời gian sẽ được vẽ trên bảng đầu tiên.
        ///          
        ///      Phase.Second:
        ///          Khối thời gian sẽ được vẽ trên bảng thứ hai.
        ///          
        ///      Phase.All:
        ///          Khối thời gian sẽ được vẽ trên cả hai bảng.
        /// </summary>
        Phase GetPhase();

        ScheduleTableItemType GetScheduleTableItemType();

        string GetId();
    }
}
