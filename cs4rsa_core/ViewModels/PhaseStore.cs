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
        private List<SchoolClassModel> _schoolClassModels;

        public ObservableCollection<int> Weeks { get; set; }

        private int _startWeek;
        /// <summary>
        /// Tuần bắt đầu
        /// </summary>
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
        /// <summary>
        /// Tuần kết thúc
        /// </summary>
        public int EndWeek
        {
            get { return _endWeek; }
            set
            {
                _endWeek = value;
                OnPropertyChanged();
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

        /// <summary>
        /// Ngay lần đầu thêm ClassGroupModel
        /// - Đánh giá là Range tuần
        /// - Đánh giá BetweenPointValue
        /// 
        /// </summary>
        /// <param name="classGroupModel"></param>
        public void AddClassGroup(ClassGroupModel classGroupModel)
        {
            IEnumerable<string> replacedSchoolClassModels = _schoolClassModels
                .Where(scm => classGroupModel.Name.Contains(scm.SubjectCode))
                .Select(scm => scm.SchoolClassName);

            _schoolClassModels = _schoolClassModels.Where(scm => !replacedSchoolClassModels.Contains(scm.SchoolClassName)).ToList();
            _schoolClassModels.AddRange(classGroupModel.CurrentSchoolClassModels);

            if (CanEvaluate(classGroupModel))
            {
                ReEvaluateWeeks();
            }

            if (CanEvaluateBetweenPoint())
            {
                ReEvaluateBetweenPointValue();
            }
        }

        /// <summary>
        /// Xem xét việc có thể đánh giá lại Week.
        /// </summary>
        /// <returns>
        /// Week sẽ được đánh giá lại nếu các Week của ClassGroupModel
        /// không tồn tại trong Range Weeks.
        /// </returns>
        public bool CanEvaluate(ClassGroupModel classGroupModel)
        {
            foreach (SchoolClassModel scm in classGroupModel.CurrentSchoolClassModels)
            {
                if (scm.StudyWeek.StartWeek < StartWeek || scm.StudyWeek.EndWeek > EndWeek)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  Xem xét việc có thể đánh giá lại BetweenPoint.
        /// </summary>
        /// <returns>
        /// BetweenPoint sẽ được đánh giá lại khi:
        /// - BetweenPoint hiện tại nằm ngoài Range Weeks
        /// - Range Weeks bị reset
        /// </returns>
        public bool CanEvaluateBetweenPoint()
        {
            if (!Weeks.Contains(_currentBetweenPointValue))
            {
                return true;
            }
            return false;
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

        /// <summary>
        /// Đánh giá lại Range Week, Start Week, End Week.
        /// </summary>
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
            CurrentBetweenPointValue = CurrentBetweenPointValue;
        }

        public void ResetBetweenPoint()
        {
            ReEvaluateBetweenPointValue(isFromExternal: true);
        }

        /// <summary>
        /// Đánh giá lại giá trị của BetweenPoint.
        /// </summary>
        private void ReEvaluateBetweenPointValue(bool isFromExternal=false)
        {
            if (!isFromExternal)
            {
                if (!CanEvaluateBetweenPoint()) return;
            }
            int index = Weeks.Count / 2;
            if (Weeks.Count > 0)
            {
                CurrentBetweenPointValue = Weeks[index];
                Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(Weeks[index]));
            }
            else
            {
                CurrentBetweenPointValue = 0;
                Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(0));
            }
        }
    }
}
