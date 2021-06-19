using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    public class PlaceConflict
    {
        private Dictionary<DayOfWeek, List<PlaceAdjacent>> _placeConflictItems = new Dictionary<DayOfWeek, List<PlaceAdjacent>>();

        public PlaceConflict(Dictionary<DayOfWeek, List<PlaceAdjacent>> placeConflictItems)
        {
            _placeConflictItems = placeConflictItems;
        }
    }
}
