using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Utils
{
    public interface IShareStringService
    {
        string GetShareString(IEnumerable<ClassGroupModel> classGroupModels);
        string GetShareString(IEnumerable<UserSubject> userSubjects);
        string GetShareString(ScheduleBagModel scheduleBagModel);
        IEnumerable<UserSubject> GetSubjectFromShareString(string shareString);
        ScheduleBagModel ToScheduleBagModel(IEnumerable<ClassGroupModel> classGroupModels);
        IEnumerable<UserSubject> ConvertToUserSubjects(IEnumerable<ClassGroupModel> classGroupModels);
    }
}
