using Cs4rsa.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

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
            , ScheduleTableItemType scheduleTableItemType) 
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
            Name = "SchoolClassBlock";
        }

        public SchoolClassUnit SchoolClassUnit { get;}
    }
}
