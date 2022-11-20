using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.Constants;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;

namespace Cs4rsa.ViewModelFunctions
{
    /// <summary>
    /// Bộ chọn class group. Khi bộ chọn này Start nó thực hiện import các class group Model
    /// được truyền vào method tới choiced sesion.
    /// </summary>
    public sealed class ClassGroupChoicer : ObservableRecipient
    {
        public void Start(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(classGroupModel));
            }
        }

        /// <summary>
        /// Phương thức Start này sẽ import các class group có trong subject info data,
        /// bằng cách kiểm tra và tìm kiếm class group trong các subject model được truyền vào.
        /// Nếu không tìm thấy class group trong subject model thì nó sẽ không import.
        /// </summary>
        /// <param name="subjectModels"></param>
        /// <param name="subjectInfoDatas"></param>
        public void Start(IEnumerable<SubjectModel> subjectModels, IEnumerable<UserSubject> userSubjects)
        {
            foreach (UserSubject userSubject in userSubjects)
            {
                SubjectModel subjectModel = GetSubjectModelWithSubjectCode(subjectModels, userSubject.SubjectCode);
                Choose(subjectModel, userSubject.ClassGroup, userSubject.SchoolClass);
            }
        }

        private void Choose(
            SubjectModel subjectModel,
            string classGroupName,
            string schoolClass)
        {
            ClassGroupModel classGroupModel = subjectModel.GetClassGroupModelWithName(classGroupName);
            if (classGroupModel == null)
            {
                throw new Exception(VMConstants.EX_CLASSGROUP_MODEL_WAS_NULL);
            }
            if (classGroupModel.IsBelongSpecialSubject)
            {
                classGroupModel.PickSchoolClass(schoolClass);
                Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(classGroupModel));
            }
            else
            {
                Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(classGroupModel));
            }
        }

        private static SubjectModel GetSubjectModelWithSubjectCode(IEnumerable<SubjectModel> subjectModels, string subjectCode)
        {
            foreach (SubjectModel subject in subjectModels)
            {
                if (subject.SubjectCode.Equals(subjectCode))
                {
                    return subject;
                }
            }
            return null;
        }
    }
}
