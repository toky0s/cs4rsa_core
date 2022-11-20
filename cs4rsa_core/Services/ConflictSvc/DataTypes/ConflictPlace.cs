using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.DataTypes
{
    public readonly struct ConflictPlace
    {
        public static readonly ConflictPlace NullInstance = new();
        public readonly Dictionary<DayOfWeek, IEnumerable<PlaceAdjacent>> PlaceAdjacents;

        public ConflictPlace(Dictionary<DayOfWeek, IEnumerable<PlaceAdjacent>> placeAdjacents)
        {
            PlaceAdjacents = placeAdjacents;
        }
    }
}
