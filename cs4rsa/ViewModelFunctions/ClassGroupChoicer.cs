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
    public static class ClassGroupChoicer
    {
        public static void Start(List<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                MessageBus.Default.Publish(new ClassGroupAddedMessage(classGroupModel));
            }
        }

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
