using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Models.Enums;
using cs4rsa.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace cs4rsa.Models
{
    class PlaceConflictFinderModel : IConflictModel
    {
        private ClassGroup _classGroup1;
        private ClassGroup _classGroup2;
        private ConflictPlace _conflictPlace;

        public ClassGroup FirstClassGroup { get => _classGroup1; set => _classGroup1 = value; }
        public ClassGroup SecondClassGroup { get => _classGroup2; set => _classGroup2 = value; }

        public PlaceConflictFinderModel(PlaceConflictFinder placeConflictFinder)
        {
            _conflictPlace = placeConflictFinder.GetPlaceConflict();
            _classGroup1 = placeConflictFinder.FirstClassGroup;
            _classGroup2 = placeConflictFinder.SecondClassGroup;
        }

        public string GetConflictInfo()
        {
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, List<PlaceAdjacent>> item in _conflictPlace.PlaceAdjacents)
            {
                string day = BasicDataConverter.ToDayOfWeekText(item.Key);
                List<string> info = new List<string>();
                foreach (PlaceAdjacent placeAdjacent in item.Value)
                {
                    string from = $"Từ {placeAdjacent.Start} ở {BasicDataConverter.ToStringFromPlace(placeAdjacent.PlaceStart)}";
                    string place = $"đến {placeAdjacent.End} ở {BasicDataConverter.ToStringFromPlace(placeAdjacent.PlaceEnd)}";
                    info.Add(place);
                }
                string placeString = string.Join("\n", info);
                resultTimes.Add(day + "\n" + placeString);
            }
            return string.Join("\n", resultTimes);
        }

        public ConflictType GetConflictType()
        {
            return ConflictType.Place;
        }

        public Phase GetPhase()
        {
            Phase classGroupPhase1 = _classGroup1.GetPhase();
            Phase classGroupPhase2 = _classGroup2.GetPhase();
            if ((classGroupPhase1 == Phase.FIRST && classGroupPhase2 == Phase.FIRST) ||
                    (classGroupPhase1 == Phase.FIRST && classGroupPhase2 == Phase.ALL))
                return Phase.FIRST;
            if ((classGroupPhase1 == Phase.SECOND && classGroupPhase2 == Phase.SECOND) ||
                    (classGroupPhase1 == Phase.SECOND && classGroupPhase2 == Phase.ALL))
                return Phase.SECOND;
            return Phase.ALL;
        }
    }
}
