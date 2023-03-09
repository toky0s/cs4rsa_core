using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Models.Bases;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.Models
{
    /// <summary>
    /// ProgramSubjectModel khác DbProgramSubjects ở chỗ chúng có các phương thức check trong cây diagram
    /// nhằm xác định việc người dùng có thể chọn subject đó hay không, điều mà DbProgramSubjects không thể
    /// làm được vì nó chỉ chứa thông tin của chính nó mà không có tương tác nào với các Folder hay Subject
    /// khác trong cây.
    /// </summary>
    internal partial class ProgramSubjectModel : TreeItem
    {
        public ProgramSubject ProgramSubject { get; set; }

        /// <summary>
        /// Danh sách các ClassGroupModel sau khi được tải.
        /// </summary>
        public ObservableCollection<ClassGroupModel> Cgms { get; set; }

        /// <summary>
        /// Danh sách tên các CGM review trước khi filter.
        /// </summary>
        public ObservableCollection<ClassGroupModel> ReviewFtCgms { get; set; }

        private IEnumerable<Func<ClassGroupModel, bool>> _filterFuncs;
        public IEnumerable<Func<ClassGroupModel, bool>> FilterFuncs
        {
            private get => _filterFuncs;
            set
            {
                _filterFuncs = value;
            }
        }

        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string FolderName { get; set; }

        private int _studyUnit;
        public int StudyUnit
        {
            get => _studyUnit == 0 ? ProgramSubject.StudyUnit : _studyUnit;
            set => _studyUnit = value;
        }

        [ObservableProperty]
        private string _color;

        [ObservableProperty]
        private StudyState _studyState;

        [ObservableProperty]
        private bool _isDone;

        [ObservableProperty]
        private bool _isAvaiable;

        [ObservableProperty]
        private bool _isDownloading;

        /// <summary>
        /// Nếu true, button chọn sẽ bị vô hiệu.
        /// Nếu false, button chọn có thể được click.
        /// </summary>
        [ObservableProperty]
        private bool _isChoosed;

        /// <summary>
        /// Cờ đánh dấu rằng PSM này đã được download
        /// với các SubjectModel và ClassGroupModel bên trong.
        /// </summary>
        [ObservableProperty]
        private bool _isDownloaded;

        /// <summary>
        /// Xác định môn học đã bắt đầu hay chưa
        /// </summary>
        [ObservableProperty]
        private bool _isStarted;

        /// <summary>
        /// Xác định môn học có tồn tại hay không
        /// </summary>
        [ObservableProperty]
        private bool _exists;

        [ObservableProperty]
        private string _status;

        public string CourseId => ProgramSubject.CourseId;
        public string ChildOfNode => ProgramSubject.ChildOfNode;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ColorGenerator _colorGenerator;
        private ProgramSubjectModel(
            ProgramSubject programSubject,
            ColorGenerator colorGenerator,
            IUnitOfWork unitOfWork
        ) : base(programSubject.SubjectName, programSubject.Id)
        {
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;

            Cgms = new();
            ReviewFtCgms = new();
            ProgramSubject = programSubject;
            SubjectCode = programSubject.SubjectCode;
            SubjectName = programSubject.SubjectName;
            FolderName = programSubject.ParentNodeName;
            StudyState = programSubject.StudyState;
            NodeType = programSubject.GetNodeType();

            IsDone = ProgramSubject.IsDone();
            IsDownloading = false;
            IsDownloaded = false;
            IsChoosed = false;
            IsStarted = false;
        }

        /// <summary>
        /// Kiểm tra xem ProgramSubjectModel này có sẵn trong học kỳ này hay không.
        /// </summary>
        private async Task IsAvaiableInThisSemester()
        {
            string[] subjectCodeSlices = ProgramSubject.SubjectCode.Split(new char[] { VmConstants.CharSpace });
            string discipline = subjectCodeSlices[0];
            string keyword1 = subjectCodeSlices[1];
            int count = await _unitOfWork.Keywords.CountAsync(discipline, keyword1);
            IsAvaiable = count > 0;
        }

        private async Task<ProgramSubjectModel> InitializeAsync()
        {
            Color = await _colorGenerator.GetColorAsync(int.Parse(ProgramSubject.CourseId));
            await IsAvaiableInThisSemester();
            return this;
        }

        public static Task<ProgramSubjectModel> CreateAsync(
            ProgramSubject programSubject,
            ColorGenerator colorGenerator,
            IUnitOfWork unitOfWork)
        {
            ProgramSubjectModel ret = new(programSubject, colorGenerator, unitOfWork);
            return ret.InitializeAsync();
        }

        /// <summary>
        /// Thêm danh sách các ClassGroupModel cho ProgramSubjectModel này.
        /// Đánh dấu rằng ProgramSubjectModel này đã được tải xong.
        /// </summary>
        /// <param name="cgms">Danh sách các ClassGroupModel.</param>
        public void AddCgms(IEnumerable<ClassGroupModel> cgms)
        {
            foreach (ClassGroupModel cgm in cgms)
            {
                Cgms.Add(cgm);
            }
        }

        /// <summary>
        /// Sử dụng danh sách các Filter được truyền vào.
        /// Lọc ra danh sách các tên class group model bắt
        /// đầu bằng hậu tố của lớp đó.
        /// 
        /// 1. Kết hợp các filter.
        /// 2. Clear danh sách kết quả filter.
        /// 3. Fill lại danh sách kết quả.
        /// </summary>
        public async Task ApplyFilter()
        {
            await Task.Run(delegate
            {
                IEnumerable<ClassGroupModel> cgms = Cgms.Where(cgm => CombineFilterFuncs(cgm));
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ReviewFtCgms.Clear();
                    foreach (ClassGroupModel cgm in cgms)
                    {
                        ReviewFtCgms.Add(cgm);
                    }
                });
            });
        }

        /// <summary>
        /// Lấy ra danh sách ClassGroupModel sau khi lọc.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClassGroupModel> GetRltAftFilter()
        {
            foreach (ClassGroupModel cgm in ReviewFtCgms)
            {
                yield return Cgms.Where(c => c.ClassSuffix.Equals(cgm.ClassSuffix)).First();
            }
        }

        public void ResetFilter()
        {
            ReviewFtCgms.Clear();
            foreach (ClassGroupModel cgm in Cgms)
            {
                ReviewFtCgms.Add(cgm);
            }
        }

        private bool CombineFilterFuncs(ClassGroupModel cgm)
        {
            if (_filterFuncs == null || !_filterFuncs.Any()) return true;
            foreach (Func<ClassGroupModel, bool> ff in _filterFuncs)
            {
                if (!ff(cgm))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
