using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ShowDetailsSubjectViewModel: ViewModelBase
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
