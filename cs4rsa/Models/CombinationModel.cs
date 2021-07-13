using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    /// <summary>
    /// Class này đại diện cho sự kết hợp một tập các ClassGroupModel khác nhau.
    /// </summary>
    public class CombinationModel
    {
        private List<SubjectModel> _subjectModels;
        public List<SubjectModel> SubjecModels
        {
            get
            {
                return _subjectModels;
            }
            set
            {
                _subjectModels = value;
            }
        }
        private List<ClassGroupModel> _classGroupModels;
        public List<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return _classGroupModels;
            }
            set
            {
                _classGroupModels = value;
            }
        }
        public CombinationModel(List<SubjectModel> subjectModels, List<ClassGroupModel> classGroupModels)
        {
            _subjectModels = subjectModels;
            _classGroupModels = classGroupModels;
        }


        /// <summary>
        /// Kiểm tra xem Bộ này có hợp lệ hay không. Một Bộ hợp lệ là khi từng ClassGroupModel
        /// bên trong thuộc một Subject, mà mỗi Subject là duy nhất.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            int count = 0;
            string subjecCode = "";
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (!classGroupModel.SubjectCode.Equals(subjecCode))
                {
                    subjecCode = classGroupModel.SubjectCode;
                    count++;
                }
            }
            if (count == _classGroupModels.Count)
                return true;
            return false;
        }

        /// <summary>
        /// Kiểm tra xem bộ này có chứa xung đột về thời gian hay không.
        /// </summary>
        /// <returns></returns>
        public bool IsHaveTimeConflicts()
        {
            for (int i = 0; i < _classGroupModels.Count; ++i)
            {
                for (int k = i + 1; k < _classGroupModels.Count; ++k)
                {
                    Conflict conflict = new Conflict(_classGroupModels[i], _classGroupModels[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                        return true;
                }
            }
            return false;
        }

        public bool IsHavePlaceConflicts()
        {
            for (int i = 0; i < _classGroupModels.Count; ++i)
            {
                for (int k = i + 1; k < _classGroupModels.Count; ++k)
                {
                    PlaceConflictFinder conflict = new PlaceConflictFinder(_classGroupModels[i], _classGroupModels[k]);
                    ConflictPlace conflictPlace = conflict.GetPlaceConflict();
                    if (conflictPlace != null)
                        return true;
                }
            }
            return false;
        }
    }
}
