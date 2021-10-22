using ConflictService.DataTypes;
using ConflictService.DataTypes.Enums;
using cs4rsa_core.Models.Interfaces;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Utils;
using System;
using System.Collections.Generic;

namespace cs4rsa_core.Models
{
    /// <summary>
    /// Model này đại diện cho một xung đột vị trí học.
    /// </summary>
    public class PlaceConflictFinderModel : IConflictModel
    {
        private SchoolClass _schoolClass1;
        private SchoolClass _schoolClass2;
        private ConflictPlace _conflictPlace;

        public SchoolClass FirstSchoolClass { get => _schoolClass1; set => _schoolClass1 = value; }
        public SchoolClass SecondSchoolClass { get => _schoolClass2; set => _schoolClass2 = value; }

        public PlaceConflictFinderModel(PlaceConflictFinder placeConflictFinder)
        {
            _conflictPlace = placeConflictFinder.GetPlaceConflict();
            _schoolClass1 = placeConflictFinder.FirstSchoolClass;
            _schoolClass2 = placeConflictFinder.SecondSchoolClass;
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
                    string to = $"đến {placeAdjacent.End} ở {BasicDataConverter.ToStringFromPlace(placeAdjacent.PlaceEnd)}";
                    info.Add(from+" "+to);
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
            Phase classGroupPhase1 = _schoolClass1.GetPhase();
            Phase classGroupPhase2 = _schoolClass2.GetPhase();
            if ((classGroupPhase1 == Phase.First && classGroupPhase2 == Phase.First) ||
                    (classGroupPhase1 == Phase.First && classGroupPhase2 == Phase.All))
                return Phase.First;
            if ((classGroupPhase1 == Phase.Second && classGroupPhase2 == Phase.Second) ||
                    (classGroupPhase1 == Phase.Second && classGroupPhase2 == Phase.All))
                return Phase.Second;
            return Phase.All;
        }
    }
}
