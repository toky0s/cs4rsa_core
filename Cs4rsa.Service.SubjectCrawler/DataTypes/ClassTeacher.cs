using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace Cs4rsa.Service.SubjectCrawler.DataTypes
{
    public class ClassTeacher
    {
        public string Name { get; set; }
        public string IntructorId { get; set; }

        private static int _visitorIntructorIndex = 0;
        public static ClassTeacher Create()
        {
            return new ClassTeacher()
            {
                Name = "TẤT CẢ",
                IntructorId = "0"
            };
        }

        public static ClassTeacher Create(string name)
        {
            return new ClassTeacher()
            {
                Name = name,
                IntructorId = (_visitorIntructorIndex++).ToString()
            };
        }

        public static ClassTeacher Create(string name, string instructorId)
        {
            return new ClassTeacher()
            {
                Name = name,
                IntructorId = instructorId
            };
        }
    }
}
