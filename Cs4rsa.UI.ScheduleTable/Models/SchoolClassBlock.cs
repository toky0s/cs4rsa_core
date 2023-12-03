using Cs4rsa.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.UI.ScheduleTable.Models;

using System;

namespace Cs4rsa.Models
{
    public class SchoolClassBlock : TimeBlock
    {
        public SchoolClassBlock(
              SchoolClassUnit schoolClassUnit
            , string id
            , string background
            , string content
            , DayOfWeek dayOfWeek
            , ScheduleTableItemType scheduleTableItemType
            , Phase phase) 
            : base(
                  id
                , background
                , content
                , dayOfWeek
                , schoolClassUnit.Start
                , schoolClassUnit.End
                , scheduleTableItemType
            )
        {
            SchoolClassUnit = schoolClassUnit;
            Phase = phase;
            Name = "SchoolClassBlock";
        }

        public SchoolClassUnit SchoolClassUnit { get;}
        public Phase Phase { get;}
    }
}
