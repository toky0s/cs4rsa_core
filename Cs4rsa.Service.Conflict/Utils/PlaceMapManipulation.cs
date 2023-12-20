using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.Utils
{
    public class PlaceMapManipulation
    {
        public static readonly List<Tuple<Place, Place>> ExcludedConflictPlaces = new List<Tuple<Place, Place>>()
        {
            new Tuple<Place, Place>(Place.Nvl254, Place.Nvl254),
            new Tuple<Place, Place>(Place.Nvl137, Place.Nvl137),
            new Tuple<Place, Place>(Place.PhanThanh, Place.PhanThanh),
            new Tuple<Place, Place>(Place.Online, Place.Online),
            new Tuple<Place, Place>(Place.VietTin, Place.VietTin),
            new Tuple<Place, Place>(Place.QuangTrung, Place.QuangTrung),
            new Tuple<Place, Place>(Place.HoaKhanh, Place.HoaKhanh),

            new Tuple<Place, Place>(Place.Nvl254, Place.PhanThanh),
            new Tuple<Place, Place>(Place.Nvl254, Place.VietTin),
            new Tuple<Place, Place>(Place.Nvl254, Place.Nvl137),
            new Tuple<Place, Place>(Place.PhanThanh, Place.VietTin),
            new Tuple<Place, Place>(Place.PhanThanh, Place.Nvl137),
        };


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
                    var tuplePlaceMap = new Tuple<PlaceMap, PlaceMap>(firstItem, placeMaps[j]);
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
        public static IEnumerable<PlaceAdjacent> GetPlaceAdjacents(
            IEnumerable<Tuple<PlaceMap, PlaceMap>> placeMapPairs,
            TimeSpan timeDelta)
        {
            foreach (Tuple<PlaceMap, PlaceMap> placeMapTuple in placeMapPairs)
            {
                PlaceAdjacent placeAdjacent = GetPlaceAdjacent(
                    placeMapTuple.Item1,
                    placeMapTuple.Item2,
                    timeDelta
                );
                if (placeAdjacent != null)
                {
                    yield return placeAdjacent;
                }
            }
        }

        private static PlaceAdjacent GetPlaceAdjacent(
            PlaceMap placeMap1,
            PlaceMap placeMap2,
            TimeSpan timeDelta)
        {
            List<PlaceMap> placeMaps = new List<PlaceMap>() { placeMap1, placeMap2 };
            placeMaps.Sort();
            // Loại trừ xung đột thời gian
            if (placeMaps[0].StudyTime.End >= placeMaps[1].StudyTime.Start)
            {
                return null;
            }

            // Loại trừ các Place trong list exclude
            if (IsInExcludedList(placeMap1.Place, placeMap2.Place))
            {
                return null;
            }

            if (placeMaps[1].StudyTime.Start - placeMaps[0].StudyTime.End <= timeDelta)
            {
                DateTime Start = placeMaps[0].StudyTime.End;
                DateTime End = placeMaps[1].StudyTime.Start;
                Place PlaceStart = placeMaps[0].Place;
                Place PlaceEnd = placeMaps[1].Place;
                return new PlaceAdjacent(
                    Start,
                    End,
                    PlaceStart,
                    PlaceEnd,
                    placeMaps[0].SchoolClass,
                    placeMaps[1].SchoolClass
                );
            }
            return null;
        }

        /// <summary>
        /// Kiểm tra xem hai place bất kỳ có thuộc danh sách bị loại trừ hay không
        /// </summary>
        private static bool IsInExcludedList(Place place1, Place place2)
        {
            foreach (Tuple<Place, Place> exclude in ExcludedConflictPlaces)
            {
                if ((place1 == exclude.Item1 && place2 == exclude.Item2)
                || (place1 == exclude.Item2 && place2 == exclude.Item1))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
