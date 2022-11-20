using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.Constants
{
    internal class Constants
    {
        public static readonly List<Tuple<Place, Place>> EXCLUDED_CONFLICT_PLACES = new()
        {
            new Tuple<Place, Place>(Place.NVL_254, Place.NVL_254),
            new Tuple<Place, Place>(Place.NVL_137, Place.NVL_137),
            new Tuple<Place, Place>(Place.PHANTHANH, Place.PHANTHANH),
            new Tuple<Place, Place>(Place.ONLINE, Place.ONLINE),
            new Tuple<Place, Place>(Place.VIETTIN, Place.VIETTIN),
            new Tuple<Place, Place>(Place.QUANGTRUNG, Place.QUANGTRUNG),
            new Tuple<Place, Place>(Place.HOAKHANH, Place.HOAKHANH),

            new Tuple<Place, Place>(Place.NVL_254, Place.PHANTHANH),
            new Tuple<Place, Place>(Place.NVL_254, Place.VIETTIN),
            new Tuple<Place, Place>(Place.NVL_254, Place.NVL_137),
            new Tuple<Place, Place>(Place.PHANTHANH, Place.VIETTIN),
            new Tuple<Place, Place>(Place.PHANTHANH, Place.NVL_137),
        };
    }
}
