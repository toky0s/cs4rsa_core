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
            BwpValue = -1;
        }

        partial void OnBwpValueChanged(int value)
        {
            if (value != -1)
            {
                Messenger.Send(new PhaseStoreMsgs.BetweenPointChangedMsg(value));
            }
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

        public void RemoveAll()
        {
            _classGroupModels.Clear();
            CleanBwpWeeks();
            Start = 0;
            End = 0;
        }

        public void EvaluateBetweenPoint()
        {
            if (BwpWeeks.Count == 0)
            {
                BwpValue = 0;
            }
            else
            {
                int tempBwpValue = BwpWeeks[(BwpWeeks.Count - 1) / 2];
                if (tempBwpValue != BwpValue)
                {
                    BwpValue = tempBwpValue;
                }
            }
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
            EvaluateBetweenPoint();
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

            CleanBwpWeeks();
            Start = start;
            End = end;
            for (int i = start; i <= end; i++)
            {
                BwpWeeks.Add(i);
            }
        }

        /// <summary>
        /// Combobox luôn giữ các tham chiếu tới các item mà nó đang chứa
        /// với selected item. Nếu danh sách item bị clean rồi sau đó được
        /// set lại với tập item khác, dù cho selected item có thuộc tập
        /// item mới thì combobox cũng không thể binding để hiển thị được
        /// , nên sau thao tác clean item của combobox phải ngay lập tức
        /// set selected item của combobox thành giá trị khác.
        /// </summary>
        private void CleanBwpWeeks()
        {
            BwpWeeks.Clear();
            BwpValue = -1;
        }
    }
}