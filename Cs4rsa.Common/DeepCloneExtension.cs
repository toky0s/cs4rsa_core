using System;
using System.Reflection;

namespace Cs4rsa.Common
{
    public static class DeepCloneExtension
    {
        private static readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static T DeepClone<T>(this T obj) where T : new()
        {
            var type = obj.GetType();
            var result = (T)Activator.CreateInstance(type);

            do
            {
                foreach (var field in type.GetFields(bindingFlags))
                    field.SetValue(result, field.GetValue(obj));
            }
            while ((type = type.BaseType) != typeof(object));

            return result;
        }
    }
}
