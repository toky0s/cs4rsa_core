using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.UI.ScheduleTable.Interfaces;

using System;

namespace Cs4rsa.UI.ScheduleTable.Models
{
    internal class CfBlock : TimeBlock
    {
        public Lesson FirstCfClass { get; }
        public Lesson SecondCfClass { get; }
        public StudyTimeIntersect StudyTimeIntersect { get; }

        public CfBlock(
            StudyTimeIntersect studyTimeIntersect,
            string id,
            string background,
            string content,
            DayOfWeek dayOfWeek,
            ScheduleTableItemType scheduleTableItemType,
            Lesson firstCfClass,
            Lesson secondCfClass)
        : base(
            id,
            background,
            content,
            dayOfWeek,
            studyTimeIntersect.Start,
            studyTimeIntersect.End,
            scheduleTableItemType
        )
        {
            FirstCfClass = firstCfClass;
            SecondCfClass = secondCfClass;
            StudyTimeIntersect = studyTimeIntersect;
            Name = "CfBlock";
        }
    }
}
