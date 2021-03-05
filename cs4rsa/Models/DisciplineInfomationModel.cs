using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace cs4rsa.Models
{
    public class DisciplineInfomationModel: INotifyPropertyChanged
    {
        private string discipline;

        public string Discipline
        {
            get
            {
                return discipline;
            }
            set
            {
                discipline = value;
                RaisePropertyChanged("Discipline");
            }
        }
        public DisciplineInfomationModel(string discipline)
        {
            this.discipline = discipline;
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
