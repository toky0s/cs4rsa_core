using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.TeacherCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.ViewModels.Profile
{
    public partial class TeacherViewModel : ViewModelBase
    {
        private static readonly int SIZE = 10;

        #region Commands
        public RelayCommand OpenOnWebCommand { get; set; }
        public RelayCommand DetailsViewCommand { get; set; }
        public RelayCommand SubjectsViewCommand { get; set; }
        public AsyncRelayCommand UpdateCommand { get; set; }
        public RelayCommand PreviousPageCommand { get; set; }
        public RelayCommand NextPageCommand { get; set; }
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ITeacherCrawler _teacherCrawler;
        #endregion

        #region Properties
        private List<Teacher> _cacheAllTeachers;
        public ObservableCollection<TeacherModel> Lectures { get; set; }

        [ObservableProperty]
        private TeacherModel _selectedTeacher;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private int _currentPage;

        [ObservableProperty]
        private long _totalPage;

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
            _cacheAllTeachers = new();

            Lectures = new();
            CurrentPage = 1;
            CurrentIndex = 0;
            IsUpdating = false;
        }

        partial void OnSearchTextChanged(string value)
        {
            value = value.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(value))
            {
                LoadTeachers();
                return;
            }
            IEnumerable<TeacherModel> teachers = _cacheAllTeachers
                .Where(t => StringHelper.ReplaceVietNamese(t.Name.ToLower())
                    .Contains(StringHelper.ReplaceVietNamese(value)))
                .Select(teacher => new TeacherModel(teacher));
            Lectures.Clear();
            foreach (TeacherModel teacher in teachers)
            {
                Lectures.Add(teacher);
            }
        }

        partial void OnSelectedTeacherChanged(TeacherModel value)
        {
            CurrentIndex = 0;
        }

        private async Task OnUpdate()
        {
            IsUpdating = true;
            TeacherModel teacherModel = await _teacherCrawler.Crawl(SelectedTeacher.Url, 0, true);
            int selectedTeacherIndex = Lectures.IndexOf(SelectedTeacher);
            if (selectedTeacherIndex >= 0)
            {
                Lectures.RemoveAt(selectedTeacherIndex);
                Lectures.Insert(selectedTeacherIndex, teacherModel);
                SelectedTeacher = Lectures[selectedTeacherIndex];
            }
            IsUpdating = false;
        }

        private void OnPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadTeachers();
            }
        }

        private void OnNextPage()
        {
            if (CurrentPage < TotalPage)
            {
                CurrentPage++;
                LoadTeachers();
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

        public void LoadTeachers()
        {
            Lectures.Clear();
            _cacheAllTeachers.Clear();

            _cacheAllTeachers = _unitOfWork.Teachers.GetTeachers();
            TotalPage = _unitOfWork.Teachers.CountPage(SIZE);
            List<Teacher> teachers = _unitOfWork.Teachers.GetTeachers(CurrentPage, SIZE);
            foreach (Teacher teacher in teachers)
            {
                Lectures.Add(new TeacherModel(teacher));
            }
            if (Lectures.Any())
            {
                SelectedTeacher = Lectures.First();
            }
            ReEvaluatePreviousNextButton();
        }
    }
}
