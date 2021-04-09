using cs4rsa.BaseClasses;
using cs4rsa.Crawler;
using cs4rsa.Models;
using cs4rsa.ViewModels;
using cs4rsa.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using cs4rsa.Database;
using LightMessageBus;
using cs4rsa.Messages;


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

        private bool canRunAddCommand = true;
        public bool CanRunAddCommand
        {
            get
            {
                return canRunAddCommand;
            }
            set
            {
                canRunAddCommand = value;
                RaisePropertyChanged("CanRunAddCommand");
            }
        }

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
                AddCommand.RaiseCanExecuteChanged();
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
                CheckCanRunAddCommand();
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
                RaisePropertyChanged("DisciplineKeywordModels");
            }
        }

        //ListBox downloaded subjects
        private ObservableCollection<SubjectModel> subjectModels =  new ObservableCollection<SubjectModel>();
        public ObservableCollection<SubjectModel> SubjectModels
        {
            get
            {
                return subjectModels;
            }
            set
            {
                subjectModels = value;
                RaisePropertyChanged("SubjectModels");
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
                MessageBus.Default.Publish(new SelectedSubjectChangeMessage(this));
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
            SelectedDiscipline = this.disciplines[0];
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        public void LoadDisciplineKeyword(string discipline)
        {
            disciplineKeywordModels.Clear();
            foreach(DisciplineKeywordModel item in cs4rsaData.GetDisciplineKeywordModels(discipline))
            {
                disciplineKeywordModels.Add(item);
            }
            SelectedKeyword = disciplineKeywordModels[0];
        }

        private void OnAddSubject()
        {
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
            if (selectedDiscipline != null &&
                selectedKeyword != null)
            {
                return true;
            }
            return false;
        }

        private void WorkerDownloadSubject(object sender, DoWorkEventArgs e)
        {
            CanRunAddCommand = false;
            SubjectCrawler subjectCrawler = (SubjectCrawler)e.Argument;
            SubjectModel subjectModel = new SubjectModel(subjectCrawler.ToSubject());
            e.Result = subjectModel;
        }

        private void WorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            subjectModels.Add((SubjectModel)e.Result);
            CheckCanRunAddCommand();
            TotalSubject = subjectModels.Count();

            //total credit
            TotalCredits = 0;
            foreach (SubjectModel subject in subjectModels)
            {
                TotalCredits += subject.StudyUnit;
            }
        }

        private void CheckCanRunAddCommand()
        {
            List<string> courseIds = subjectModels.Select(item => item.CourseId).ToList();
            if (selectedKeyword != null)
            {
                if (courseIds.Contains(selectedKeyword.CourseID))
                    CanRunAddCommand = false;
                else
                    CanRunAddCommand = true;
            }
        }
    }
}
