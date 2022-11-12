using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace cs4rsa_core.Utils
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
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public ShareString(
            IUnitOfWork unitOfWork, 
            ICourseCrawler courseCrawler,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
        }
        public async Task<string> GetShareString(IEnumerable<ClassGroupModel> classGroupModels)
        {
            if (!classGroupModels.Any())
            {
                return string.Empty;
            }

            IEnumerable<UserSubject> userSubjects = await ConvertToUserSubjects(classGroupModels);
            string json = JsonConvert.SerializeObject(userSubjects);
            return StringHelper.EncodeTo64(json);
        }

        public IEnumerable<UserSubject> GetSubjectFromShareString(string shareString)
        {
            try
            {
                string json = StringHelper.DecodeFrom64(shareString);
                return JsonConvert.DeserializeObject<IEnumerable<UserSubject>>(json);
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserSubject>> ConvertToUserSubjects(IEnumerable<ClassGroupModel> classGroupModels)
        {
            List<UserSubject> userSubjects = new();
            foreach (ClassGroupModel classGroupModel in classGroupModels)
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

                UserSubject userSubject = new()
                {
                    SubjectCode = classGroupModel.SubjectCode,
                    SubjectName = (await _unitOfWork.Keywords.GetKeyword(classGroupModel.SubjectCode)).SubjectName,
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
