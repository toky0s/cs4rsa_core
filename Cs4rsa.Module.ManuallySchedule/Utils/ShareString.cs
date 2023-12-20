using Cs4rsa.Common;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Models;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Module.ManuallySchedule.Utils
{
    /// <summary>
    /// ShareString là một tính năng quan trọng giúp các
    /// sinh viên chia sẻ lịch học mà họ đã sắp xếp thông qua
    /// các phương tiện truyền thông, giúp giảm thời gian lựa chọn
    /// môn học cũng như nhóm lớp.
    /// </summary>
    public class ShareString
    {
        #region DI
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        public ShareString(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public string GetShareString(IEnumerable<ClassGroupModel> classGroupModels)
        {
            if (!classGroupModels.Any())
            {
                return string.Empty;
            }

            var userSubjects = ConvertToUserSubjects(classGroupModels);
            var json = JsonConvert.SerializeObject(userSubjects);
            return StringHelper.EncodeTo64(json);
        }

        public static IEnumerable<UserSubject> GetSubjectFromShareString(string shareString)
        {
            try
            {
                var json = StringHelper.DecodeFrom64(shareString);
                return JsonConvert.DeserializeObject<IEnumerable<UserSubject>>(json);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<UserSubject> ConvertToUserSubjects(IEnumerable<ClassGroupModel> classGroupModels)
        {
            var userSubjects = new List<UserSubject>();
            foreach (var classGroupModel in classGroupModels)
            {
                string selectedSchoolClassName;
                string registerCode;
                if (classGroupModel.IsSpecialClassGroup)
                {
                    selectedSchoolClassName = classGroupModel.UserSelectedSchoolClass.SchoolClassName;
                    registerCode = string.IsNullOrEmpty(classGroupModel.UserSelectedSchoolClass.RegisterCode)
                                 ? string.Empty
                                 : classGroupModel.UserSelectedSchoolClass.RegisterCode;
                }
                else
                {
                    selectedSchoolClassName = classGroupModel.CodeSchoolClass.SchoolClassName;
                    registerCode = string.IsNullOrEmpty(classGroupModel.CompulsoryClass.RegisterCode)
                       ? string.IsNullOrEmpty(classGroupModel.CodeSchoolClass.RegisterCode)
                           ? string.Empty
                           : classGroupModel.CodeSchoolClass.RegisterCode
                       : classGroupModel.CompulsoryClass.RegisterCode;
                }

                var userSubject = new UserSubject()
                {
                    SubjectCode = classGroupModel.SubjectCode,
                    SubjectName = _unitOfWork.Keywords.GetKeywordBySubjectCode(classGroupModel.SubjectCode).SubjectName,
                    ClassGroup = classGroupModel.ClassGroup.Name,
                    SchoolClass = selectedSchoolClassName,
                    RegisterCode = registerCode
                };
                userSubjects.Add(userSubject);
            }
            return userSubjects;
        }
    }
}
