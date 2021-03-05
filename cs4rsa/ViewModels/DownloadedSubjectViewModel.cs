using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using cs4rsa.Models;


namespace cs4rsa.ViewModels
{
    public class DownloadedSubjectViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SubjectModel> subjectModels;
        public ObservableCollection<SubjectModel> SubjectModels
        {
            get
            {
                return subjectModels;
            }
            set
            {
                subjectModels = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
