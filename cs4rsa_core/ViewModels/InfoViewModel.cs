﻿using cs4rsa_core.BaseClasses;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Settings.Interfaces;

namespace cs4rsa_core.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        public string Version { get; set; }

        private bool _isChecking;
        public bool IsChecking
        {
            get { return _isChecking; }
            set { _isChecking = value; OnPropertyChanged(); }
        }

        private readonly ISetting _setting;

        public InfoViewModel(ISetting setting)
        {
            _setting = setting;

            Version = _setting.Read("Version");
            IsChecking = false;
        }
    }
}
