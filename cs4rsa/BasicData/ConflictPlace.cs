using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public class ConflictPlace
    {
        private Dictionary<DayOfWeek, List<PlaceAdjacent>> _placeAdjacents = new Dictionary<DayOfWeek, List<PlaceAdjacent>>();
        public Dictionary<DayOfWeek, List<PlaceAdjacent>> PlaceAdjacents
        {
            get => _placeAdjacents;
            private set => _placeAdjacents = value;
        }

        public ConflictPlace(Dictionary<DayOfWeek, List<PlaceAdjacent>> placeAdjacents)
        {
            _placeAdjacents = placeAdjacents;
        }
    }
}
