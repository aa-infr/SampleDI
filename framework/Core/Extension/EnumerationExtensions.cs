using System;
using System.ComponentModel;
using System.Linq;

namespace Infrabel.ICT.Framework.Extension
{
    public static class EnumerationExtensions
    {
        public static string GetDescription(this Enum enumeration)
        {
            var attribute = GetText<DescriptionAttribute>(enumeration);
            return attribute.Description;
        }

        private static T GetText<T>(Enum enumeration) where T : Attribute
        {
            var type = enumeration.GetType();

            var memberInfo = type.GetMember(enumeration.ToString());

            if (!memberInfo.Any())
                throw new ArgumentException($"No public members for the argument '{enumeration}'.");

            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);

            if (attributes == null || attributes.Length != 1)
                throw new ArgumentException(
                    $"Can't find an attribute matching '{typeof(T).Name}' for the argument '{enumeration}'");

            return attributes.Single() as T;
        }

        public static bool IsMemberOf<TEnum>(this object value) where TEnum : Enum

        {
            return Enum.IsDefined(typeof(TEnum), value);
        }
    }
}