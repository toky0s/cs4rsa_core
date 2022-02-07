using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using cs4rsa_core.Services;
using cs4rsa_core.ViewModelFunctions;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using HelperService;
using LightMessageBus;
using LightMessageBus.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;



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
        public AsyncRelayCommand ImportDialogCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        #endregion

        #region Properties
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

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); }
        }

        private FullMatchSearchingKeyword _selectedFullMatchSearchingKeyword;
        public FullMatchSearchingKeyword SelectedFullMatchSearchingKeyword
        {
            get { return _selectedFullMatchSearchingKeyword; }
            set { _selectedFullMatchSearchingKeyword = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Keyword> DisciplineKeywordModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<FullMatchSearchingKeyword> FullMatchSearchingKeywords { get; set; }

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

        #endregion

        #region Services
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ColorGenerator _colorGenerator;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public SearchSessionViewModel(ICourseCrawler courseCrawler, IUnitOfWork unitOfWork,
            ISubjectCrawler subjectCrawler, ColorGenerator colorGenerator, IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _courseCrawler = courseCrawler;
            _subjectCrawler = subjectCrawler;
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
            _openInBrowser = openInBrowser;
            _snackbarMessageQueue = snackbarMessageQueue;

            MessageBus.Default.FromAny().Where<UpdateSuccessMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ShowOnSimuMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ExitImportSubjectMessage>().Notify(this);

            DisciplineKeywordModels = new();
            SubjectModels = new();
            Disciplines = new();
            FullMatchSearchingKeywords = new();
            SearchText = "";

            AddCommand = new AsyncRelayCommand(OnAddSubjectAsync);
            DeleteCommand = new RelayCommand(OnDeleteSubject, CanDeleteSubject);
            ImportDialogCommand = new(OnOpenImportDialog);
            GotoCourseCommand = new RelayCommand(OnGotoCourse, () => true);
            DeleteAllCommand = new RelayCommand(OnDeleteAll);
        }

        public async Task LoadDiscipline()
        {
            List<Discipline> disciplines = await _unitOfWork.Disciplines.GetAllIncludeKeywordAsync();
            foreach (Discipline discipline in disciplines)
            {
                Disciplines.Add(discipline);
            }
            SelectedDiscipline = Disciplines[0];
        }

        internal void LoadSelectedDisciplineAndKeyword(Discipline discipline, Keyword keyword)
        {
            Discipline actualDiscipline = _unitOfWork.Disciplines.GetById(discipline.DisciplineId);
            Keyword actualKeyword = _unitOfWork.Keywords.GetById(keyword.KeywordId);
            SelectedDiscipline = actualDiscipline;
            SelectedKeyword = actualKeyword;
        }

        public async Task LoadSearchItemSource(string text)
        {
            text = text.Trim();
            if (text.Equals("")) return;

            FullMatchSearchingKeywords.Clear();

            Task<List<Keyword>> result1 = _unitOfWork.Keywords.GetByDisciplineStartWith(text);
            Task<List<Keyword>> result2 = _unitOfWork.Keywords.GetBySubjectNameContains(text.FirstCharToUpper());
            List<Keyword>[] whenAllResult = await Task.WhenAll(result1, result2);
            foreach (List<Keyword> keywords in whenAllResult)
            {
                foreach (Keyword keyword in keywords)
                {
                    FullMatchSearchingKeyword fullMacth = new()
                    {
                        Keyword = keyword,
                        Discipline = keyword.Discipline
                    };
                    FullMatchSearchingKeywords.Add(fullMacth);
                }
            }

            if (text.Contains(' '))
            {
                string[] textSplit = text.Split(" ", System.StringSplitOptions.None);
                string discipline = textSplit[0];
                string keyword1 = textSplit[1];
                List<Keyword> keywordsBySubjectCode = await _unitOfWork.Keywords.GetByDisciplineAndKeyword1(discipline, keyword1);
                foreach (Keyword kw in keywordsBySubjectCode)
                {
                    FullMatchSearchingKeyword fullMatch = new()
                    {
                        Keyword = kw,
                        Discipline = kw.Discipline
                    };
                    FullMatchSearchingKeywords.Add(fullMatch);
                }
            }

            if (FullMatchSearchingKeywords.Count == 0)
            {
                Keyword keyword = new()
                {
                    CourseId = 000000,
                    SubjectName = "Không tìm thấy tên môn này",
                    Color = "#ffffff"
                };
                Discipline discipline = new()
                {
                    Name = "Không tìm thấy mã môn này"
                };
                FullMatchSearchingKeyword fullMatchSearchingKeyword = new();
                fullMatchSearchingKeyword.Keyword = keyword;
                fullMatchSearchingKeyword.Discipline = discipline;
                FullMatchSearchingKeywords.Add(fullMatchSearchingKeyword);
            }
        }

        private void OnDeleteAll()
        {
            List<SubjectModel> actionData = new();
            for (int i = SubjectModels.Count - 1; i >= 0; --i)
            {
                MessageBus.Default.Publish(new DeleteSubjectMessage(SubjectModels[i]));
                SubjectModel restoreSubject = SubjectModels[i].DeepClone();
                actionData.Add(restoreSubject);
                SubjectModels.RemoveAt(i);
            }
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
            _snackbarMessageQueue.Enqueue<List<SubjectModel>>("Đã xoá hết", "HOÀN TÁC", OnRestore, actionData);
        }

        private void OnRestore(List<SubjectModel> obj)
        {
            foreach (SubjectModel subjectModel in obj)
            {
                AddSubjectAndReload(subjectModel);
            }
        }

        private void OnGotoCourse()
        {
            int courseId = _selectedSubjectModel.CourseId;
            string semesterValue = _courseCrawler.GetCurrentSemesterValue();
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
            _openInBrowser.Open(url);
        }

        /// <summary>
        /// Load lại data môn học từ cơ sở dữ liệu lên
        /// </summary>
        private async Task ReloadDisciplineAndKeyWord()
        {
            Disciplines.Clear();
            List<Discipline> disciplines = await _unitOfWork.Disciplines.GetAllIncludeKeywordAsync();
            disciplines.ForEach(discipline => Disciplines.Add(discipline));
            SelectedDiscipline = Disciplines[0];
            LoadDisciplineKeyword(SelectedDiscipline);
        }

        private readonly ImportSessionUC _importSessionUC = new();
        private async Task OnOpenImportDialog()
        {
            ImportSessionViewModel vm = _importSessionUC.DataContext as ImportSessionViewModel;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(_importSessionUC);
            await vm.LoadScheduleSession();
        }

        private async Task CloseDialogAndHandleSessionManagerResult(SessionManagerResult result)
        {
            Cs4rsaMessageBox messageBoxService = new();
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            if (result != null)
            {
                SubjectImporterUC subjectImporterUC = new();
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

            SubjectModel actionData = _selectedSubjectModel.DeepClone();
            
            string message = $"Vừa xoá môn {_selectedSubjectModel.SubjectName}";
            SubjectModels.Remove(_selectedSubjectModel);
            _snackbarMessageQueue.Enqueue(message, "HOÀN TÁC", OnRestoreSubjectModel, actionData);
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
        }

        private void OnRestoreSubjectModel(SubjectModel obj)
        {
            AddSubjectAndReload(obj);
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        public void LoadDisciplineKeyword(Discipline discipline)
        {
            DisciplineKeywordModels.Clear();
            Discipline currentDiscipline = Disciplines.Where(d => d.DisciplineId == discipline.DisciplineId).FirstOrDefault();
            List<Keyword> keywords = currentDiscipline.Keywords;
            keywords.ForEach(keyword => DisciplineKeywordModels.Add(keyword));
            SelectedKeyword = DisciplineKeywordModels[0];
        }

        private async Task OnAddSubjectAsync()
        {
            CanAddSubjectChange(false);

            SubjectDownloadingUC subjectDownloadingUC = new();
            SubjectDownloadingViewModel vm = subjectDownloadingUC.DataContext as SubjectDownloadingViewModel;
            string subjectName = selectedKeyword.SubjectName;
            string subjectCode = selectedDiscipline.Name + " " + selectedKeyword.Keyword1;
            vm.SubjectName = subjectName;
            vm.SubjectCode = subjectCode;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(subjectDownloadingUC);

            Subject subject = await _subjectCrawler.Crawl(selectedDiscipline.Name, selectedKeyword.Keyword1);
            if (subject != null)
            {
                await subject.GetClassGroups();
                SubjectModel subjectModel = await SubjectModel.CreateAsync(subject, _colorGenerator);
                AddSubjectAndReload(subjectModel);
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
                SelectedSubjectModel = SubjectModels.Last();
            }
            else
            {
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
                _snackbarMessageQueue.Enqueue("Môn học không tồn tại trong học kỳ này");
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

        /// <summary>
        /// Thêm một SubjectModel vào danh sách.
        /// Load lại các thông tin cần thiết.
        /// </summary>
        /// <param name="subjectModel"></param>
        private void AddSubjectAndReload(SubjectModel subjectModel)
        {
            SubjectModels.Add(subjectModel);
            TotalSubject = SubjectModels.Count;
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
        }

        public async void Handle(UpdateSuccessMessage message)
        {
            DisciplineKeywordModels.Clear();
            Disciplines.Clear();
            await ReloadDisciplineAndKeyWord();
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
