using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.States;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

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
    /// 
    /// Thao tác Add ClassGroup Model(s) không đánh giá lại BetweenPoint từ lần thứ hai trở đi.
    /// </summary>
    internal sealed partial class PhaseStore : ViewModelBase
    {
        private List<ClassGroupModel> _classGroupModels;
        public ObservableCollection<int> BwpWeeks { get; set; }

        [ObservableProperty]
        private int _bwpValue;

        [ObservableProperty]
        private int _start;

        [ObservableProperty]
        private int _end;

        public PhaseStore()
        {
            _classGroupModels = new();
            BwpWeeks = new();
            BwpValue = 0;
        }

        partial void OnBwpValueChanged(int value)
        {
            Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(value));
        }

        public void AddClassGroupModel(ClassGroupModel classGroupModel)
        {
            _classGroupModels = _classGroupModels.Where(cgm => !cgm.SubjectCode.Equals(classGroupModel.SubjectCode)).ToList();
            _classGroupModels.Add(classGroupModel);
            EvaluateWeek();
            EvaluateBetweenPoint();
        }

        public void AddClassGroupModels(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel item in classGroupModels)
            {
                _classGroupModels = _classGroupModels.Where(cgm => !cgm.SubjectCode.Equals(item.SubjectCode)).ToList();
                _classGroupModels.Add(item);
            }

            EvaluateWeek();
            EvaluateBetweenPoint();
        }

        public void RemoveClassGroup(ClassGroupModel classGroupModel)
        {
            RemoveClassGroupBySubjectCode(classGroupModel.SubjectCode);
            EvaluateWeek();
            EvaluateBetweenPoint();

        }

        public void RemoveClassGroups(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                RemoveClassGroup(classGroupModel);
            }
            EvaluateWeek();
            EvaluateBetweenPoint();
        }

        public void RemoveAll()
        {
            _classGroupModels.Clear();
            BwpWeeks.Clear();
            Start = 0;
            End = 0;
            BwpValue = 0;
        }

        /// <summary>
        /// Đánh giá lại Week range dựa theo những class group model còn lại.
        /// </summary>
        private void EvaluateWeek()
        {
            int start = 0;
            int end = 0;
            foreach (ClassGroupModel cgm in _classGroupModels)
            {
                foreach (SchoolClassModel scm in cgm.CurrentSchoolClassModels)
                {
                    int scmStart = scm.StudyWeek.StartWeek;
                    int scmEnd = scm.StudyWeek.EndWeek;
                    if (scmEnd > end)
                    {
                        end = scmEnd;
                    }
                    if (start == 0 || scmStart < start)
                    {
                        start = scmStart;
                    }
                }
            }

            if (Start == start && End == end) return;

            BwpWeeks.Clear();
            Start = start;
            End = end;

            // Không thực hiện Render nếu một trong hai điểm bằng 0
            if (Start == 0 || End == 0) return;
            for (int i = start; i <= end; i++)
            {
                BwpWeeks.Add(i);
            }
        }

        public void EvaluateBetweenPoint()
        {
            if (BwpWeeks.Count == 0)
            {
                BwpValue = 0;
            }
            else
            {
                BwpValue = BwpWeeks[(BwpWeeks.Count - 1) / 2];
            }
            Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(BwpValue));
        }

        /// <summary>
        /// Loại bỏ class group model.
        /// <br></br>
        /// Loại bỏ class group model thực hiện đánh giá lại week.
        /// Đánh giá lại between point nếu nó out of range.
        /// </summary>
        /// <param name="subjectModel">Subject model.</param>
        public void RemoveClassGroup(SubjectModel subjectModel)
        {
            RemoveClassGroupBySubjectCode(subjectModel.SubjectCode);
            EvaluateWeek();
            if (IsBwpOutOfWeekRange(BwpValue, BwpWeeks))
            {
                EvaluateBetweenPoint();
            }
        }

        /// <summary>
        /// Loại bỏ Class Group Model bằng subject code.
        /// </summary>
        /// <param name="subjectCode">Subject code.</param>
        private void RemoveClassGroupBySubjectCode(string subjectCode)
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

        /// <summary>
        /// Kiểm tra xem between point có nằm ngoài week range hay không?
        /// </summary>
        /// <param name="bwpIndex">Between Point Index</param>
        /// <param name="weekRange">Week range chạy start tới end với step +1</param>
        /// <returns>Trả về true nếu nằm ngoài, ngược lại trả về false.</returns>
        private static bool IsBwpOutOfWeekRange(int bwtValue, IList<int> weekRange)
        {
            if (bwtValue == 0) return true;
            if (weekRange.Count == 0) return false;
            return !weekRange.Contains(bwtValue);
        }
    }
}
