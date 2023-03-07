using CommunityToolkit.Mvvm.ComponentModel;
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

namespace Cs4rsa.ViewModels.Profile
{
    internal partial class TeacherViewModel : ViewModelBase
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

        [ObservableProperty]
        private TeacherModel _selectedTeacher;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private int _currentPage;

        [ObservableProperty]
        private int _totalPage;

        [ObservableProperty]
        private bool _canPreviousPage;

        [ObservableProperty]
        private bool _canNextPage;

        [ObservableProperty]
        private int _currentIndex;

        [ObservableProperty]
        private bool _isUpdating;
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

        partial void OnSearchTextChanged(string value)
        {
            Filter(value);
        }

        partial void OnSelectedTeacherChanged(TeacherModel value)
        {
            CurrentIndex = 0;
        }

        private async Task OnUpdate()
        {
            IsUpdating = true;
            TeacherModel teacherModel = await _teacherCrawler.Crawl(SelectedTeacher.Url, VmConstants.IntInvalidCourseId, true);
            int selectedTeacherIndex = Lectures.IndexOf(SelectedTeacher);
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
            _openInBrowser.Open(SelectedTeacher.Url);
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
            IAsyncEnumerable<Teacher> teachers = _unitOfWork.Teachers.GetTeachersAsync(CurrentPage, SIZE);
            await foreach (Teacher teacher in teachers)
            {
                Lectures.Add(new TeacherModel(teacher));
            }
            if (Lectures.Any())
            {
                SelectedTeacher = Lectures.FirstOrDefault();
            }
            ReEvaluatePreviousNextButton();
        }
    }
}
