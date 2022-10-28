﻿using CommunityToolkit.Mvvm.Input;

using cs4rsa_core.BaseClasses;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Utils.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.ViewModels
{
    public class LectureViewModel: ViewModelBase
    {
        private static readonly int SIZE = 20;

        public RelayCommand OpenOnWebCommand { get; set; }
        public AsyncRelayCommand PreviousPageCommand { get; set; }
        public AsyncRelayCommand NextPageCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;

        public ObservableCollection<Teacher> Lectures { get; set; }

        private Teacher _selectedTeacher;
        public Teacher SelectedTeacher
        {
            get
            {
                return _selectedTeacher;
            }
            set
            {
                _selectedTeacher = value;
                OnPropertyChanged();
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



        public LectureViewModel(
            IUnitOfWork unitOfWork,
            IOpenInBrowser openInBrowser
        )
        {
            OpenOnWebCommand = new(OnOpenOnWeb);
            PreviousPageCommand = new(OnPreviousPage);
            NextPageCommand = new(OnNextPage);

            _unitOfWork = unitOfWork;
            _openInBrowser = openInBrowser;

            Lectures = new();
            CurrentPage = 1;
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
        /// Tính toán lại Enable của các 
        /// </summary>
        private void ReEvaluatePreviousNextButton()
        {
            CanPreviousPage = CurrentPage > 1;
            CanNextPage = CurrentPage < TotalPage;
        }

        private void OnOpenOnWeb()
        {
            string url = $"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&intructorid={SelectedTeacher.TeacherId}";
            _openInBrowser.Open(url);
        }

        private void Filter(string searchText)
        {
            searchText = searchText.Trim();
            if (searchText == string.Empty)
            {
                LoadTeachers().Wait();
                return;
            }
            IEnumerable<Teacher> teachers = _unitOfWork.Teachers.GetTeacherByNameOrId(searchText);
            Lectures.Clear();
            foreach (Teacher teacher in teachers)
            {
                Lectures.Add(teacher);
            }
        }

        public async Task LoadTeachers()
        {
            Lectures.Clear();
            TotalPage = await _unitOfWork.Teachers.CountPageAsync(SIZE, t => true);
            IAsyncEnumerable<Teacher> teachers = _unitOfWork.Teachers.GetTeachersAsync(_currentPage, SIZE);
            await foreach (Teacher teacher in teachers)
            {
                Lectures.Add(teacher);
            }
            ReEvaluatePreviousNextButton();
        }
    }
}
