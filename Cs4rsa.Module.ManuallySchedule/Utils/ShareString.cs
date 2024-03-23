using Cs4rsa.Database;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Infrastructure.Common;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Models;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (classGroupModels == null || !classGroupModels.Any())
            {
                return string.Empty;
            }

            //var userSubjects = ConvertToUserSubjects(classGroupModels);
            //return GetShareString(userSubjects);

            var scheduleBagModel = ToScheduleBagModel(classGroupModels);
            return GetShareString(scheduleBagModel);
        }

        public string GetShareString(IEnumerable<UserSubject> userSubjects)
        {
            var json = JsonConvert.SerializeObject(userSubjects);
            return StringHelper.EncodeTo64(json);
        }

        public string GetShareString(ScheduleBagModel scheduleBagModel)
        {
            var json = JsonConvert.SerializeObject(scheduleBagModel);
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

        public ScheduleBagModel ToScheduleBagModel(IEnumerable<ClassGroupModel> classGroupModels)
        {
            var settings = _unitOfWork.Settings.GetSettings();
            ScheduleBagModel scheduleBagModel = new ScheduleBagModel()
            {
                Year = settings[DbConsts.StCurrentYearInfo],
                Semester = settings[DbConsts.StCurrentSemesterInfo],
                YearValue = settings[DbConsts.StCurrentYearValue],
                SemesterValue = settings[DbConsts.StCurrentSemesterValue],
                SaveDate = DateTime.Now,
                UserScheduleId = 0,
                // Name from user input
                ScheduleBagItemModels = new ObservableCollection<ScheduleBagItemModel>()
            };

            var scheduleBagItems = classGroupModels.Select(cgm =>
            {
                ScheduleBagItemModel scheduleBagItemModel = new ScheduleBagItemModel()
                {
                    ClassGroup = cgm.Name,
                    SubjectCode = cgm.SubjectCode,
                    ScheduleDetailId = 0, // Create New
                    SubjectName = cgm.ClassGroup.SubjectName
                };

                if (cgm.IsSpecialClassGroup)
                {
                    scheduleBagItemModel.SelectedSchoolClass = cgm.UserSelectedSchoolClass.SchoolClassName;
                    scheduleBagItemModel.RegisterCode = string.IsNullOrEmpty(cgm.UserSelectedSchoolClass.RegisterCode)
                                 ? string.Empty
                                 : cgm.UserSelectedSchoolClass.RegisterCode;
                }
                else
                {
                    scheduleBagItemModel.SelectedSchoolClass = cgm.CodeSchoolClass.SchoolClassName;
                    scheduleBagItemModel.RegisterCode = string.IsNullOrEmpty(cgm.CompulsoryClass.RegisterCode)
                                 ? string.IsNullOrEmpty(cgm.CodeSchoolClass.RegisterCode)
                                     ? string.Empty // (3) if not null
                                     : cgm.CodeSchoolClass.RegisterCode // (2) if not null
                                 : cgm.CompulsoryClass.RegisterCode; // (1) if not null
                }

                return scheduleBagItemModel;
            });

            scheduleBagModel.ScheduleBagItemModels.AddRange(scheduleBagItems);
            return scheduleBagModel;
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
                                     ? string.Empty // (3) if not null
                                     : classGroupModel.CodeSchoolClass.RegisterCode // (2) if not null
                                 : classGroupModel.CompulsoryClass.RegisterCode; // (1) if not null
                }

                var userSubject = new UserSubject()
                {
                    SubjectCode = classGroupModel.SubjectCode,
                    SubjectName = classGroupModel.ClassGroup.SubjectName,
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
