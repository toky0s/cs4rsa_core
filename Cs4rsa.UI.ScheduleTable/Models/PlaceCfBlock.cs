using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.UI.ScheduleTable.Interfaces;

using System;

namespace Cs4rsa.UI.ScheduleTable.Models
{
    internal class PlaceCfBlock : TimeBlock
    {
        public PlaceAdjacent PlaceAdjacent { get; }
        public PlaceCfBlock(
            PlaceAdjacent placeAdjacent,
            string id,
            string background,
            string content,
            DayOfWeek dayOfWeek,
            DateTime start,
            DateTime end,
            ScheduleTableItemType scheduleTableItemType)
            : base(
                id,
                background,
                content,
                dayOfWeek,
                start,
                end,
                scheduleTableItemType
            )
        {
            PlaceAdjacent = placeAdjacent;
            Name = "PlaceCfBlock";
        }
    }
}
