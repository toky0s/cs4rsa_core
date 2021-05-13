using cs4rsa.BaseClasses;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace cs4rsa.ViewModels
{
    /// <summary>
    /// ViewModel này đại diện cho phần Search Môn học.
    /// </summary>
    public class DisciplinesViewModel : NotifyPropertyChangedBase
    {
        private Cs4rsaData cs4rsaData = new Cs4rsaData();
        //Add button
        public MyICommand AddCommand { get; set; }
        private bool canRunAddCommand = false;
        public bool CanRunAddCommand
        {
            get
            {
                return canRunAddCommand;
            }
            set
            {
                canRunAddCommand = value;
                RaisePropertyChanged();
            }
        }
        public MyICommand DeleteCommand { get; set; }

        //ComboBox discipline
        private DisciplineInfomationModel selectedDiscipline;
        public DisciplineInfomationModel SelectedDiscipline
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

        //ComboxBox keyword
        private DisciplineKeywordModel selectedKeyword;
        public DisciplineKeywordModel SelectedKeyword
        {
            get
            {
                return selectedKeyword;
            }
            set
            {
                selectedKeyword = value;
                AddCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("SelectedKeyword");
            }
        }

        private ObservableCollection<DisciplineKeywordModel> disciplineKeywordModels = new ObservableCollection<DisciplineKeywordModel>();
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

        //ListBox downloaded subjects
        private ObservableCollection<SubjectModel> subjectModels = new ObservableCollection<SubjectModel>();
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

        private int totalSubject = 0;
        public int TotalSubject
        {
            get
            {
                return totalSubject;
            }
            set
            {
                totalSubject = value;
                RaisePropertyChanged();
            }
        }

        private SubjectModel selectedSubjectModel;
        public SubjectModel SelectedSubjectModel
        {
            get
            {
                return selectedSubjectModel;
            }
            set
            {
                selectedSubjectModel = value;
                DeleteCommand.RaiseCanExecuteChanged();
                MessageBus.Default.Publish(new SelectedSubjectChangeMessage(value));
            }
        }


        //Total Credits
        private int totalCredits = 0;
        public int TotalCredits
        {
            get
            {
                return totalCredits;
            }
            set
            {
                totalCredits = value;
                RaisePropertyChanged();
            }
        }

        public DisciplinesViewModel()
        {
            List<string> disciplines = cs4rsaData.GetDisciplines();
            List<DisciplineInfomationModel> disciplineInfomationModels = disciplines.Select(item => new DisciplineInfomationModel(item)).ToList();
            this.disciplines = new ObservableCollection<DisciplineInfomationModel>(disciplineInfomationModels);
            AddCommand = new MyICommand(OnAddSubject, CanAddSubject);
            DeleteCommand = new MyICommand(OnDeleteSubject, CanDeleteSubject);
            SelectedDiscipline = this.disciplines[0];
        }

        private bool CanDeleteSubject()
        {
            return selectedSubjectModel != null;
        }

        private void OnDeleteSubject()
        {
            MessageBus.Default.Publish(new DeleteSubjectMessage(selectedSubjectModel));
            subjectModels.Remove(selectedSubjectModel);
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        public void LoadDisciplineKeyword(string discipline)
        {
            disciplineKeywordModels.Clear();
            foreach (DisciplineKeywordModel item in cs4rsaData.GetDisciplineKeywordModels(discipline))
            {
                disciplineKeywordModels.Add(item);
            }
            SelectedKeyword = disciplineKeywordModels[0];
        }

        private void OnAddSubject()
        {
            CanRunAddCommand = false;
            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
            };
            backgroundWorker.DoWork += WorkerDownloadSubject;
            backgroundWorker.RunWorkerCompleted += WorkerComplete;
            SubjectCrawler subjectCrawler = new SubjectCrawler(selectedDiscipline.Discipline, selectedKeyword.Keyword1);
            backgroundWorker.RunWorkerAsync(subjectCrawler);
        }

        private bool CanAddSubject()
        {
            List<string> courseIds = subjectModels.Select(item => item.CourseId).ToList();
            if (SelectedKeyword == null)
            {
                return true;
            }
            if (courseIds.Contains(SelectedKeyword.CourseID))
            {
                CanRunAddCommand = false;
                return false;
            }

            else
            {
                CanRunAddCommand = true;
                return true;
            }
        }

        private void WorkerDownloadSubject(object sender, DoWorkEventArgs e)
        {
            SubjectCrawler subjectCrawler = (SubjectCrawler)e.Argument;
            SubjectModel subjectModel = new SubjectModel(subjectCrawler.ToSubject());
            e.Result = subjectModel;
        }

        private void WorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            CanRunAddCommand = false;
            subjectModels.Add((SubjectModel)e.Result);
            TotalSubject = subjectModels.Count;
            UpdateCreditTotal();
            UpdateSubjectAmount();
        }

        private void UpdateSubjectAmount()
        {
            TotalSubject = subjectModels.Count;
        }

        private void UpdateCreditTotal()
        {
            TotalCredits = 0;
            foreach (SubjectModel subject in subjectModels)
            {
                TotalCredits += subject.StudyUnit;
            }
        }
    }
}
