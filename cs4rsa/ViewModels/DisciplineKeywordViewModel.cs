using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using cs4rsa.Crawler;
using cs4rsa.Models;

namespace cs4rsa.ViewModels
{
    public class DisciplineKeywordViewModel : INotifyPropertyChanged
    {
        private DisciplineKeywordModel selectedDisciplineKeyWord;
        public DisciplineKeywordModel SelectedDisciplineKeyWord
        {
            get
            {
                return selectedDisciplineKeyWord;
            }
            set
            {
                selectedDisciplineKeyWord = value;
                RaisePropertyChanged("SelectedDisciplineKeyWord");
            }
        }
        private ObservableCollection<DisciplineKeywordModel> disciplineKeywordModels;
        public ObservableCollection<DisciplineKeywordModel> DisciplineKeywordModels
        {
            get
            {
                return disciplineKeywordModels;
            }
            set
            {
                disciplineKeywordModels = value;
            }
        }
        public DisciplineKeywordViewModel(string discipline)
        {
            List<DisciplineKeywordInfo> disciplineKeywordInfos = HomeCourseSearch.GetDisciplineKeywordInfos(discipline);
            this.disciplineKeywordModels = new ObservableCollection<DisciplineKeywordModel>();
            foreach(DisciplineKeywordInfo disciplineKeywordInfo in disciplineKeywordInfos)
            {
                disciplineKeywordModels.Add(new DisciplineKeywordModel(disciplineKeywordInfo));
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
