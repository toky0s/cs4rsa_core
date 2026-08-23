using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.UC;
using Cs4rsa.UI.Helper;

using Prism.Mvvm;
using Prism.Regions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class FilterInfo
    {
        public string SubjectCode { get; set; }
        public HashSet<DayOfWeek> SelectedDayOfWeeks { get; set; }
        public HashSet<string> LectureNames { get; set; }
    }

    public class FilterSubjectViewModel : BindableBase
    {
        #region Config
        private string _subjectCode;
        public string SubjectCode
        {
            get { return _subjectCode; }
            set { SetProperty(ref _subjectCode, value); }
        }

        private string _subjectName;
        public string SubjectName
        {
            get { return _subjectName; }
            set { SetProperty(ref _subjectName, value); }
        }

        private string _color;
        public string Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        /// <summary>
        /// Cờ hiển thị Filter trên UI, trong trường hợp một Subject 
        /// không còn trong danh sách đã chọn nhưng Filter của nó vẫn phải được giữ nguyên.
        /// </summary>
        private bool _isDisplayed;
        public bool IsDisplayed
        {
            get { return _isDisplayed; }
            set { SetProperty(ref _isDisplayed, value); }
        }
        #endregion

        #region Day of Week
        // ========================= START DAY OF WEEKS ==========================================
        private bool _isBulkUpdating = false;
        private bool _inCodeUpdate = false;
        private bool? _all = true;
        public bool? All
        {
            get { return _all; }

            set
            {
                if (_inCodeUpdate)
                {
                    SetProperty(ref _all, value);
                }
                else
                {
                    if (_all.HasValue)
                    {
                        var newValue = !_all.Value;
                        SetProperty(ref _all, newValue);
                        _isBulkUpdating = true;
                        Mon = Tue = Wed = Thu = Fri = Sat = Sun = newValue;
                        _isBulkUpdating = false;
                        RequestRefresh(); // chỉ gọi 1 lần
                    }
                    else if (value == null)
                    {
                        SetProperty(ref _all, value);
                    }
                }
            }
        }

        private bool _mon;
        public bool Mon
        {
            get { return _mon; }
            set { SetProperty(ref _mon, value); EvaluateAllCheck(); RequestRefresh(); }
        }

        private bool _tue;
        public bool Tue
        {
            get { return _tue; }
            set { SetProperty(ref _tue, value); EvaluateAllCheck(); RequestRefresh(); }
        }

        private bool _wed;
        public bool Wed
        {
            get { return _wed; }
            set { SetProperty(ref _wed, value); EvaluateAllCheck(); RequestRefresh(); }
        }

        private bool _thu;
        public bool Thu
        {
            get { return _thu; }
            set { SetProperty(ref _thu, value); EvaluateAllCheck(); RequestRefresh(); }
        }

        private bool _fri;
        public bool Fri
        {
            get { return _fri; }
            set { SetProperty(ref _fri, value); EvaluateAllCheck(); RequestRefresh(); }
        }

        private bool _sat;
        public bool Sat
        {
            get { return _sat; }
            set { SetProperty(ref _sat, value); EvaluateAllCheck(); RequestRefresh(); }
        }

        private bool _sun;
        public bool Sun
        {
            get { return _sun; }
            set { SetProperty(ref _sun, value); EvaluateAllCheck(); RequestRefresh(); }
        }

        private void EvaluateAllCheck()
        {
            var dayOfWeeks = new bool[7] { Mon, Tue, Wed, Thu, Fri, Sat, Sun };
            var selectedCount = dayOfWeeks.Count(d => d);
            _inCodeUpdate = true;
            if (selectedCount < 7 && selectedCount > 0)
            {
                All = null;
            }
            else if (selectedCount == 0)
            {
                All = false;
            }
            else
            {
                All = true;
            }
            _inCodeUpdate = false;
        }

        public FilterInfo AskRequestFilter()
        {
            var htbDayOfWeeks = new Dictionary<DayOfWeek, bool>()
            {
                {DayOfWeek.Monday, Mon },
                {DayOfWeek.Tuesday, Tue },
                {DayOfWeek.Wednesday, Wed },
                {DayOfWeek.Thursday, Thu },
                {DayOfWeek.Friday, Fri },
                {DayOfWeek.Saturday, Sat },
                {DayOfWeek.Sunday, Sun },
            };

            var selectedDayOfWeeks = htbDayOfWeeks
                .Where(pair => pair.Value)
                .Select(pair => pair.Key)
                .ToHashSet();

            var selectedLectureNames = SelectedLectures.Select(l => l.Label).ToHashSet();

            var filterInfo = new FilterInfo()
            {
                SubjectCode = SubjectCode,
                SelectedDayOfWeeks = selectedDayOfWeeks,
                LectureNames = selectedLectureNames
            };

            return filterInfo;
        }
        // ========================= END DAY OF WEEKS ==========================================
        #endregion

        #region Lectures

        private ObservableCollection<MultiSelectionItem> _lectures;
        public ObservableCollection<MultiSelectionItem> Lectures
        {
            get { return _lectures; }
            set { SetProperty(ref _lectures, value); }
        }

        private ObservableCollection<MultiSelectionItem> _selectedLectures = new ObservableCollection<MultiSelectionItem>();
        public ObservableCollection<MultiSelectionItem> SelectedLectures
        {
            get { return _selectedLectures; }
            set { SetProperty(ref _selectedLectures, value); }
        }
        private void SelectedLectures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Selected Lectures Count: " + SelectedLectures.Count);
            RequestRefresh();
        }
        #endregion

        public FilterSubjectViewModel()
        {
            _isBulkUpdating = true;
            Mon = Tue = Wed = Thu = Fri = Sat = Sun = true;
            _isBulkUpdating = false;
            EvaluateAllCheck();

            _selectedLectures.CollectionChanged += SelectedLectures_CollectionChanged;
        }

        private void RequestRefresh()
        {
            if (!_isBulkUpdating)
            {
                Filter?.Invoke();
            }
        }

        public event Action Filter;
    }
}
