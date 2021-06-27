using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using cs4rsa.Crawler;

namespace cs4rsa.ViewModels
{
    class AutoScheduleViewModel: NotifyPropertyChangedBase
    {
        private string _name;
        private string _studentId;
        private DateTime _birthday;
        private string _cmnd;
        private string _email;
        private string _phoneNumber;
        private string _address;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }
        public string StudentId
        {
            get
            {
                return _studentId;
            }
            set
            {
                _studentId = value;
                RaisePropertyChanged();
            }
        }
        public DateTime Birthday
        {
            get
            {
                return _birthday;
            }
            set
            {
                _birthday = value;
                RaisePropertyChanged();
            }
        }
        public string CMND
        {
            get
            {
                return _cmnd;
            }
            set
            {
                _cmnd = value;
                RaisePropertyChanged();
            }
        }
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                RaisePropertyChanged();
            }
        }
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                RaisePropertyChanged();
            }
        }
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                RaisePropertyChanged();
            }
        }

        private string _specialString;
        private string _sessionId;

        public AutoScheduleViewModel(string specialString, string sessionId=null)
        {
            _specialString = specialString;
            _sessionId = sessionId;
        }
    }
}
