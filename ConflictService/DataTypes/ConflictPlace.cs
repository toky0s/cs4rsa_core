using System;
using System.Collections.Generic;

namespace ConflictService.DataTypes
{
    public class ConflictPlace
    {
        public Dictionary<DayOfWeek, IEnumerable<PlaceAdjacent>> PlaceAdjacents { get; private set; }

        public ConflictPlace(Dictionary<DayOfWeek, IEnumerable<PlaceAdjacent>> placeAdjacents)
        {
            PlaceAdjacents = placeAdjacents;
        }
    }
}
