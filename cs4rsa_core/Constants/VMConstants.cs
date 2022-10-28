namespace cs4rsa_core.Constants
{
    internal abstract class VMConstants
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
        public static readonly string SNB_AT_LAST_SCHEDULE = "Đã đến bộ lịch cuối";
        public static readonly string SNB_ALREADY_DOWNLOADED = "Đã được tải xuống";
        #endregion

        #region Snackbar Actions
        public static readonly string SNBAC_RESTORE = "HOÀN TÁC";
        #endregion
    }
}
