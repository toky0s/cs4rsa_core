using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Messages.Publishers;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Linq;
using System;

namespace cs4rsa_core.ViewModelFunctions
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
        public void Start(IEnumerable<SubjectModel> subjectModels, IEnumerable<SubjectInfoData> subjectInfoDatas)
        {
            foreach (SubjectInfoData subjectInfoData in subjectInfoDatas)
            {
                SubjectModel subjectModel = GetSubjectModelWithSubjectCode(subjectModels, subjectInfoData.SubjectCode);
                Choose(subjectModel, subjectInfoData.ClassGroup, subjectInfoData.RegisterCode, subjectInfoData.SchoolClassName);
            }
        }

        private void Choose(
            SubjectModel subjectModel, 
            string classGroupName,
            string registerCode,
            string schoolClassName
        )
        {
            ClassGroupModel classGroupModel = subjectModel.GetClassGroupModelWithName(classGroupName);
            bool isValidRegisterCode = classGroupModel.GetSchoolClassModels()
                .Any(scm => scm.RegisterCode == registerCode); // <== Chỗ này isValidRegisterCode sẽ bằng true dùng registerCode có rỗng đi chăng nữa.
            if (classGroupModel == null)
            {
                throw new Exception(VMConstants.EX_CLASSGROUP_MODEL_WAS_NULL);
            }
            if (!isValidRegisterCode && !string.IsNullOrEmpty(registerCode))
            {
                throw new Exception(VMConstants.EX_INVALID_REGISTER_CODE);
            }
            if (classGroupModel.IsBelongSpecialSubject)
            {
                classGroupModel.PickSchoolClass(registerCode, schoolClassName);
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
