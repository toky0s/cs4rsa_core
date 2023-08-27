using Cs4rsa.Constants;
using Cs4rsa.Interfaces;
using Cs4rsa.Models;
using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.DataTypes.Enums;
using Cs4rsa.Services.ConflictSvc.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.Utils;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.Models
{
    /// <summary>
    /// Model này đại diện cho một xung đột vị trí học.
    /// </summary>
    public class PlaceConflictFinderModel : IConflictModel, IScheduleTableItem
    {
        public ConflictPlace ConflictPlace { get; set; }
        public SchoolClassModel FirstSchoolClass { get; set; }
        public SchoolClassModel SecondSchoolClass { get; set; }

        public PlaceConflictFinderModel(PlaceConflictFinder placeConflictFinder)
        {
            ConflictPlace = placeConflictFinder.GetPlaceConflict();
            FirstSchoolClass = placeConflictFinder.FirstSchoolClass;
            SecondSchoolClass = placeConflictFinder.SecondSchoolClass;
        }

        public string GetConflictInfo()
        {
            List<string> resultTimes = new();
            foreach (KeyValuePair<DayOfWeek, IEnumerable<PlaceAdjacent>> item in ConflictPlace.PlaceAdjacents)
            {
                string day = item.Key.ToDayOfWeekText();
                List<string> info = new();
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
            Phase classGroupPhase1 = FirstSchoolClass.GetPhase();
            Phase classGroupPhase2 = SecondSchoolClass.GetPhase();
            if (classGroupPhase1 == Phase.First && classGroupPhase2 == Phase.First
                || classGroupPhase1 == Phase.First && classGroupPhase2 == Phase.All
                || classGroupPhase1 == Phase.All && classGroupPhase2 == Phase.First)
                return Phase.First;
            if (classGroupPhase1 == Phase.Second && classGroupPhase2 == Phase.Second
                || classGroupPhase1 == Phase.Second && classGroupPhase2 == Phase.All
                || classGroupPhase1 == Phase.All && classGroupPhase2 == Phase.Second)
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
                    PlaceCfBlock timeBlock = new
                    (
                        placeAdjacent
                        , GetId()
                        , BACKGROUND
                        , FirstSchoolClass.SchoolClassName + " x " + SecondSchoolClass.SchoolClassName
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
            return "pc" 
                + VmConstants.CharSpace 
                + FirstSchoolClass.SubjectCode 
                + VmConstants.CharSpace 
                + SecondSchoolClass.SubjectCode;
        }
    }
}
