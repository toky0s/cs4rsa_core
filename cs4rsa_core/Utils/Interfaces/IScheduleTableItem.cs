﻿using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Utils.Models;

using System.Collections.Generic;

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

        ScheduleTableItemType GetScheduleTableItemType();


        /// <summary>
        /// ClassGroupModel: SubjectCode will be ID.
        /// TimeConflict: Time + SubjectCode1 + SubjectCode2
        /// PlaceConflict: Place + SubjectCode1 + SubjectCode2
        /// </summary>
        string GetId();
    }
}
