﻿using cs4rsa_core.BaseClasses;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ShowDetailsSubjectViewModel : ViewModelBase
    {
        private SubjectModel _subjectModel;

        public SubjectModel SubjectModel
        {
            get { return _subjectModel; }
            set { _subjectModel = value; OnPropertyChanged(); }
        }
        public ShowDetailsSubjectViewModel()
        {

        }
    }
}
