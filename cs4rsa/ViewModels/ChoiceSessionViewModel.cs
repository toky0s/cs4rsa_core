using cs4rsa.BaseClasses;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System.Collections.ObjectModel;
using System.Collections;

namespace cs4rsa.ViewModels
{
    class ChoiceSessionViewModel : NotifyPropertyChangedBase, 
        IMessageHandler<ClassGroupAddedMessage>,
        IMessageHandler<DeleteSubjectMessage>
    {
        private ObservableCollection<ClassGroupModel> classGroupModels = new ObservableCollection<ClassGroupModel>();
        public ObservableCollection<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return classGroupModels;
            }
            set
            {
                classGroupModels = value;
            }
        }

        public ChoiceSessionViewModel()
        {
            MessageBus.Default.FromAny().Where<ClassGroupAddedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
        }

        public void Handle(ClassGroupAddedMessage message)
        {
            ClassGroupModel classGroupModel = (message.Source as ClassGroupViewModel).SelectedClassGroup;
            if (classGroupModel != null)
            {
                if (IsReallyHaveAnotherVersionInChoicedList(classGroupModel))
                    RemoveCurrentVersionAndReplaceNew(classGroupModel);
                else
                    classGroupModels.Add(classGroupModel);
            }
        }

        private bool IsReallyHaveAnotherVersionInChoicedList(ClassGroupModel classGroupModel)
        {
            foreach(ClassGroupModel _classGroupModel in classGroupModels)
            {
                if (_classGroupModel.SubjectCode.Equals(classGroupModel.SubjectCode))
                    return true;
            }
            return false;
        }

        private void RemoveCurrentVersionAndReplaceNew(ClassGroupModel classGroupModel)
        {
            for(int i=0; i<classGroupModels.Count;++i)
            {
                if (classGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
                    classGroupModels[i] = classGroupModel;
            }
        }

        public void Handle(DeleteSubjectMessage message)
        {
            SubjectModel subjectModel = message.Source;
            foreach(ClassGroupModel classGroupModel in classGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    classGroupModels.Remove(classGroupModel);
                    return;
                }
            }
        }
    }
}
