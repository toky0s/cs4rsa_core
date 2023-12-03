using Cs4rsa.Interfaces;
using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.UI.ScheduleTable.Models;

using System;

namespace Cs4rsa.Models
{
    internal class CfBlock : TimeBlock
    {
        public SchoolClassModel FirstCfClass { get; }
        public SchoolClassModel SecondCfClass { get; }
        public StudyTimeIntersect StudyTimeIntersect { get; }

        public CfBlock(
            StudyTimeIntersect studyTimeIntersect
            , string id
            , string background
            , string content
            , DayOfWeek dayOfWeek
            , ScheduleTableItemType scheduleTableItemType
            , SchoolClassModel firstCfClass
            , SchoolClassModel secondCfClass) 
        : base(
            id
            , background
            , content
            , dayOfWeek
            , studyTimeIntersect.Start
            , studyTimeIntersect.End
            , scheduleTableItemType
        )
        {
            FirstCfClass = firstCfClass;
            SecondCfClass = secondCfClass;
            StudyTimeIntersect = studyTimeIntersect;
            Name = "CfBlock";
        }
    }
}
