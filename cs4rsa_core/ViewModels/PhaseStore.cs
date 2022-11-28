using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.States;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cs4rsa.ViewModels
{
    public class PhaseStore : ViewModelBase
    {
        private readonly bool _isEvaluateBetweenPoint;
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
                ReEvaluateBetweenPointValue();
            }
        }

        private int _currentBetweenPointValue;
        public int CurrentBetweenPointValue
        {
            get { return _currentBetweenPointValue; }
            set { _currentBetweenPointValue = value; OnPropertyChanged(); }
        }


        public PhaseStore()
        {
            _schoolClassModels = new();
            Weeks = new() { 0 };
        }

        private bool CanEvaluateBetweenPoint()
        {

        }

        public void AddClassGroup(ClassGroupModel classGroupModel)
        {
            IEnumerable<string> replacedSchoolClassModels = _schoolClassModels
                .Where(scm => classGroupModel.Name.Contains(scm.SubjectCode))
                .Select(scm => scm.SchoolClassName);

            _schoolClassModels = _schoolClassModels.Where(scm => !replacedSchoolClassModels.Contains(scm.SchoolClassName)).ToList();
            AddSchoolClasses(classGroupModel.CurrentSchoolClassModels);
        }

        /// <summary>
        /// Thêm một danh sách SchoolClassModel vào PhaseStore.
        /// 
        /// Mô tả:
        ///     Sau đó, thực hiện đánh giá lại các Tuần và BetweenPoint.
        /// </summary>
        private void AddSchoolClasses(IEnumerable<SchoolClassModel> schoolClassModels)
        {
            _schoolClassModels.AddRange(schoolClassModels);
            ReEvaluateWeeks();
            ReEvaluateBetweenPointValue();
        }

        public void RemoveAllSchoolClass()
        {
            _schoolClassModels.Clear();
            ReEvaluateWeeks();
            ReEvaluateBetweenPointValue();
        }

        public void RemoveSchoolClassBySubjectCode(string subjectCode)
        {
            int index = _schoolClassModels.FindIndex(scm => scm.SubjectCode.Equals(subjectCode));
            if (index != -1)
            {
                _schoolClassModels.RemoveAt(index);
                ReEvaluateWeeks();
                ReEvaluateBetweenPointValue();
            }
        }

        public void ResetBetweenPoint()
        {
            ReEvaluateBetweenPointValue();
        }

        private void ReEvaluateWeeks()
        {
            Weeks.Clear();
            int minStart = _schoolClassModels.Count > 0 ? _schoolClassModels[0].StudyWeek.StartWeek : 1;
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
                StartWeek = minStart;
                EndWeek = maxEnd;
                for (int i = minStart + 1; i <= maxEnd - 1; i++)
                {
                    Weeks.Add(i);
                }
            }
            else
            {
                StartWeek = 0;
                EndWeek = 0;
                Weeks.Add(0);
            }
        }


        /// <summary>
        /// Đánh giá lại giá trị của BetweenPoint.
        /// 
        /// Mô tả:
        ///     Việc đánh giá sẽ được thực hiện khi có ít nhất một Tuần trong danh sách Tuần.
        ///     Sau khi đánh giá, thông báo PhaseStoreMsgs.BetweenPointChangedMsg.
        /// </summary>
        private void ReEvaluateBetweenPointValue()
        {
            BetweenPointIndex = Weeks.Count / 2;
            if (Weeks.Count > 0)
            {
                CurrentBetweenPointValue = Weeks[BetweenPointIndex];
                Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(Weeks[BetweenPointIndex]));
            }
            else
            {
                CurrentBetweenPointValue = 0;
                Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(0));
            }
        }
    }
}
