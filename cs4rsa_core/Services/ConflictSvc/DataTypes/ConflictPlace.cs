using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.DataTypes
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
