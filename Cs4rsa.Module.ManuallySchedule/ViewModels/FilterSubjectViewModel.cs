using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.UC;
using Cs4rsa.UI.Helper;

using Prism.Mvvm;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class FilterSubjectViewModel : BindableBase
    {
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

        private bool _mon;
        public bool Mon
        {
            get { return _mon; }
            set { SetProperty(ref _mon, value); }
        }

        private bool _tue;
        public bool Tue
        {
            get { return _tue; }
            set { SetProperty(ref _tue, value); }
        }

        private bool _wed;
        public bool Wed
        {
            get { return _wed; }
            set { SetProperty(ref _wed, value); }
        }

        private bool _thu;
        public bool Thu
        {
            get { return _thu; }
            set { SetProperty(ref _thu, value); }
        }

        private bool _fri;
        public bool Fri
        {
            get { return _fri; }
            set { SetProperty(ref _fri, value); }
        }

        private bool _sat;
        public bool Sat
        {
            get { return _sat; }
            set { SetProperty(ref _sat, value); }
        }

        private bool _sun;
        public bool Sun
        {
            get { return _sun; }
            set { SetProperty(ref _sun, value); }
        }

        private ObservableCollection<MultiSelectionItem> _lectures;
        public ObservableCollection<MultiSelectionItem> Lectures
        {
            get { return _lectures; }
            set { SetProperty(ref _lectures, value); }
        }

        public FilterSubjectViewModel()
        {
            
        }
    }
}
