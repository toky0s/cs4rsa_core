using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.UI.ScheduleTable.Models;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Services
{
    public class ScheduleValidator : IScheduleValidator
    {
        private readonly ILogger<ScheduleValidator> _logger;
        public ScheduleValidator(ILogger<ScheduleValidator> logger)
        {
            _logger = logger;
        }

        public List<WarningModel> ValidateSchedule(List<SchoolClassModel> schoolClasses)
        {
            List<WarningModel> warningModels = new List<WarningModel>();
            for (var i = 0; i < schoolClasses.Count; ++i)
            {
                var schoolClassModel_i = schoolClasses[i];
                for (var k = i + 1; k < schoolClasses.Count; ++k)
                {
                    var schoolClassModel_k = schoolClasses[k];

                    if (schoolClasses[i].SchoolClass.ClassGroupName.Equals(schoolClassModel_k.SchoolClass.ClassGroupName))
                    {
                        // Không kiểm tra xung đột giữa các lớp học cùng nhóm lớp
                        _logger.LogWarning("Don't check conflict between school classes in same class group");
                        continue;
                    }

                    _logger.LogInformation($"Checking conflict between school class {schoolClassModel_i.SchoolClass.ClassGroupName} and {schoolClassModel_k.SchoolClass.ClassGroupName}");
                    var lessonA = schoolClassModel_i.ConvertToLesson();
                    var lessonB = schoolClassModel_k.ConvertToLesson();

                    var conflict = new Conflict(lessonA, lessonB);
                    var conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        var message = $"Phát hiện trùng lịch giữa hai nhóm lớp {schoolClassModel_i.SchoolClass.ClassGroupName} và {schoolClassModel_k.SchoolClass.ClassGroupName}";
                        var warningModel = new WarningModel(WarningType.TimeConflict, message);
                        warningModels.Add(warningModel);
                    }
                }
            }
            return warningModels;
        }
    }
}
