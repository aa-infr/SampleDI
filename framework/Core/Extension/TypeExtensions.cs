using Infrabel.ICT.Framework.Ioc;
using System;
using System.Linq;

namespace Infrabel.ICT.Framework.Extension
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttributes(false)
                       .Any(a => a is TAttribute);
        }

        public static RegistrationInfo ResolveRegistrationInfo(this Type type)
        {
            var attribute = type.GetCustomAttributes(true)
                .FirstOrDefault(a => a is IoCRegistrationAttribute);

            return (attribute as IoCRegistrationAttribute)?.Information ?? RegistrationInfo.Default;
        }

        public static bool IsBasedOnGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null || !genericType.IsGenericType)
                return false;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            return givenType.BaseType.IsBasedOnGenericType(genericType);
        }
    }
}