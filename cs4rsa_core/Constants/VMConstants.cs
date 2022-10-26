namespace cs4rsa_core.Constants
{
    internal abstract class VMConstants
    {
        #region Exceptions
        public static readonly string EX_UNKNOWN_ERROR = "Lỗi không xác định";
        public static readonly string EX_INTERNET_ERROR = "Lỗi kết nối mạng";
        public static readonly string EX_SETTING_DOES_NOT_EXIST = "Setting is not exist";

        public static readonly string EX_SCHOOLCLASS_NAME_IS_DIFF = "SchoolClass's Name is difference!";
        public static readonly string EX_INVALID_REGISTER_CODE = "Register code is invalid!";
        public static readonly string EX_CLASSGROUP_MODEL_WAS_NULL = "ClassGroupModel was null!";
        public static readonly string EX_NOT_FOUND_CLASSFORM = "Class Form could not be found!";
        public static readonly string EX_NOT_FOUND_BASE_SCHOOLCLASS_MODEL = "Base school class model cound not be found";
        #endregion

        #region Setting Props
        public static readonly string IS_DATABASE_CREATED = "IsDatabaseCreated";
        #endregion
    }
}
