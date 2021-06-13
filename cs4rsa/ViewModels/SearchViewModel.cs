using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using cs4rsa.Dialogs.DialogService;
using System.Windows;

namespace cs4rsa.ViewModels
{
    /// <summary>
    /// ViewModel này đại diện cho phần Search Môn học.
    /// </summary>
    public class SearchViewModel : NotifyPropertyChangedBase
    {
        #region Commands
        public MyICommand AddCommand { get; set; }
        private bool canRunAddCommand = true;
        public bool CanRunAddCommand
        {
            get
            {
                return canRunAddCommand;
            }
            set
            {
                canRunAddCommand = value;
                RaisePropertyChanged();
            }
        }
        public MyICommand DeleteCommand { get; set; }
        public RelayCommand ImportDialogCommand { get; set; }
        #endregion

        #region DI Properties
        //ComboBox discipline
        private DisciplineInfomationModel selectedDiscipline;
        public DisciplineInfomationModel SelectedDiscipline
        {
            get
            {
                return selectedDiscipline;
            }

            set
            {
                selectedDiscipline = value;
                RaisePropertyChanged("SelectedDiscipline");
            }
        }

        private readonly ObservableCollection<DisciplineInfomationModel> disciplines;
        public ObservableCollection<DisciplineInfomationModel> Disciplines
        {
            get
            {
                return disciplines;
            }
        }

        //ComboxBox keyword
        private DisciplineKeywordModel selectedKeyword;
        public DisciplineKeywordModel SelectedKeyword
        {
            get
            {
                return selectedKeyword;
            }
            set
            {
                selectedKeyword = value;
                CanAddSubjectChange();
                RaisePropertyChanged("SelectedKeyword");
            }
        }

        private ObservableCollection<DisciplineKeywordModel> disciplineKeywordModels = new ObservableCollection<DisciplineKeywordModel>();
        public ObservableCollection<DisciplineKeywordModel> DisciplineKeywordModels
        {
            get
            {
                return disciplineKeywordModels;
            }
            set
            {
                disciplineKeywordModels = value;
            }
        }

        //ListBox downloaded subjects
        private ObservableCollection<SubjectModel> subjectModels = new ObservableCollection<SubjectModel>();
        public ObservableCollection<SubjectModel> SubjectModels
        {
            get
            {
                return subjectModels;
            }
            set
            {
                subjectModels = value;
            }
        }

        private int totalSubject = 0;
        public int TotalSubject
        {
            get
            {
                return totalSubject;
            }
            set
            {
                totalSubject = value;
                RaisePropertyChanged();
            }
        }

        private SubjectModel selectedSubjectModel;
        public SubjectModel SelectedSubjectModel
        {
            get
            {
                return selectedSubjectModel;
            }
            set
            {
                selectedSubjectModel = value;
                DeleteCommand.RaiseCanExecuteChanged();
                MessageBus.Default.Publish(new SelectedSubjectChangeMessage(value));
            }
        }

        //Total Credits
        private int totalCredits = 0;
        public int TotalCredits
        {
            get
            {
                return totalCredits;
            }
            set
            {
                totalCredits = value;
                RaisePropertyChanged();

            }
        }
        #endregion

        #region DI
        public IMessageBox MessageBox;
        #endregion

        public SearchViewModel()
        {
            List<string> disciplines = Cs4rsaDataView.GetDisciplines();
            List<DisciplineInfomationModel> disciplineInfomationModels = disciplines.Select(item => new DisciplineInfomationModel(item)).ToList();
            this.disciplines = new ObservableCollection<DisciplineInfomationModel>(disciplineInfomationModels);
            AddCommand = new MyICommand(OnAddSubject, ()=> true);
            DeleteCommand = new MyICommand(OnDeleteSubject, CanDeleteSubject);
            ImportDialogCommand = new RelayCommand(OnOpenImportDialog, () => true);
            SelectedDiscipline = this.disciplines[0];
        }

        private void OnOpenImportDialog(object obj)
        {
            Cs4rsaMessageBox messageBoxService = new Cs4rsaMessageBox();
            ImportDialogViewModel vm = new ImportDialogViewModel(messageBoxService);
            SessionManagerWindow dialogWindow = new SessionManagerWindow();
            SessionManagerResult result = DialogService<SessionManagerResult>.OpenDialog(vm, dialogWindow, obj as Window);
        }

        private bool CanDeleteSubject()
        {
            return selectedSubjectModel != null;
        }

        private void OnDeleteSubject()
        {
            MessageBus.Default.Publish(new DeleteSubjectMessage(selectedSubjectModel));
            subjectModels.Remove(selectedSubjectModel);
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        public void LoadDisciplineKeyword(string discipline)
        {
            disciplineKeywordModels.Clear();
            foreach (DisciplineKeywordModel item in Cs4rsaDataView.GetDisciplineKeywordModels(discipline))
            {
                disciplineKeywordModels.Add(item);
            }
            SelectedKeyword = disciplineKeywordModels[0];
        }

        private void OnAddSubject()
        {
            CanAddSubjectChange(false);
            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };
            backgroundWorker.DoWork += WorkerDownloadSubject;
            backgroundWorker.RunWorkerCompleted += WorkerComplete;
            SubjectCrawler subjectCrawler = new SubjectCrawler(selectedDiscipline.Discipline, selectedKeyword.Keyword1);
            backgroundWorker.RunWorkerAsync(subjectCrawler);
        }

        private void WorkerDownloadSubject(object sender, DoWorkEventArgs e)
        {
            SubjectCrawler subjectCrawler = (SubjectCrawler)e.Argument;
            Subject subject = subjectCrawler.ToSubject();
            if (subject != null)
            {
                SubjectModel subjectModel = new SubjectModel(subject);
                e.Result = subjectModel;
            }
        }

        private void WorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                SubjectModel subjectModel = (SubjectModel)e.Result;
                subjectModel.Color = ColorGenerator.GetColor(subjectModel.CourseId);
                subjectModels.Add(subjectModel);
                TotalSubject = subjectModels.Count;
                CanAddSubjectChange();
                UpdateCreditTotal();
                UpdateSubjectAmount();
            }
            else
            {
                MessageBox.ShowMessage("Môn học này không tồn tại trong học kỳ này",
                    "Thông báo",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
                CanAddSubjectChange();
            }
        }

        /// <summary>
        /// Cập nhật tổng số môn học.
        /// </summary>
        private void UpdateSubjectAmount()
        {
            TotalSubject = subjectModels.Count;
            MessageBus.Default.Publish(new SubjectItemChangeMessage(this));
        }

        /// <summary>
        /// Cập nhật tổng số tín chỉ.
        /// </summary>
        private void UpdateCreditTotal()
        {
            TotalCredits = 0;
            foreach (SubjectModel subject in subjectModels)
            {
                TotalCredits += subject.StudyUnit;
            }
            MessageBus.Default.Publish(new SubjectItemChangeMessage(this));
        }

        private void CanAddSubjectChange(bool? value=null)
        {
            if (value == null)
            {
                List<string> courseIds = subjectModels.Select(item => item.CourseId).ToList();
                if (selectedKeyword == null)
                {
                    CanRunAddCommand = true;
                    return;
                }
                if (courseIds.Contains(SelectedKeyword.CourseID))
                    CanRunAddCommand = false;
                else
                    CanRunAddCommand = true;
            }
            else
            {
                CanRunAddCommand = value.Value;
            }
        }
    }
}
