using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.UI.ScheduleTable;

using System.Linq;

namespace Cs4rsa.Module.ManuallySchedule.Services
{
    public class TimeBlockGenerator : ITimeBlockGenerator
    {
        public TimeBlock[] Generate(Conflict conflict)
        {
            return conflict.ConflictTime.ConflictTimes
                .SelectMany(item => item.Value
                    .Select(sti => new TimeBlock(
                        id: TimeBlockGroupID.GenerateId(conflict.LessonA.SubjectCode, conflict.LessonB.SubjectCode, TimeBlockType.TimeConflict),
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
                    id: TimeBlockGroupID.GenerateId(schoolClassModel.SubjectCode),
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
                        id: TimeBlockGroupID.GenerateId(conflict.LessonA.SubjectCode, conflict.LessonB.SubjectCode, TimeBlockType.PlaceConflict),
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
