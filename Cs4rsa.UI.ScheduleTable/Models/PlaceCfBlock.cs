using Cs4rsa.Interfaces;
using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.UI.ScheduleTable.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Models
{
    internal class PlaceCfBlock : TimeBlock
    {
        public PlaceAdjacent PlaceAdjacent { get; }
        public PlaceCfBlock(
            PlaceAdjacent placeAdjacent
            , string id
            , string background
            , string content
            , DayOfWeek dayOfWeek
            , DateTime start
            , DateTime end
            , ScheduleTableItemType scheduleTableItemType) 
            : base(
                id
                , background
                , content
                , dayOfWeek
                , start
                , end
                , scheduleTableItemType
            )
        {
            PlaceAdjacent = placeAdjacent;
            Name = "PlaceCfBlock";
        }
    }
}
