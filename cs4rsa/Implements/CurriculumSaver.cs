using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Implements
{
    class CurriculumSaver : ISaver<Curriculum>
    {
        /// <summary>
        /// Đã bao gồm cập nhật.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameters"></param>
        public void Save(Curriculum obj, object[] parameters = null)
        {
            if (!Cs4rsaDataView.IsExistsCurriculum(obj.CurId))
                Cs4rsaDataEdit.AddCurriculum(obj);
        }
    }
}
