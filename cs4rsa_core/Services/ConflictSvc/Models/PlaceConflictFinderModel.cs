using cs4rsa_core.Commons.Enums;
using cs4rsa_core.Commons.Interfaces;
using cs4rsa_core.Commons.Models;
using cs4rsa_core.Services.ConflictSvc.DataTypes;
using cs4rsa_core.Services.ConflictSvc.DataTypes.Enums;
using cs4rsa_core.Services.ConflictSvc.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;
using cs4rsa_core.Services.SubjectCrawlerSvc.Utils;

using System;
using System.Collections.Generic;

namespace cs4rsa_core.Services.ConflictSvc.Models
{
    /// <summary>
    /// Model này đại diện cho một xung đột vị trí học.
    /// </summary>
    public class PlaceConflictFinderModel : IConflictModel, IScheduleTableItem
    {
        private SchoolClass _schoolClass1;
        private SchoolClass _schoolClass2;
        private ConflictPlace _conflictPlace;

        public ConflictPlace ConflictPlace
        {
            get
            {
                return _conflictPlace;
            }
            set
            {
                _conflictPlace = value;
            }
        }

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
            List<string> resultTimes = new();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<PlaceAdjacent>> item in _conflictPlace.PlaceAdjacents)
            {
                string day = item.Key.ToDayOfWeekText();
                List<string> info = new();
                foreach (PlaceAdjacent placeAdjacent in item.Value)
                {
                    string from = $"Từ {placeAdjacent.StartAsString} ở {placeAdjacent.PlaceStart.ToActualPlace()}";
                    string to = $"đến {placeAdjacent.EndAsString} ở {placeAdjacent.PlaceEnd.ToActualPlace()}";
                    info.Add(from + " " + to);
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
            if (classGroupPhase1 == Phase.First && classGroupPhase2 == Phase.First ||
                    classGroupPhase1 == Phase.First && classGroupPhase2 == Phase.All)
                return Phase.First;
            if (classGroupPhase1 == Phase.Second && classGroupPhase2 == Phase.Second ||
                    classGroupPhase1 == Phase.Second && classGroupPhase2 == Phase.All)
                return Phase.Second;
            return Phase.All;
        }

        public IEnumerable<TimeBlock> GetBlocks()
        {
            const string BACKGROUND = "#f1f2f6";
            const string NAME = "Xung đột vị trí";
            foreach (var item in _conflictPlace.PlaceAdjacents)
            {
                foreach (PlaceAdjacent placeAdjacent in item.Value)
                {
                    TimeBlock timeBlock = new(
                        BACKGROUND,
                        GetTimeBlockDescription(placeAdjacent),
                        item.Key,
                        placeAdjacent.Start,
                        placeAdjacent.End,
                        BlockType.PlaceConflict,
                        NAME,
                        class1: _schoolClass1.ClassGroupName,
                        class2: _schoolClass2.ClassGroupName
                    );
                    yield return timeBlock;
                }
            }
        }

        private static string GetTimeBlockDescription(PlaceAdjacent placeAdjacent)
        {
            return $"{placeAdjacent.SchoolClass1.SchoolClassName} kết thúc lúc {placeAdjacent.StartAsString} - {placeAdjacent.PlaceStart.ToActualPlace()}\n" +
                   $"{placeAdjacent.SchoolClass2.SchoolClassName} bắt đầu lúc {placeAdjacent.EndAsString} - {placeAdjacent.PlaceEnd.ToActualPlace()}";
        }

        public object GetValue()
        {
            return this;
        }

        public ContextType GetContextType()
        {
            return ContextType.PConflict;
        }

        public string GetId()
        {
            return _schoolClass1.SchoolClassName + _schoolClass2.SchoolClassName;
        }
    }
}
