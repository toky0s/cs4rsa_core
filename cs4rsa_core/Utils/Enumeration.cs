using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cs4rsa.Utils
{
    public abstract class Enumeration : IComparable
    {
        public readonly string Name;

        public readonly int Id;

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
            {
                return false;
            }

            bool typeMatches = GetType().Equals(obj.GetType());
            bool valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other)
        {
            return Id.CompareTo(((Enumeration)other).Id);
        }
    }
}
