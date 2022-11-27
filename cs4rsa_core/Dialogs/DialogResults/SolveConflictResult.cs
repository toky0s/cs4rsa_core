namespace Cs4rsa.Dialogs.DialogResults
{
    public class SolveConflictResult
    {
        private string _classGroupModelName;
        public string ClassGroupModel
        {
            get { return _classGroupModelName; }
            set { _classGroupModelName = value; }
        }

        /// <summary>
        /// Loại bỏ ClassGroupModel để solve conflict.
        /// </summary>
        /// <param name="classGroupModelName">Tên của ClassGroupModel cần loại bỏ.</param>
        public SolveConflictResult(string classGroupModelName)
        {
            _classGroupModelName = classGroupModelName;
        }
    }
}
