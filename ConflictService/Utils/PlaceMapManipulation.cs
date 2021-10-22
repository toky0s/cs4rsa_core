using ConflictService.DataTypes;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConflictService.Utils
{
    public class PlaceMapManipulation
    {
        /// <summary>
        /// Bắt cặp các PlaceMap trong cùng một List. 
        /// Giống với phương thức StudyTimeManipulation.PairStudyTimes()
        /// </summary>
        /// <param name="studyTimes">List các PlaceMap.</param>
        /// <returns>List các Tuple là cặp các PlaceMap.</returns>
        public static List<Tuple<PlaceMap, PlaceMap>> PairPlaceMaps(List<PlaceMap> placeMaps)
        {
            List<Tuple<PlaceMap, PlaceMap>> tuplePlaceMaps = new List<Tuple<PlaceMap, PlaceMap>>();
            int index = 0;
            while (index < placeMaps.Count() - 1)
            {
                PlaceMap firstItem = placeMaps[index];
                for (int j = index + 1; j <= placeMaps.Count() - 1; ++j)
                {
                    Tuple<PlaceMap, PlaceMap> tuplePlaceMap = new Tuple<PlaceMap, PlaceMap>(firstItem, placeMaps[j]);
                    tuplePlaceMaps.Add(tuplePlaceMap);
                }
                index++;
            }
            return tuplePlaceMaps;
        }


        /// <summary>
        /// Phát hiện xung đột Vị trí học.
        /// </summary>
        /// <param name="placeMapPairs"></param>
        /// <param name="timeDelta">TimeDelta đại diện cho một khoảng thời gian tối thiểu để
        /// chấp nhận một xung đột vị trí giữa hai nơi học.</param>
        /// <returns></returns>
        public static List<PlaceAdjacent> GetPlaceAdjacents(List<Tuple<PlaceMap, PlaceMap>> placeMapPairs, TimeSpan timeDelta)
        {
            List<PlaceAdjacent> placeAdjacents = new List<PlaceAdjacent>();
            foreach (Tuple<PlaceMap, PlaceMap> placeMapTuple in placeMapPairs)
            {
                PlaceAdjacent placeAdjacent = GetPlaceAdjacent(placeMapTuple.Item1, placeMapTuple.Item2, timeDelta);
                if (placeAdjacent != null)
                    placeAdjacents.Add(placeAdjacent);
            }
            return placeAdjacents;
        }

        private static PlaceAdjacent GetPlaceAdjacent(PlaceMap placeMap1, PlaceMap placeMap2, TimeSpan timeDelta)
        {
            List<PlaceMap> placeMaps = new List<PlaceMap>() { placeMap1, placeMap2 };
            placeMaps.Sort();
            // Loại trừ xung đột thời gian
            if (placeMaps[0].StudyTime.End >= placeMaps[1].StudyTime.Start)
            {
                return null;
            }
            if (placeMaps[1].StudyTime.Start - placeMaps[0].StudyTime.End <= timeDelta)
            {
                DateTime Start = placeMaps[0].StudyTime.End;
                DateTime End = placeMaps[1].StudyTime.Start;
                Place PlaceStart = placeMaps[0].Place;
                Place PlaceEnd = placeMaps[1].Place;
                return new PlaceAdjacent(Start, End, PlaceStart, PlaceEnd);
            }
            return null;
        }
    }
}
