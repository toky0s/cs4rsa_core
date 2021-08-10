using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Models;

namespace cs4rsa.Dialogs.DialogResults
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
        /// 
        /// </summary>
        /// <param name="classGroupModelName">Tên của ClassGroupModel cần loại bỏ.</param>
        public SolveConflictResult(string classGroupModelName)
        {
            _classGroupModelName = classGroupModelName;
        }
    }
}
