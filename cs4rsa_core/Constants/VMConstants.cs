using Cs4rsa.Utils.Interfaces;

using System;
using System.IO;

namespace Cs4rsa.Constants
{
    internal static class CredizText
    {
        /// <summary>
        /// Có lỗi trong quá trình xử lý <paramref name="handleName"/>
        /// </summary>
        /// <param name="handleName">Tên xử lý</param>
        /// <returns></returns>
        public static string Common001(string handleName) => $"Có lỗi trong quá trình xử lý {handleName}";
        /// <summary>
        /// Có lỗi trong quá trình xử lý <paramref name="handleName"/>
        /// </summary>
        /// <param name="handleName">Tên xử lý</param>
        /// <param name="errMsg">Mô tả lỗi</param>
        /// <returns></returns>
        public static string Common001(string handleName, string errMsg) => $"Có lỗi trong quá trình xử lý {handleName}.\nMô tả: {errMsg}";
        /// <summary>
        /// Thao tác thành công.
        /// </summary>
        /// <param name="actionName">Tên thao tác</param>
        public static string Common002(string actionName) => $"{actionName} thành công";
        public static string ManualMsg001(string name) => $"Đã bỏ chọn lớp {name}";
        public static string ManualMsg002(string subjectName) => $"Đã xoá môn {subjectName}";
        public static string ManualMsg003(string courseId) => $"Không tồn tại {courseId}";
        public static string ManualMsg004() => $"Sai đường dẫn";
        public static string TeacherMsg001(int tcId) => $"Đã sao chép mã {tcId} vào Clipboard";
        /// <summary>
        /// Thông báo: Đã sao chép MSSV vào Clipboard.
        /// </summary>
        /// <param name="stId">Mã số sinh viên</param>
        /// <returns>Thông báo</returns>
        public static string StudentMsg001(string stId) => $"Đã sao chép MSSV {stId} vào Clipboard";
        /// <summary>
        /// Thông báo file của sinh viên không tồn tại.
        /// </summary>
        /// <param name="fName">File name</param>
        /// <param name="stdName">Student name</param>
        public static string AutoMsg001(string fName, string stdName) => $"File {fName} của sinh viên {stdName} không tồn tại.";
        public static string DbMsg001(int result) => $"Hoàn tất cập nhật {result} môn";
        /// <summary>
        /// File chương trình học dự kiến cho mã ngành không tồn tại
        /// </summary>
        /// <param name="currId">Mã ngành</param>
        /// <returns>string</returns>
        public static string DbMsg002(string currId) => $"File chương trình học dự kiến cho mã ngành {currId} không tồn tại";
        public static string PathProgramJsonFile(string stdId) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FdStudentPrograms, $"StudentProgram_{stdId}.json");
        public static string PathPlanJsonFile(int curid) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FdStudentPlans, $"{curid}.json");
        public static string PathHtmlCacheFile(string courseId) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FdHtmlCaches, $"{courseId}.html");
        /// <summary>
        /// Đường dẫn tới hình ảnh hồ sơ sinh viên.
        /// </summary>
        /// <param name="studentId">Mã sinh viên</param>
        /// <returns>Đường dẫn</returns>
        public static string PathStudentProfileImg(string studentId) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FdStudentImgs, $"{studentId}.jpg");
    }

    public abstract class VmConstants
    {
        public const string DbFilePath = "cs4rsa.db";
        public const string DbConnectionString = $@"Data Source={DbFilePath}";

        public const string UnknowErrorException = "Lỗi không xác định";
        public const string InternetConnectException = "Lỗi kết nối mạng";
        public const string SettingDoesNotExistException = "Setting is not exist";
        public const string SchoolClassNameIsDiffException = "SchoolClass's Name is difference!";
        public const string InvalidRegisterCodeExcepion = "Register code is invalid!";
        public const string ClassGroupModelWasNullException = "ClassGroupModel was null!";
        public const string NotFoundClassFormException = "Class Form could not be found!";
        public const string NotFoundBaseSchoolClassModelException = "Base school class model cound not be found";
        public const string NotFoundViewModelException = "ViewModel cound not be found";
        public const string SchoolClassStudyTimeNotOnlyOneException = "SchoolClass's StudyTimes does not have only one item";

        public const string LinkProject = "https://toky0s.github.io/cs4rsa_core/";
        public const string LinkProjectPage = "https://github.com/toky0s/cs4rsa_core";
        public const string LinkProjectGoogleSheet = "https://forms.gle/JHipUM7knjbqKGKWA";

        public const string SnbNotFoundSubjectInThisSemester = "Môn học không tồn tại trong học kỳ này";
        public const string SnbInvalidUnselectSubjectName = "Tên lớp cần bỏ chọn không hợp lệ";
        public const string SnbUnselectAll = "Đã bỏ chọn tất cả";
        public const string SnbDeleteAll = "Đã xoá tất cả";
        public const string SnbInvalidSharestring = "Thông tin nhập bị lỗi";
        public const string SnbAlreadyDownloaded = "Đã được tải xuống";
        public const string SnbCopySuccess = "Đã sao chép";
        public const string SnbSaveToStore = "Đã lưu vào kho";
        public const string SnbAllGen = "Đã gen hết";
        public const string SnbNfRegisterCode = "Lớp này không có mã đăng ký";
        public const string SnbRestore = "HOÀN TÁC";
        public const string SnbGotoStore = "TỚI KHO";

        public const char SeparatorTeacherSubject = '$';
        public const int IntInvalidCourseId = 0;
        public const string StrSpace = " ";
        public const string TimeFormat = "HH:mm";
        public const char CharSpace = ' ';

        public const string StCurrentYearInfo = "CurrentYearInfo";
        public const string StCurrentSemesterInfo = "CurrentSemesterInfo";
        public const string StCurrentYearValue = "CurrentYearValue";
        public const string StCurrentSemesterValue = "CurrentSemesterValue";
        public const string StLastOfScreenIdx = "LastOfScreenIdx";
    }
}
