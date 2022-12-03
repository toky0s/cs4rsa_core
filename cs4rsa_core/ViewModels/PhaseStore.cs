using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.States;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.ViewModels.Interfaces;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.ViewModels
{
    /// <summary>
    /// Rule đánh giá Start Week, EndWeek, Range Week, Between Point.
    /// 
    /// Chỉ đánh giá lại Start và End Week nếu giá trị tuần
    /// đó lớn hơn End hoặc nhỏ hơn Start.
    /// 
    /// Range Week sẽ chỉ đánh giá lại nếu Week Value không
    /// tồn tại trong Range Week.
    /// 
    /// Between Point sẽ được đánh giá ngay lần thêm ClassGroupModel(s) đầu tiên.
    /// Between Point sẽ được đánh giá lại nếu nó nằm ngoài Range Week do các thao
    /// tác Remove(s) tạo nên.
    /// </summary>
    internal class PhaseStore : ViewModelBase, IPhaseStore
    {
        private bool _isEvaluatedBwp = false;
        private List<ClassGroupModel> _classGroupModels;

        public ObservableCollection<int> Weeks { get; set; }

        private int _bwpValue;
        public int CurrentBetweenPointValue
        {
            get { return _bwpValue; }
            set { _bwpValue = value; OnPropertyChanged(); }
        }

        public int BetweenPoint => _bwpValue;

        public PhaseStore()
        {
            _classGroupModels = new();
            Weeks = new() { 0 };
        }

        public void AddClassGroupModel(ClassGroupModel classGroupModel)
        {
            _classGroupModels = _classGroupModels.Where(cgm => !cgm.SubjectCode.Equals(classGroupModel.SubjectCode)).ToList();
            _classGroupModels.Add(classGroupModel);

            EvaluateWeek(classGroupModel);
            EvaluateBetweenPoint();
        }

        private void EvaluateWeek(ClassGroupModel classGroupModel)
        {
            List<SchoolClassModel> schoolClassModels = classGroupModel.CurrentSchoolClassModels;
            foreach (SchoolClassModel scm in schoolClassModels)
            {
                int start = scm.StudyWeek.StartWeek;
                int end = scm.StudyWeek.EndWeek;
                AddWeek(start);
                AddWeek(end);
            }
        }

        public void AddClassGroupModels(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel item in classGroupModels)
            {
                AddClassGroupModel(item);
            }
        }

        public void RemoveClassGroup(ClassGroupModel classGroupModel)
        {
            RemoveClassGroupBySubjectCode(classGroupModel.SubjectCode);
        }

        public void RemoveClassGroups(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                RemoveClassGroup(classGroupModel);
            }
        }

        public void RemoveAll()
        {
            Weeks.Clear();
            _classGroupModels.Clear();
            _bwpValue = 0;
        }

        public void EvaluateBetweenPoint()
        {
            if (Weeks.Contains(_bwpValue))
            {
                return;
            }

            if (_isEvaluatedBwp)
            {
                return;
            }
            else if (Weeks.Count == 0)
            {
                _bwpValue = 0;
            }
            else if (Weeks.Count == 1)
            {
                _bwpValue = Weeks[0];
            }
            else if (Weeks.Count == 2)
            {
                _bwpValue = Weeks[1];
            }
            else
            {
                int index = Weeks.Count / 2;
                _bwpValue = Weeks[index];
            }

            Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(_bwpValue));
            _isEvaluatedBwp = true;
        }

        public void AddWeek(int week)
        {
            if (Weeks.Contains(week))
            {
                return;
            }

            if (Weeks.Count == 0)
            {
                Weeks.Add(week);
                return;
            }

            if (week > Weeks[^1])
            {
                Weeks.Add(week);
                return;
            }
            else if (week < Weeks[0])
            {
                Weeks.Insert(0, week);
            }
            else // In range Start to End but is not contained in Weeks.
            {
                for (int i = 1; i < Weeks.Count; i++)
                {
                    if (Weeks[i] > week)
                    {
                        Weeks.Insert(i - 1, week);
                        break;
                    }
                }
            }
        }

        public void RemoveClassGroup(SubjectModel subjectModel)
        {
            RemoveClassGroupBySubjectCode(subjectModel.SubjectCode);
        }

        public void RemoveClassGroupBySubjectCode(string subjectCode)
        {
            for (int i = 0; i < _classGroupModels.Count; i++)
            {
                if (_classGroupModels[i].SubjectCode.Equals(subjectCode))
                {
                    _classGroupModels.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
