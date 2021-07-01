using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Chứa thông tin ngành
    /// </summary>
    public class Curriculum
    {
        private string _curid;
        private string _name;
        public string CurId => _curid;
        public string Name => _name;
        public Curriculum(string curId, string name)
        {
            _curid = curId;
            _name = name;
        }
    }
}
