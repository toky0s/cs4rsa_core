using System;
using System.Collections.Generic;

namespace ConflictService.DataTypes
{
    public class ConflictPlace
    {
        public Dictionary<DayOfWeek, List<PlaceAdjacent>> PlaceAdjacents { get; private set; } = new Dictionary<DayOfWeek, List<PlaceAdjacent>>();

        public ConflictPlace(Dictionary<DayOfWeek, List<PlaceAdjacent>> placeAdjacents)
        {
            PlaceAdjacents = placeAdjacents;
        }
    }
}
