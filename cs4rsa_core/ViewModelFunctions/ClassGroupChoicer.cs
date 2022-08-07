using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Messages.Publishers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SubjectCrawlService1.Models;
using System.Collections.Generic;
using System.Linq;

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
                Choose(subjectModel, subjectInfoData.ClassGroup, subjectInfoData.RegisterCode);
            }
        }

        private void Choose(SubjectModel subjectModel, string classGroupName, string registerCode)
        {
            ClassGroupModel classGroupModel = subjectModel.GetClassGroupModelWithName(classGroupName);
            bool isValidRegisterCode = classGroupModel.GetSchoolClassModels()
                .Any(scm => scm.RegisterCode == registerCode);
            if (classGroupModel == null)
            {
                throw new System.Exception("ClassGroupModel was null!");
            }
            if (!isValidRegisterCode)
            {
                throw new System.Exception("Register code for choose is invalid");
            }
            if (classGroupModel.IsBelongSpecialSubject)
            {
                classGroupModel.PickSchoolClass(registerCode);
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
