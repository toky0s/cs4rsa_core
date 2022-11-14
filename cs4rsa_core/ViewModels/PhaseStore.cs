using cs4rsa_core.BaseClasses;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.ViewModels
{
    public class PhaseStore : ViewModelBase
    {
        private List<SchoolClassModel> _schoolClassModels;

        public ObservableCollection<int> Weeks { get; set; }

        private int _startWeek;

        public int StartWeek
        {
            get { return _startWeek; }
            set 
            { 
                _startWeek = value; 
                OnPropertyChanged(); 
                ReEvaluateWeeks();
                ReEvaluateBetweenPointIndex();
            }
        }

        private int _endWeek;

        public int EndWeek
        {
            get { return _endWeek; }
            set 
            { 
                _endWeek = value; 
                OnPropertyChanged();
                ReEvaluateWeeks();
                ReEvaluateBetweenPointIndex();
            }
        }

        private int _betweenPointIndex;

        public int BetweenPointIndex
        {
            get { return _betweenPointIndex; }
            set 
            { 
                if (value < 0 || value > Weeks.Count - 1)
                {
                    _betweenPointIndex = 0;
                }
                _betweenPointIndex = value;
                OnPropertyChanged();
            }
        }

        public PhaseStore()
        {
            _schoolClassModels = new();
            Weeks = new();
        }

        public void AddClassGroup(ClassGroupModel classGroupModel)
        {
            IEnumerable<string> replacedSchoolClassModels = _schoolClassModels
                .Where(scm => classGroupModel.Name.Contains(scm.SubjectCode))
                .Select(scm => scm.SchoolClassName);

            _schoolClassModels = _schoolClassModels.Where(scm => !replacedSchoolClassModels.Contains(scm.SchoolClassName)).ToList();
            AddSchoolClasses(classGroupModel.CurrentSchoolClassModels);
        }

        private void AddSchoolClass(SchoolClassModel schoolClassModel)
        {
            _schoolClassModels.Add(schoolClassModel);
            ReEvaluateWeeks();
            ReEvaluateBetweenPointIndex();
        }

        private void AddSchoolClasses(IEnumerable<SchoolClassModel> schoolClassModels)
        {
            _schoolClassModels.AddRange(schoolClassModels);
            ReEvaluateWeeks();
            ReEvaluateBetweenPointIndex();
        }

        public void RemoveAllSchoolClass()
        {
            _schoolClassModels.Clear();
            ReEvaluateWeeks();
            ReEvaluateBetweenPointIndex();
        }

        public void RemoveSchoolClassBySubjectCode(string subjectCode)
        {
            int index = _schoolClassModels.FindIndex(scm => scm.SubjectCode.Equals(subjectCode));
            if (index != -1)
            {
                _schoolClassModels.RemoveAt(index);
                ReEvaluateWeeks();
                ReEvaluateBetweenPointIndex();
            }
        }

        private void ReEvaluateWeeks()
        {
            Weeks.Clear();
            int minStart = _schoolClassModels.Count > 0 ? _schoolClassModels[0].StudyWeek.StartWeek : 0;
            int maxEnd = _schoolClassModels.Count > 0 ? _schoolClassModels[0].StudyWeek.EndWeek : 0;

            if (_schoolClassModels.Count > 0)
            {
                for (int i = 1; i < _schoolClassModels.Count; i++)
                {
                    minStart = minStart < _schoolClassModels[i].StudyWeek.StartWeek
                            ? minStart
                            : _schoolClassModels[i].StudyWeek.StartWeek;

                    maxEnd = maxEnd > _schoolClassModels[i].StudyWeek.EndWeek
                             ? maxEnd
                             : _schoolClassModels[i].StudyWeek.EndWeek;
                }
                for (int i = minStart; i <= maxEnd; i++)
                {
                    Weeks.Add(i);
                }
            }
        }

        private void ReEvaluateBetweenPointIndex()
        {
            BetweenPointIndex = Weeks.Count / 2;
        }
    }
}
