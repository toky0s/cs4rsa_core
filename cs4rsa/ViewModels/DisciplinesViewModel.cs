using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using cs4rsa.Crawler;
using cs4rsa.Models;

namespace cs4rsa.ViewModels
{
    public class DisciplinesViewModel: INotifyPropertyChanged
    {
        private string selectedDiscipline;
        public string SelectedDiscipline
        {
            get
            {
                return selectedDiscipline;
            }

            set
            {
                selectedDiscipline = value;
                RaisePropertyChanged("SelectedDiscipline");
            }
        }

        private ObservableCollection<DisciplineInfomationModel> disciplines;
        public ObservableCollection<DisciplineInfomationModel> Disciplines
        {
            get
            {
                return disciplines;
            }
        }

        public DisciplinesViewModel()
        {
            List<string> disciplines = HomeCourseSearch.GetDisciplines();
            List<DisciplineInfomationModel> disciplineInfomationModels = disciplines.Select(item => new DisciplineInfomationModel(item)).ToList();
            this.disciplines = new ObservableCollection<DisciplineInfomationModel>(disciplineInfomationModels);
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
