using cs4rsa_core.Models;
using System.Collections.Generic;

namespace cs4rsa_core.Utils
{
    public class ClassGroupModelCountComparer : IComparer<List<ClassGroupModel>>
    {
        public int Compare(List<ClassGroupModel> x, List<ClassGroupModel> y)
        {
            return x.Count.CompareTo(y.Count);
        }
    }
}
