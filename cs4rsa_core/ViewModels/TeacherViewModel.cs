using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.TeacherCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;
using Cs4rsa.Utils.Interfaces;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.ViewModels
{
    public class TeacherViewModel : ViewModelBase
    {
        private static readonly int SIZE = 20;

        #region Commands
        public RelayCommand OpenOnWebCommand { get; set; }
        public RelayCommand DetailsViewCommand { get; set; }
        public RelayCommand SubjectsViewCommand { get; set; }
        public AsyncRelayCommand UpdateCommand { get; set; }
        public AsyncRelayCommand PreviousPageCommand { get; set; }
        public AsyncRelayCommand NextPageCommand { get; set; }
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ITeacherCrawler _teacherCrawler;
        #endregion

        #region Properties
        public ObservableCollection<TeacherModel> Lectures { get; set; }

        private TeacherModel _selectedTeacher;
        public TeacherModel SelectedTeacher
        {
            get
            {
                return _selectedTeacher;
            }
            set
            {
                _selectedTeacher = value;
                OnPropertyChanged();
                CurrentIndex = 0;
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
                Filter(value);
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; OnPropertyChanged(); }
        }

        private int _totalPage;
        public int TotalPage
        {
            get { return _totalPage; }
            set { _totalPage = value; OnPropertyChanged(); }
        }

        private bool _canPreviousPage;
        public bool CanPreviousPage
        {
            get { return _canPreviousPage; }
            set { _canPreviousPage = value; OnPropertyChanged(); }
        }

        private bool _canNextPage;
        public bool CanNextPage
        {
            get { return _canNextPage; }
            set { _canNextPage = value; OnPropertyChanged(); }
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; OnPropertyChanged(); }
        }

        private bool isUpdating;

        public bool IsUpdating
        {
            get { return isUpdating; }
            set { isUpdating = value; }
        }

        #endregion

        public TeacherViewModel(
            IUnitOfWork unitOfWork,
            ITeacherCrawler teacherCrawler,
            IOpenInBrowser openInBrowser
        )
        {
            SubjectsViewCommand = new RelayCommand(() =>
            {
                CurrentIndex = 1;
            });

            DetailsViewCommand = new RelayCommand(() =>
            {
                CurrentIndex = 0;
            });

            UpdateCommand = new(OnUpdate);
            OpenOnWebCommand = new(OnOpenOnWeb);
            PreviousPageCommand = new(OnPreviousPage);
            NextPageCommand = new(OnNextPage);

            _unitOfWork = unitOfWork;
            _openInBrowser = openInBrowser;
            _teacherCrawler = teacherCrawler;

            Lectures = new();
            CurrentPage = 1;
            CurrentIndex = 0;
            IsUpdating = false;
        }

        private async Task OnUpdate()
        {
            IsUpdating = true;
            TeacherModel teacherModel = await _teacherCrawler.Crawl(_selectedTeacher.Url, VMConstants.INT_INVALID_COURSEID, true);
            int selectedTeacherIndex = Lectures.IndexOf(_selectedTeacher);
            if (selectedTeacherIndex >= 0)
            {
                Lectures.RemoveAt(selectedTeacherIndex);
                Lectures.Insert(selectedTeacherIndex, teacherModel);
                SelectedTeacher = Lectures[selectedTeacherIndex];
            }
            IsUpdating = false;
        }

        private async Task OnPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadTeachers();
            }
        }

        private async Task OnNextPage()
        {
            if (CurrentPage < TotalPage)
            {
                CurrentPage++;
                await LoadTeachers();
            }
        }

        /// <summary>
        /// Tính toán lại Enable của các button điều hướng
        /// </summary>
        private void ReEvaluatePreviousNextButton()
        {
            CanPreviousPage = CurrentPage > 1;
            CanNextPage = CurrentPage < TotalPage;
        }

        private void OnOpenOnWeb()
        {
            _openInBrowser.Open(_selectedTeacher.Url);
        }

        private void Filter(string searchText)
        {
            searchText = searchText.Trim();
            if (searchText == string.Empty)
            {
                LoadTeachers().Wait();
                return;
            }
            IEnumerable<TeacherModel> teachers = _unitOfWork.Teachers
                .GetTeacherByNameOrId(searchText)
                .Select(teacher => new TeacherModel(teacher));
            Lectures.Clear();
            foreach (TeacherModel teacher in teachers)
            {
                Lectures.Add(teacher);
            }
        }

        public async Task LoadTeachers()
        {
            Lectures.Clear();
            TotalPage = await _unitOfWork.Teachers.CountPageAsync(SIZE);
            IAsyncEnumerable<Teacher> teachers = _unitOfWork.Teachers.GetTeachersAsync(_currentPage, SIZE);
            await foreach (Teacher teacher in teachers)
            {
                Lectures.Add(new TeacherModel(teacher));
            }
            ReEvaluatePreviousNextButton();
        }
    }
}
