using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.Dialogs.DialogResults;
using LightMessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.ViewModelFunctions
{
    /// <summary>
    /// Bộ chọn class group. Khi bộ chọn này Start nó thực hiện import các class group Model
    /// được truyền vào method tới choiced sesion.
    /// </summary>
    public static class ClassGroupChoicer
    {
        public static void Start(List<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                MessageBus.Default.Publish(new ClassGroupAddedMessage(classGroupModel));
            }
        }


        /// <summary>
        /// Phương thức Start này sẽ import các class group có trong subject info data,
        /// bằng cách kiểm tra và tìm kiếm class group trong các subject model được truyền vào.
        /// Nếu không tìm thấy class group trong subject model thì nó sẽ không import.
        /// </summary>
        /// <param name="subjectModels"></param>
        /// <param name="subjectInfoDatas"></param>
        public static void Start(List<SubjectModel> subjectModels, List<SubjectInfoData> subjectInfoDatas)
        {
            foreach (SubjectInfoData subjectInfoData in subjectInfoDatas)
            {
                SubjectModel subjectModel = GetSubjectModelWithSubjectCode(subjectModels, subjectInfoData.SubjectCode);
                Choice(subjectModel, subjectInfoData.ClassGroup);
            }
        }

        private static void Choice(SubjectModel subjectModel, string classGroupName)
        {
            ClassGroupModel classGroupModel = subjectModel.GetClassGroupModelWithName(classGroupName);
            if (classGroupModel != null)
            {
                MessageBus.Default.Publish(new ClassGroupAddedMessage(classGroupModel));
            }
        }

        private static SubjectModel GetSubjectModelWithSubjectCode(List<SubjectModel> subjectModels, string subjectCode)
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
