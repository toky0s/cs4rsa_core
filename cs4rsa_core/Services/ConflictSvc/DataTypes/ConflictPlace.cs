using System;
using System.Collections.Generic;

namespace cs4rsa_core.Services.ConflictSvc.DataTypes
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
