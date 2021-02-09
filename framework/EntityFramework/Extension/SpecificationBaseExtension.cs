using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using Infrabel.ICT.Framework.Extension;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Extension
{
    public static class SpecificationBaseExtension
    {
        public static string GetShadowColumnName<T, TProperty>(this ISpecification<T> specification, Expression<Func<T, TProperty>> propertyExpression)
            where T : class, IEntityBase, new()
        {
            var member = propertyExpression.Body as MemberExpression;
            var provider = EntityBaseConfiguration<T>.GetProvider();
            var field = EntityTypeBuilderExtensions.GetShadowField(typeof(T), member?.Member as PropertyInfo);

            return DbColumnBuilder.CreateWithNormalizedValue(field?.ColumnName, provider).Build();
        }

        public static string GetShadowCriteria<T, TProperty>(this ISpecification<T> specification, Expression<Func<T, TProperty>> propertyExpression, string value)
            where T : class, IEntityBase, new()
        {
            var member = propertyExpression.Body as MemberExpression;
            var searchType = EntityTypeBuilderExtensions.GetShadowField(typeof(T), member?.Member as PropertyInfo)?.SearchableType;
            return value.ToSearchable((SearchableType)searchType);
        }

        public static string GetShadowColumnName<T>(this ISpecification<T> specification, string value)
            where T : class, IEntityBase, new()
        {
            var provider = EntityBaseConfiguration<T>.GetProvider();
            var field = EntityTypeBuilderExtensions.GetShadowField(typeof(T), value);
            return DbColumnBuilder.CreateWithNormalizedValue(field?.ColumnName, provider).Build();
        }
    }
}