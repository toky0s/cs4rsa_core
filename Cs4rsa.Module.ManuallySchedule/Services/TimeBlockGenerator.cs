using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.UI.ScheduleTable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Services
{
    public class TimeBlockGenerator : ITimeBlockGenerator
    {
        public static string GenerateId(PlaceConflict conflict)
        {
            return $"pc {conflict.LessonA.SubjectCode} {conflict.LessonB.SubjectCode}";
        }
        public static string GenerateId(Conflict conflict)
        {
            return $"pc {conflict.LessonA.SubjectCode} {conflict.LessonB.SubjectCode}";
        }
        public static string GenerateId(SchoolClassModel schoolClassModel)
        {
            return schoolClassModel.SchoolClass.Subject.SubjectCode;
        }

        public TimeBlock[] Generate(Conflict conflict)
        {
            return conflict.ConflictTime.ConflictTimes
                .SelectMany(item => item.Value
                    .Select(sti => new TimeBlock(
                        id: GenerateId(conflict),
                        background: "#e74c3c",
                        content: conflict.LessonA.SchoolClassName + " x " + conflict.LessonB.SchoolClassName,
                        dayOfWeek: item.Key,
                        scheduleTableItemType: TimeBlockType.TimeConflict,
                        phase: conflict.Phase,
                        studyTimeIntersect: sti,
                        firstCfClass: conflict.LessonA,
                        secondCfClass: conflict.LessonB,
                        schoolClassUnit: null,
                        placeAdjacent: null
                ))).ToArray();
        }

        public TimeBlock[] Generate(SchoolClassModel schoolClassModel)
        {
            return schoolClassModel.SchoolClass.SchoolClassUnits
                .Select(unit => new TimeBlock
                (
                    id: GenerateId(schoolClassModel),
                    background: schoolClassModel.ClassGroupModel.Color,
                    content: schoolClassModel.SchoolClassName,
                    unit.DayOfWeek,
                    scheduleTableItemType: TimeBlockType.SchoolClass,
                    phase: schoolClassModel.Phase,
                    studyTimeIntersect: null,
                    firstCfClass: null,
                    secondCfClass: null,
                    schoolClassUnit: unit,
                    placeAdjacent: null
                )).ToArray();
        }

        public TimeBlock[] Generate(PlaceConflict conflict)
        {
            return conflict.ConflictPlace.PlaceAdjacents
                .SelectMany(pa => pa.Value
                    .Select(item => new TimeBlock(
                        id: GenerateId(conflict),
                        background: "#f1f2f6",
                        content: conflict.LessonA.SchoolClassName + " x " + conflict.LessonB.SchoolClassName,
                        dayOfWeek: pa.Key,
                        scheduleTableItemType: TimeBlockType.TimeConflict,
                        phase: conflict.Phase,
                        studyTimeIntersect: null,
                        firstCfClass: null,
                        secondCfClass: null,
                        schoolClassUnit: null,
                        placeAdjacent: item
                    ))
                ).ToArray();
        }
    }
}
