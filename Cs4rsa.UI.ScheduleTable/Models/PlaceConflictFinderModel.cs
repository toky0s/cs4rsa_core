using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.DataTypes.Enums;
using Cs4rsa.Service.Conflict.Interfaces;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.SubjectCrawler.Utils;
using Cs4rsa.UI.ScheduleTable.Interfaces;

using System;
using System.Collections.Generic;

namespace Cs4rsa.UI.ScheduleTable.Models
{
    /// <summary>
    /// Model này đại diện cho một xung đột vị trí học.
    /// </summary>
    public class PlaceConflictFinderModel : IConflictModel, IScheduleTableItem
    {
        public ConflictPlace ConflictPlace { get; set; }
        public Lesson LessonA { get; set; }
        public Lesson LessonB { get; set; }

        public PlaceConflictFinderModel(PlaceConflictFinder placeConflictFinder)
        {
            ConflictPlace = placeConflictFinder.GetPlaceConflict();
            LessonA = placeConflictFinder.LessonA;
            LessonB = placeConflictFinder.LessonB;
        }

        public string GetConflictInfo()
        {
            List<string> resultTimes = new List<string>();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<PlaceAdjacent>> item in ConflictPlace.PlaceAdjacents)
            {
                string day = item.Key.ToCs4rsaVietnamese();
                List<string> info = new List<string>();
                foreach (PlaceAdjacent placeAdjacent in item.Value)
                {
                    string from = $"Từ {placeAdjacent.StartAsString} ở {placeAdjacent.PlaceStart.ToActualPlace()}";
                    string to = $"đến {placeAdjacent.EndAsString} ở {placeAdjacent.PlaceEnd.ToActualPlace()}";
                    info.Add($"{from} {to}");
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
            Phase phaseA = LessonA.Phase;
            Phase phaseB = LessonB.Phase;
            if (phaseA == Phase.First && phaseB == Phase.First
                || phaseA == Phase.First && phaseB == Phase.All
                || phaseA == Phase.All && phaseB == Phase.First)
                return Phase.First;
            if (phaseA == Phase.Second && phaseB == Phase.Second
                || phaseA == Phase.Second && phaseB == Phase.All
                || phaseA == Phase.All && phaseB == Phase.Second)
                return Phase.Second;
            return Phase.All;
        }

        public IEnumerable<TimeBlock> GetBlocks()
        {
            const string BACKGROUND = "#f1f2f6";
            foreach (var item in ConflictPlace.PlaceAdjacents)
            {
                foreach (PlaceAdjacent placeAdjacent in item.Value)
                {
                    var timeBlock = new PlaceCfBlock
                    (
                        placeAdjacent
                        , GetId()
                        , BACKGROUND
                        , LessonA.SchoolClassName + " x " + LessonB.SchoolClassName
                        , item.Key
                        , placeAdjacent.Start
                        , placeAdjacent.End
                        , ScheduleTableItemType.PlaceConflict
                    );

                    yield return timeBlock;
                }
            }
        }

        private static string GetTimeBlockDescription(PlaceAdjacent placeAdjacent)
        {
            return "Nơi học quá xa\n"
                + $"{placeAdjacent.SchoolClass1.SchoolClassName} kết thúc lúc {placeAdjacent.StartAsString} - {placeAdjacent.PlaceStart.ToActualPlace()}\n"
                + $"{placeAdjacent.SchoolClass2.SchoolClassName} bắt đầu lúc {placeAdjacent.EndAsString} - {placeAdjacent.PlaceEnd.ToActualPlace()}";
        }

        public string GetId()
        {
            return $"pc {LessonA.SubjectCode} {LessonB.SubjectCode}";
        }
    }
}
