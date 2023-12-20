using System;
using System.Collections.Generic;

namespace Cs4rsa.Service.Conflict.DataTypes
{
    public class ConflictPlace
    {
        public readonly Dictionary<DayOfWeek, IEnumerable<PlaceAdjacent>> PlaceAdjacents;

        public ConflictPlace(Dictionary<DayOfWeek, IEnumerable<PlaceAdjacent>> placeAdjacents)
        {
            PlaceAdjacents = placeAdjacents;
        }
    }
}
