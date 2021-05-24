using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.Helpers;
using cs4rsa.Dialogs.SaveDialog;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System;

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

        private ObservableCollection<ConflictModel> conflictModels = new ObservableCollection<ConflictModel>();
        public ObservableCollection<ConflictModel> ConflictModels
        {
            get
            {
                return conflictModels;
            }
            set
            {
                conflictModels = value;
            }
        }

        public MyICommand SaveCommand { get; set; }

        public ChoiceSessionViewModel()
        {
            MessageBus.Default.FromAny().Where<ClassGroupAddedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            SaveCommand = new MyICommand(OpenSaveDialog, ()=>true);
        }

        private void OpenSaveDialog()
        {
            SaveDialog saveDialog = new SaveDialog
            {
                DataContext = new SaveDialogViewModel(ClassGroupModels)
            };
            saveDialog.ShowDialog();
        }

        public void Handle(ClassGroupAddedMessage message)
        {
            ClassGroupModel classGroupModel = message.Source;
            //TODO: add color
            string color = ColorGenerator.GetColor();
            
            if (classGroupModel != null)
            {
                classGroupModel.AddColor(color);
                int ClassGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (ClassGroupModelIndex != -1)
                    classGroupModels[ClassGroupModelIndex] = classGroupModel;
                else
                    classGroupModels.Add(classGroupModel);
            }
            UpdateConflictModelCollection();
            MessageBus.Default.Publish(new ChoicesChangedMessage(classGroupModels.ToList()));
        }

        /// <summary>
        /// Kiểm tra xem một Class Group Model nào đó có tồn tại một
        /// phiên bản cùng Subject Code nhưng khác tên khác không.
        /// </summary>
        /// <param name="classGroupModel">Một Class Group Model.</param>
        /// <returns>Trả về index của ClassGroupModel nếu nó có SubjectCode
        /// bằng với ClassGroupModel được truyền vào nếu không trả về -1.</returns>
        private int IsReallyHaveAnotherVersionInChoicedList(ClassGroupModel classGroupModel)
        {
            for (int i = 0; i < classGroupModels.Count; ++i)
                if (classGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
                    return i;
            return -1;
        }

        public void Handle(DeleteSubjectMessage message)
        {
            SubjectModel subjectModel = message.Source;
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    classGroupModels.Remove(classGroupModel);
                    break;
                }
            }
            UpdateConflictModelCollection();
            MessageBus.Default.Publish(new ChoicesChangedMessage(classGroupModels.ToList()));
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict.
        /// </summary>
        /// <returns>Danh sách các Conflict</returns>
        private void UpdateConflictModelCollection()
        {
            conflictModels.Clear();
            for (int i = 0; i < classGroupModels.Count; ++i)
            {
                for (int k = i+1; k < classGroupModels.Count; ++k)
                {
                    Conflict conflict = new Conflict(classGroupModels[i], classGroupModels[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new ConflictModel(conflict, conflictTime);
                        conflictModels.Add(conflictModel);
                    }
                }
            }
        }
    }
}
