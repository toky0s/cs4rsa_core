/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Comparers
{
    public class DisciplineEqualityComparer : IEqualityComparer<Discipline>
    {
        public bool Equals(Discipline? x, Discipline? y)
        {
            if (x is null || y is null) return false;
            return x.Name!.Equals(y.Name);
        }

        public int GetHashCode(Discipline obj)
        {
            return obj.Name!.GetHashCode();
        }
    }
}
