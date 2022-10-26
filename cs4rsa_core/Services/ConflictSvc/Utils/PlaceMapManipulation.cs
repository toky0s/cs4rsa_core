using cs4rsa_core.Services.ConflictSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Collections.Generic;

namespace cs4rsa_core.Services.ConflictSvc.Utils
{
    public class PlaceMapManipulation
    {
        /// <summary>
        /// Bắt cặp các PlaceMap trong cùng một List. 
        /// Giống với phương thức StudyTimeManipulation.PairStudyTimes()
        /// </summary>
        /// <param name="studyTimes">List các PlaceMap.</param>
        /// <returns>List các Tuple là cặp các PlaceMap.</returns>
        public static IEnumerable<Tuple<PlaceMap, PlaceMap>> PairPlaceMaps(List<PlaceMap> placeMaps)
        {
            int index = 0;
            int placeMapsCount = placeMaps.Count;
            while (index < placeMapsCount - 1)
            {
                PlaceMap firstItem = placeMaps[index];
                for (int j = index + 1; j <= placeMaps.Count - 1; ++j)
                {
                    Tuple<PlaceMap, PlaceMap> tuplePlaceMap = new(firstItem, placeMaps[j]);
                    yield return tuplePlaceMap;
                }
                index++;
            }
        }


        /// <summary>
        /// Phát hiện xung đột Vị trí học.
        /// </summary>
        /// <param name="placeMapPairs"></param>
        /// <param name="timeDelta">TimeDelta đại diện cho một khoảng thời gian tối thiểu để
        /// chấp nhận một xung đột vị trí giữa hai nơi học.</param>
        /// <returns></returns>
        public static IEnumerable<PlaceAdjacent> GetPlaceAdjacents(IEnumerable<Tuple<PlaceMap, PlaceMap>> placeMapPairs, TimeSpan timeDelta)
        {
            foreach (Tuple<PlaceMap, PlaceMap> placeMapTuple in placeMapPairs)
            {
                yield return GetPlaceAdjacent(placeMapTuple.Item1, placeMapTuple.Item2, timeDelta);
            }
        }

        private static PlaceAdjacent GetPlaceAdjacent(PlaceMap placeMap1, PlaceMap placeMap2, TimeSpan timeDelta)
        {
            List<PlaceMap> placeMaps = new() { placeMap1, placeMap2 };
            placeMaps.Sort();
            // Loại trừ xung đột thời gian
            if (placeMaps[0].StudyTime.End >= placeMaps[1].StudyTime.Start)
            {
                return PlaceAdjacent.Instance;
            }
            if (placeMaps[1].StudyTime.Start - placeMaps[0].StudyTime.End <= timeDelta)
            {
                DateTime Start = placeMaps[0].StudyTime.End;
                DateTime End = placeMaps[1].StudyTime.Start;
                Place PlaceStart = placeMaps[0].Place;
                Place PlaceEnd = placeMaps[1].Place;
                return new PlaceAdjacent(Start, End, PlaceStart, PlaceEnd);
            }
            return PlaceAdjacent.Instance;
        }
    }
}
