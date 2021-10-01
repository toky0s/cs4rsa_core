using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperService
{
    public class Checker
    {
        /// <summary>
        /// Kiểm tra một List<T> có 
        /// </summary>
        /// <typeparam name="T">Là một trong các BasicData</typeparam>
        /// <param name="thisSet"></param>
        /// <param name="thatSet"></param>
        /// <returns></returns>
        public static bool ThisSetInThatSet<T>(List<T> thisSet, List<T> thatSet)
        {
            List<T> outSet = thisSet.Intersect(thatSet).ToList();
            if (outSet.Count >= thatSet.Count)
            {
                return true;
            }
            return false;
        }
    }
}
