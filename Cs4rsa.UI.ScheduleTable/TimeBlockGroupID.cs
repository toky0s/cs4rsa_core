using System;
using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.UI.ScheduleTable
{
    public class TimeBlockGroupID : IEquatable<TimeBlockGroupID>
    {
        public static readonly HashSet<TimeBlockGroupID> UsedRealIDs = new HashSet<TimeBlockGroupID>();
        public string RealID { get; }

        private TimeBlockGroupID(string realID)
        {
            RealID = realID.Replace(" ", "");
        }

        private static TimeBlockGroupID Build(string realID)
        {
            var id = UsedRealIDs.Where(TimeBlockGroupID => TimeBlockGroupID.RealID.Equals(realID)).FirstOrDefault();
            if (id != null)
            {
                return id;
            }
            else
            {
                id = new TimeBlockGroupID(realID);
                UsedRealIDs.Add(id);
                return id;
            }
        }

        public bool Equals(TimeBlockGroupID other)
        {
            return other != null && RealID == other.RealID;
        }

        public static TimeBlockGroupID GenerateId(string lessonA_SubjectCode, string lessonB_SubjectCode, TimeBlockType type)
        {
            var prefix = type == TimeBlockType.PlaceConflict ? "PlaceConflict" : "TimeConflict";
            return Build($"{prefix}{lessonA_SubjectCode}{lessonB_SubjectCode}");
        }

        public static TimeBlockGroupID GenerateId(string schoolClassModel_SubjectCode)
        {
            return Build(schoolClassModel_SubjectCode);
        }
    }
}