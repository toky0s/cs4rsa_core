using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.UI.ScheduleTable.Interfaces;

using System;

namespace Cs4rsa.UI.ScheduleTable.Models
{
    public class SchoolClassBlock : TimeBlock, IEquatable<SchoolClassBlock>
    {
        public SchoolClassBlock(SchoolClassUnit schoolClassUnit,
            string id,
            string background,
            string content,
            DayOfWeek dayOfWeek,
            ScheduleTableItemType scheduleTableItemType,
            Phase phase)
            : base(id,
                  background,
                  content,
                  dayOfWeek,
                  schoolClassUnit.Start,
                  schoolClassUnit.End,
                  scheduleTableItemType)
        {
            SchoolClassUnit = schoolClassUnit;
            Phase = phase;
            Name = "SchoolClassBlock";
        }

        public SchoolClassUnit SchoolClassUnit { get; }
        public Phase Phase { get; }

        public bool Equals(SchoolClassBlock other)
        {
            return other != null && other.Id == Id;
        }
    }
}
