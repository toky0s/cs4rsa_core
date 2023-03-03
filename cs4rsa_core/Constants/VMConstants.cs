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
        public static string PathProgramJsonFile(string stdId) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PROGRAMS, $"StudentProgram_{stdId}.json");
        public static string PathPlanJsonFile(int curid) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PLANS, $"{curid}.json");
        public static string PathHtmlCacheFile(int courseId) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_HTML_CACHES, $"{courseId}.html");
        /// <summary>
        /// Đường dẫn tới hình ảnh hồ sơ sinh viên.
        /// </summary>
        /// <param name="studentId">Mã sinh viên</param>
        /// <returns>Đường dẫn</returns>
        public static string PathStudentProfileImg(string studentId) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_IMGS, $"{studentId}.jpg");
    }

    internal static class VMConstants
    {
        #region DB
        public static readonly string DB_CONN = @"Data Source=cs4rsa.db";
        #endregion

        #region Exceptions
        public static readonly string EX_UNKNOWN_ERROR = "Lỗi không xác định";
        public static readonly string EX_INTERNET_ERROR = "Lỗi kết nối mạng";
        public static readonly string EX_SETTING_DOES_NOT_EXIST = "Setting is not exist";

        public static readonly string EX_SCHOOLCLASS_NAME_IS_DIFF = "SchoolClass's Name is difference!";
        public static readonly string EX_INVALID_REGISTER_CODE = "Register code is invalid!";
        public static readonly string EX_CLASSGROUP_MODEL_WAS_NULL = "ClassGroupModel was null!";
        public static readonly string EX_NOT_FOUND_CLASSFORM = "Class Form could not be found!";
        public static readonly string EX_NOT_FOUND_BASE_SCHOOLCLASS_MODEL = "Base school class model cound not be found";
        public static readonly string EX_NOT_FOUND_VIEWMODEL = "ViewModel cound not be found";
        public static readonly string EX_SCHOOLCLASS_STUDYTIME_NOT_ONLY_ONE = "SchoolClass's StudyTimes does not have only one item";
        #endregion

        #region Setting Props
        public static readonly string STPROPS_IS_DATABASE_CREATED = "IsDatabaseCreated";
        public static readonly string STPROPS_VERSION = "Version";
        #endregion

        #region Links
        public static readonly string LK_PROJECT = "https://toky0s.github.io/cs4rsa_core/";
        public static readonly string LK_PROJECT_PAGE = "https://github.com/toky0s/cs4rsa_core";
        public static readonly string LK_PROJECT_GG_SHEET = "https://forms.gle/JHipUM7knjbqKGKWA";
        #endregion

        #region Snackbars
        public static readonly string SNB_NOT_FOUND_SUBJECT_IN_THIS_SEMESTER = "Môn học không tồn tại trong học kỳ này";
        public static readonly string SNB_INVALID_UNSELECT_SUBJECT_NAME = "Tên lớp cần bỏ chọn không hợp lệ";
        public static readonly string SNB_UNSELECT_ALL = "Đã bỏ chọn tất cả";
        public static readonly string SNB_DELETE_ALL = "Đã xoá tất cả";
        public static readonly string SNB_INVALID_SHARESTRING = "ShareString có vấn đề 🤔";
        public static readonly string SNB_ALREADY_DOWNLOADED = "Đã được tải xuống";
        public static readonly string SNB_COPY_SUCCESS = "Đã sao chép";
        public static readonly string SNB_SAVE_TO_STORE = "Đã lưu vào kho";
        public static readonly string SNB_ALL_GEN = "Đã gen hết";

        // NF: Not Found
        public static readonly string SNB_NF_REGISTER_CODE = "Lớp này không có mã đăng ký";
        #endregion

        #region Snackbar Actions
        public static readonly string SNBAC_RESTORE = "HOÀN TÁC";
        public static readonly string SNBAC_GOTO_STORE = "TỚI KHO";
        #endregion

        #region SePaRaTors
        public static readonly char SPRT_TEACHER_SUBJECTS = '$';
        #endregion

        #region Consts
        public const int INT_INVALID_COURSEID = 0;

        public const string STR_SPACE = " ";
        public const string TIME_HH_MM_FORMAT = "HH:mm";
        public const char CHAR_SPACE = ' ';
        #endregion
    }
}
