using Cs4rsa.Commons.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System.Collections.Generic;

namespace Cs4rsa.Commons.Interfaces
{
    /**
     * Mô tả:
     *      Buộc phải triển khai Interface này nếu muốn hiển thị một 
     *      khối thời gian trên ScheduleTable.
     */
    public interface IScheduleTableItem
    {
        /**
         * Mô tả:
         *      Trả về một danh sách khối thời gian sẽ được vẽ trên bảng mô phỏng.
         */
        IEnumerable<TimeBlock> GetBlocks();

        /**
         * Mô tả:
         *      Implement phương thức này để xác định khối thời gian sẽ thuộc giai đoạn nào.
         * 
         * 
         * Trả về:
         *      Phase.First:
         *          Khối thời gian sẽ được vẽ trên bảng đầu tiên.
         *          
         *      Phase.Second:
         *          Khối thời gian sẽ được vẽ trên bảng thứ hai.
         *          
         *      Phase.All:
         *          Khối thời gian sẽ được vẽ trên cả hai bảng.
         */
        Phase GetPhase();
    }
}
