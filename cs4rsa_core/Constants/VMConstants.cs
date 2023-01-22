using Cs4rsa.Utils.Interfaces;

using System;
using System.IO;

namespace Cs4rsa.Constants
{
    internal static class CredizText
    {
        /// <summary>
        /// Thông báo file của sinh viên không tồn tại.
        /// </summary>
        /// <param name="fName">File name</param>
        /// <param name="stdName">Student name</param>
        public static string AutoMsg001(string fName, string stdName) => $"File {fName} của sinh viên {stdName} không tồn tại.";
        public static string PathProgramJsonFile(string stdId) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PROGRAMS, $"StudentProgram_{stdId}.json");
        public static string PathPlanJsonFile(int curid) => Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PLANS, $"{curid}.json");
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
        public static readonly string SNB_CAL_DONE = "Đã tính toán xong";
        public static readonly string SNB_ALREADY_DOWNLOADED = "Đã được tải xuống";
        public static readonly string SNB_COPY_SUCCESS = "Đã sao chép";

        // NF: Not Found
        public static readonly string SNB_NF_REGISTER_CODE = "Lớp này không có mã đăng ký";
        #endregion

        #region Snackbar Actions
        public static readonly string SNBAC_RESTORE = "HOÀN TÁC";
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
