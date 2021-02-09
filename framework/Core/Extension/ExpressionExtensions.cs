using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrabel.ICT.Framework.Extension
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo<TType, TProperty>(this Expression<Func<TType, TProperty>> expression)
        {
            if (!(expression.Body is MemberExpression memberExpression))
                throw new InvalidOperationException($"{expression} is not a property");
            if (memberExpression.Member is PropertyInfo propertyInfo)
                return propertyInfo;

            throw new InvalidOperationException($"{expression} is not a property");
        }

        public static string GetPropertyName<TType, TProperty>(this Expression<Func<TType, TProperty>> expression)
        {
            if (!(expression.Body is MemberExpression memberExpression))
                throw new InvalidOperationException($"{expression} is not a property");
            if (memberExpression.Member is PropertyInfo propertyInfo)
                return propertyInfo.Name;

            throw new InvalidOperationException($"{expression} is not a property");
        }

        public static Type GetPropertyType<TType, TProperty>(this Expression<Func<TType, TProperty>> expression)
        {
            if (!(expression.Body is MemberExpression memberExpression))
                throw new InvalidOperationException($"{expression} is not a property");
            if (memberExpression.Member is PropertyInfo propertyInfo)
                return propertyInfo.PropertyType;

            throw new InvalidOperationException($"{expression} is not a property");
        }
    }
}