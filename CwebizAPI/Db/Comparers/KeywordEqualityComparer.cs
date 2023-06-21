/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.Diagnostics.CodeAnalysis;
using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Comparers
{
    /// <summary>
    /// Keyword Equality Comparer.
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public class KeywordEqualityComparer : IEqualityComparer<Keyword>
    {
        public bool Equals(Keyword? x, Keyword? y)
        {
            if (x == null || y == null) return false;
            return x.CourseId == y.CourseId;
        }

        public int GetHashCode([DisallowNull] Keyword obj)
        {
            return obj.CourseId.GetHashCode();
        }
    }
}
