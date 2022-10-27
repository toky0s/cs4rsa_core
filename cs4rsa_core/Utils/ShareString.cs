using cs4rsa_core.Constants;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cs4rsa_core.Utils
{
    /// <summary>
    /// ShareString là một tính năng quan trọng giúp các
    /// sinh viên chia sẻ lịch học mà họ đã sắp xếp thông qua
    /// các phương tiện truyền thông, giúp giảm thời gian lựa chọn
    /// môn học cũng như nhóm lớp.
    /// </summary>
    public class ShareString
    {
        #region DI
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public ShareString(
            IUnitOfWork unitOfWork, 
            ICourseCrawler courseCrawler,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
        }
        public string GetShareString(IEnumerable<ClassGroupModel> classGroupModels)
        {
            if (!classGroupModels.Any())
            {
                return string.Empty;
            }
            IEnumerable<string> SubjectHasses = classGroupModels.Select(item => SubjectCodeVsRegisterCode(item));
            string SubjectHassesJoin = string.Join("?", SubjectHasses);
            string Count = classGroupModels.Count().ToString();
            string currentYear = _courseCrawler.GetCurrentYearValue();
            string currentSemester = _courseCrawler.GetCurrentSemesterValue();
            string raw = $"cs4rsa!{currentYear}!{currentSemester}!{Count}!{SubjectHassesJoin}".Replace(' ', '$');
            return StringHelper.EncodeTo64(raw);
        }

        public async Task<SessionManagerResult> GetSubjectFromShareString(string shareString)
        {
            try
            {
                shareString = StringHelper.DecodeFrom64(shareString);
                string[] shareStringSlices = shareString.Split(new char[] { '!' });

                string subjectHasses = shareStringSlices[4].Replace('$', ' ');
                string[] subjectHassesSlices = subjectHasses.Split(new char[] { '?' });
                List<SubjectInfoData> subjectInfoDatas = new();
                foreach (string item in subjectHassesSlices)
                {
                    string[] infoes = item.Split(new char[] { '|' });
                    string subjectCode = infoes[0]; 
                    string classGroupName = infoes[1];
                    string registerCode = infoes[2];

                    string discipline = subjectCode.Split(new char[] { ' ' })[0];
                    string keyword1 = subjectCode.Split(new char[] { ' ' })[1];
                    Keyword keyword = await _unitOfWork.Keywords.GetKeyword(discipline, keyword1);
                    string subjectName = keyword.SubjectName;
                    SubjectInfoData subjectInfoData = new()
                    {
                        SubjectCode = subjectCode,
                        ClassGroup = classGroupName,
                        SubjectName = subjectName,
                        RegisterCode = registerCode
                    };
                    subjectInfoDatas.Add(subjectInfoData);
                }
                return new SessionManagerResult(subjectInfoDatas);
            }
            catch
            {
                _snackbarMessageQueue.Enqueue(VMConstants.SNB_INVALID_SHARESTRING);
                return null;
            }
        }

        private static string SubjectCodeVsRegisterCode(ClassGroupModel classGroupModel)
        {
            return classGroupModel.SubjectCode + "|" + classGroupModel.Name + "|" + classGroupModel.CurrentRegisterCode;
        }
    }
}
