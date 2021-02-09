using System;
using System.Reflection;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Extension
{
    internal static class PropertyInfoExtension
    {
        public static bool Exist(this PropertyInfo[] properties, string name, StringComparison stringComparison= StringComparison.Ordinal)
        {
            foreach (var property in properties) if (property.Name.Equals(name, stringComparison)) return true;
            return false;
        }
    }
}
