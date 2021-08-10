using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.ViewModelFunctions;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace cs4rsa.ViewModels
{
    /// <summary>
    /// ViewModel này đại diện cho phần Search Môn học.
    /// </summary>
    public class SearchViewModel : ViewModelBase,
        IMessageHandler<UpdateSuccessMessage>,
        IMessageHandler<ShowOnSimuMessage>
    {
        #region Commands
        public RelayCommand AddCommand { get; set; }
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
                OnPropertyChanged();
            }
        }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand ImportDialogCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        #endregion

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
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DisciplineInfomationModel> _disciplines;
        public ObservableCollection<DisciplineInfomationModel> Disciplines
        {
            get
            {
                return _disciplines;
            }
            set
            {
                _disciplines = value;
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();

            }
        }

        public SearchViewModel()
        {
            MessageBus.Default.FromAny().Where<UpdateSuccessMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ShowOnSimuMessage>().Notify(this);
            List<string> disciplines = Cs4rsaDataView.GetDisciplines();
            List<DisciplineInfomationModel> disciplineInfomationModels = disciplines.Select(item => new DisciplineInfomationModel(item)).ToList();
            _disciplines = new ObservableCollection<DisciplineInfomationModel>(disciplineInfomationModels);
            SelectedDiscipline = _disciplines[0];
            AddCommand = new RelayCommand(OnAddSubject);
            DeleteCommand = new RelayCommand(OnDeleteSubject, CanDeleteSubject);
            ImportDialogCommand = new RelayCommand(OnOpenImportDialog, () => true);
            GotoCourseCommand = new RelayCommand(OnGotoCourse, () => true);
            DeleteAllCommand = new RelayCommand(OnDeleteAll);
        }

        private void OnDeleteAll(object obj)
        {
            for (int i = subjectModels.Count-1; i >= 0; --i)
            {
                MessageBus.Default.Publish(new DeleteSubjectMessage(subjectModels[i]));
                subjectModels.RemoveAt(i);
            }
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.RaiseCanExecuteChanged();
        }

        private void OnGotoCourse(object obj)
        {
            string courseId = selectedSubjectModel.CourseId;
            string semesterValue = HomeCourseSearch.GetInstance().CurrentSemesterValue;
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
            Process.Start(url);
        }


        /// <summary>
        /// Load lại data môn học từ cơ sở dữ liệu lên
        /// </summary>
        private void ReloadDisciplineAndKeyWord()
        {
            List<string> disciplines = Cs4rsaDataView.GetDisciplines();
            List<DisciplineInfomationModel> disciplineInfomationModels = disciplines.Select(item => new DisciplineInfomationModel(item)).ToList();
            foreach (DisciplineInfomationModel item in disciplineInfomationModels)
            {
                _disciplines.Add(item);
            }
            SelectedDiscipline = _disciplines[0];
            LoadDisciplineKeyword(selectedDiscipline.Discipline);
        }

        private void OnOpenImportDialog(object obj)
        {
            ImportSessionUC importSessionUC = new ImportSessionUC();
            ImportDialogViewModel vm = (importSessionUC.DataContext as ImportDialogViewModel);
            vm.MessageBox = new Cs4rsaMessageBox();
            vm.CloseDialogCallback = CloseDialogAndHandleSessionManagerResult;
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(importSessionUC);
        }

        private void CloseDialogAndHandleSessionManagerResult(SessionManagerResult result)
        {
            Cs4rsaMessageBox messageBoxService = new Cs4rsaMessageBox();
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            if (result != null)
            {
                SubjectImporterUC subjectImporterUC = new SubjectImporterUC();
                SubjectImporter vm = subjectImporterUC.DataContext as SubjectImporter;
                vm.SessionManagerResult = result;
                vm.CloseDialogCallback = CloseDialogAndHandleImportResult;
                (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(subjectImporterUC);
            }
        }

        private void CloseDialogAndHandleImportResult(ImportResult importResult, SessionManagerResult sessionManagerResult)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            ImportSubjects(importResult.SubjectModels);
            ClassGroupChoicer.Start(importResult.SubjectModels, sessionManagerResult.SubjectInfoDatas);
            SelectedSubjectModel = SubjectModels[0];
        }

        private bool CanDeleteSubject()
        {
            return selectedSubjectModel != null;
        }

        private void OnDeleteSubject(object obj)
        {
            MessageBus.Default.Publish(new DeleteSubjectMessage(selectedSubjectModel));
            string message = $"Vừa xoá môn {selectedSubjectModel.SubjectName}";
            subjectModels.Remove(selectedSubjectModel);
            MessageBus.Default.Publish(new Cs4rsaSnackbarMessage(message));
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

        private void OnAddSubject(object obj)
        {
            CanAddSubjectChange(false);

            SubjectDownloadingUC subjectDownloadingUC = new SubjectDownloadingUC();
            SubjectDownloadingViewModel vm = subjectDownloadingUC.DataContext as SubjectDownloadingViewModel;
            string subjectName = selectedKeyword.SubjectName;
            string subjectCode = selectedDiscipline.Discipline + " " + selectedKeyword.Keyword1;
            vm.SubjectName = subjectName;
            vm.SubjectCode = subjectCode;
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(subjectDownloadingUC);

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
                subjectModels.Add(subjectModel);
                TotalSubject = subjectModels.Count;
                CanAddSubjectChange();
                UpdateCreditTotal();
                UpdateSubjectAmount();
                (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            }
            else
            {
                MessageBus.Default.Publish(new Cs4rsaSnackbarMessage("Môn học không tồn tại trong học kỳ này"));
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

        private void CanAddSubjectChange(bool? value = null)
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

        private void ImportSubjects(List<SubjectModel> importSubjects)
        {
            foreach (SubjectModel subject in subjectModels)
                MessageBus.Default.Publish(new DeleteSubjectMessage(subject));
            subjectModels.Clear();
            foreach (SubjectModel subject in importSubjects)
            {
                subjectModels.Add(subject);
            }
            TotalSubject = subjectModels.Count;
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
        }

        public void Handle(UpdateSuccessMessage message)
        {
            DisciplineKeywordModels.Clear();
            Disciplines.Clear();
            ReloadDisciplineAndKeyWord();
        }

        public void Handle(ShowOnSimuMessage message)
        {
            List<SubjectModel> subjectModels = message.Source.SubjecModels;
            ImportSubjects(subjectModels);

            List<ClassGroupModel> classGroupModels = message.Source.ClassGroupModels;
            ClassGroupChoicer.Start(classGroupModels);
        }
    }
}
