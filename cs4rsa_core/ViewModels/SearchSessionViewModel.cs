using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using cs4rsa_core.ViewModelFunctions;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using HelperService;
using LightMessageBus;
using LightMessageBus.Interfaces;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Toolkit.Mvvm.Input;

namespace cs4rsa_core.ViewModels
{
    /// <summary>
    /// ViewModel này đại diện cho phần Search Môn học.
    /// </summary>
    public class SearchSessionViewModel : ViewModelBase,
        IMessageHandler<UpdateSuccessMessage>,
        IMessageHandler<ShowOnSimuMessage>,
        IMessageHandler<ExitImportSubjectMessage>
    {
        #region Commands
        public IAsyncRelayCommand AddCommand { get; set; }
        private bool canRunAddCommand = true;
        public bool CanRunAddCommand
        {
            get => canRunAddCommand;
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
        private Discipline selectedDiscipline;
        public Discipline SelectedDiscipline
        {
            get => selectedDiscipline;

            set
            {
                selectedDiscipline = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Discipline> Disciplines { get; set; }

        //ComboxBox keyword
        private Keyword selectedKeyword;
        public Keyword SelectedKeyword
        {
            get => selectedKeyword;
            set
            {
                selectedKeyword = value;
                CanAddSubjectChange();
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Keyword> DisciplineKeywordModels { get; set; } = new();
        public ObservableCollection<SubjectModel> SubjectModels { get; set; } = new();

        private int _totalSubject;
        public int TotalSubject
        {
            get => _totalSubject;
            set
            {
                _totalSubject = value;
                OnPropertyChanged();
            }
        }

        private SubjectModel _selectedSubjectModel;
        public SubjectModel SelectedSubjectModel
        {
            get => _selectedSubjectModel;
            set
            {
                _selectedSubjectModel = value;
                DeleteCommand.NotifyCanExecuteChanged();
                MessageBus.Default.Publish(new SelectedSubjectChangeMessage(value));
            }
        }

        private int totalCredits;
        public int TotalCredits
        {
            get => totalCredits;
            set
            {
                totalCredits = value;
                OnPropertyChanged();
            }
        }

        private readonly ICourseCrawler _courseCrawler;
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly Cs4rsaDbContext _cs4rsaDbContext;
        private readonly ColorGenerator _colorGenerator;

        public SearchSessionViewModel(ICourseCrawler courseCrawler, Cs4rsaDbContext cs4rsaDbContext,
            ISubjectCrawler subjectCrawler, ColorGenerator colorGenerator)
        {
            _courseCrawler = courseCrawler;
            _subjectCrawler = subjectCrawler;
            _cs4rsaDbContext = cs4rsaDbContext;
            _colorGenerator = colorGenerator;
            MessageBus.Default.FromAny().Where<UpdateSuccessMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ShowOnSimuMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ExitImportSubjectMessage>().Notify(this);
            List<Discipline> disciplines = _cs4rsaDbContext.Disciplines.ToList();
            Disciplines = new ObservableCollection<Discipline>(disciplines);

            SelectedDiscipline = Disciplines[0];
            AddCommand = new AsyncRelayCommand(OnAddSubjectAsync);
            DeleteCommand = new RelayCommand(OnDeleteSubject, CanDeleteSubject);
            ImportDialogCommand = new RelayCommand(OnOpenImportDialog, () => true);
            GotoCourseCommand = new RelayCommand(OnGotoCourse, () => true);
            DeleteAllCommand = new RelayCommand(OnDeleteAll);
        }

        private void OnDeleteAll()
        {
            for (int i = SubjectModels.Count - 1; i >= 0; --i)
            {
                MessageBus.Default.Publish(new DeleteSubjectMessage(SubjectModels[i]));
                SubjectModels.RemoveAt(i);
            }
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
        }

        private void OnGotoCourse()
        {
            int courseId = _selectedSubjectModel.CourseId;
            string semesterValue = _courseCrawler.GetCurrentSemesterValue();
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }


        /// <summary>
        /// Load lại data môn học từ cơ sở dữ liệu lên
        /// </summary>
        private void ReloadDisciplineAndKeyWord()
        {
            List<Discipline> disciplines = _cs4rsaDbContext.Disciplines.ToList();
            disciplines.ForEach(discipline => Disciplines.Add(discipline));

            SelectedDiscipline = Disciplines[0];
            LoadDisciplineKeyword(SelectedDiscipline);
        }

        private void OnOpenImportDialog()
        {
            ImportSessionUC importSessionUC = new ImportSessionUC();
            ImportSessionViewModel vm = importSessionUC.DataContext as ImportSessionViewModel;
            vm.MessageBox = new Cs4rsaMessageBox();
            vm.LoadScheduleSession();
            //vm.CloseDialogCallback = CloseDialogAndHandleSessionManagerResult;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(importSessionUC);
        }

        private async Task CloseDialogAndHandleSessionManagerResult(SessionManagerResult result)
        {
            Cs4rsaMessageBox messageBoxService = new Cs4rsaMessageBox();
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            if (result != null)
            {
                SubjectImporterUC subjectImporterUC = new SubjectImporterUC();
                SubjectImporterViewModel vm = subjectImporterUC.DataContext as SubjectImporterViewModel;
                vm.SessionManagerResult = result;
                vm.CloseDialogCallback = CloseDialogAndHandleImportResult;
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(subjectImporterUC);
                await vm.Run();
            }
        }

        private void CloseDialogAndHandleImportResult(ImportResult importResult, SessionManagerResult sessionManagerResult)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            ImportSubjects(importResult.SubjectModels);
            ClassGroupChoicer.Start(importResult.SubjectModels, sessionManagerResult.SubjectInfoDatas);
            SelectedSubjectModel = SubjectModels[0];
        }

        private bool CanDeleteSubject()
        {
            return _selectedSubjectModel != null;
        }

        private void OnDeleteSubject()
        {
            MessageBus.Default.Publish(new DeleteSubjectMessage(_selectedSubjectModel));
            string message = $"Vừa xoá môn {_selectedSubjectModel.SubjectName}";
            SubjectModels.Remove(_selectedSubjectModel);
            MessageBus.Default.Publish(new Cs4rsaSnackbarMessage(message));
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        public void LoadDisciplineKeyword(Discipline discipline)
        {
            DisciplineKeywordModels.Clear();
            List<Keyword> keywords = _cs4rsaDbContext.Keywords.Where(keyword => keyword.Discipline == discipline).ToList();
            keywords.ForEach(keyword => DisciplineKeywordModels.Add(keyword));
            SelectedKeyword = DisciplineKeywordModels[0];
        }

        private async Task OnAddSubjectAsync()
        {
            CanAddSubjectChange(false);
            // Dialog here
            SubjectDownloadingUC subjectDownloadingUC = new();
            SubjectDownloadingViewModel vm = subjectDownloadingUC.DataContext as SubjectDownloadingViewModel;
            string subjectName = selectedKeyword.SubjectName;
            string subjectCode = selectedDiscipline.Name + " " + selectedKeyword.Keyword1;
            vm.SubjectName = subjectName;
            vm.SubjectCode = subjectCode;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(subjectDownloadingUC);
            // Dialog here
            Subject subject = await _subjectCrawler.Crawl(selectedDiscipline.Name, selectedKeyword.Keyword1);
            await subject.GetClassGroups();
            if (subject != null)
            {
                SubjectModel subjectModel = new(subject, _colorGenerator);
                SubjectModels.Add(subjectModel);
                TotalSubject = SubjectModels.Count;
                CanAddSubjectChange();
                UpdateCreditTotal();
                UpdateSubjectAmount();
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
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
            TotalSubject = SubjectModels.Count;
            MessageBus.Default.Publish(new SubjectItemChangeMessage(this));
        }

        /// <summary>
        /// Cập nhật tổng số tín chỉ.
        /// </summary>
        private void UpdateCreditTotal()
        {
            TotalCredits = 0;
            foreach (SubjectModel subject in SubjectModels)
            {
                TotalCredits += subject.StudyUnit;
            }
            MessageBus.Default.Publish(new SubjectItemChangeMessage(this));
        }

        private void CanAddSubjectChange(bool? value = null)
        {
            if (value == null)
            {
                List<int> courseIds = SubjectModels.Select(item => item.CourseId).ToList();
                if (selectedKeyword == null)
                {
                    CanRunAddCommand = true;
                    return;
                }
                CanRunAddCommand = !courseIds.Contains(SelectedKeyword.CourseId);
            }
            else
            {
                CanRunAddCommand = value.Value;
            }
        }

        private void ImportSubjects(List<SubjectModel> importSubjects)
        {
            foreach (SubjectModel subject in SubjectModels)
            {
                MessageBus.Default.Publish(new DeleteSubjectMessage(subject));
            }

            SubjectModels.Clear();
            foreach (SubjectModel subject in importSubjects)
            {
                SubjectModels.Add(subject);
            }
            TotalSubject = SubjectModels.Count;
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

        public async void Handle(ExitImportSubjectMessage message)
        {
            await CloseDialogAndHandleSessionManagerResult(message.Source);
        }
    }
}
