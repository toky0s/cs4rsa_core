using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

using System;

namespace Cs4rsa.UI.ScheduleTable
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleControl.
    /// </summary>
    public class TimeBlock: IEquatable<TimeBlock>
    {
        public TimeBlock(
            TimeBlockGroupID id,
            string background,
            string content,
            DayOfWeek dayOfWeek,
            TimeBlockType scheduleTableItemType,
            Phase phase,
            // Conflict
            StudyTimeIntersect studyTimeIntersect,
            Lesson firstCfClass,
            Lesson secondCfClass,
            // SchoolClass
            SchoolClassUnit schoolClassUnit,
            // PlaceConflict
            PlaceAdjacent placeAdjacent)
        {
            Id = id;
            Background = background;
            Content = content;
            DayOfWeek = dayOfWeek;

            switch(scheduleTableItemType)
            {
                case TimeBlockType.TimeConflict:
                    if (studyTimeIntersect == null || firstCfClass == null || secondCfClass == null)
                        throw new ArgumentException("Các tham số liên quan đến xung đột không được null khi scheduleTableItemType là TimeConflict.");
                    Start = studyTimeIntersect.Start;
                    End = studyTimeIntersect.End;
                    FirstCfClass = firstCfClass;
                    SecondCfClass = secondCfClass;
                    StudyTimeIntersect = studyTimeIntersect;
                    Name = TimeBlockName.TimeConflict;
                    break;
                case TimeBlockType.SchoolClass:
                    if (schoolClassUnit == null)
                        throw new ArgumentException("Tham số schoolClassUnit không được null khi scheduleTableItemType là SchoolClass.");
                    Start = schoolClassUnit.Start;
                    End = schoolClassUnit.End;
                    SchoolClassUnit = schoolClassUnit;
                    Phase = phase;
                    Name = TimeBlockName.SchoolClass;
                    break;
                case TimeBlockType.PlaceConflict:
                    PlaceAdjacent = placeAdjacent;
                    Start = placeAdjacent.Start;
                    End = placeAdjacent.End;
                    Name = TimeBlockName.PlaceConflict;
                    break;
                default:
                    throw new ArgumentException("scheduleTableItemType không hợp lệ.");
            }
            ScheduleTableItemType = scheduleTableItemType;
        }

        public SchoolClassUnit SchoolClassUnit { get; }
        public Phase Phase { get; }
        public Lesson FirstCfClass { get; }
        public Lesson SecondCfClass { get; }
        public StudyTimeIntersect StudyTimeIntersect { get; }
        public PlaceAdjacent PlaceAdjacent { get; }
        public string Name { get; set; }

        public TimeBlockGroupID Id { get; }
        /// <summary>
        /// Màu nền
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Thứ trong tuần
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime End { get; set; }
        public TimeBlockType ScheduleTableItemType { get; set; }

        public bool Equals(TimeBlock other)
        {
            return other != null && other.Id == Id;
        }
    }
}
