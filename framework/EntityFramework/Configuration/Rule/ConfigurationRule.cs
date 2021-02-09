using System;
using System.Linq.Expressions;
using Infrabel.ICT.Framework.Extension;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    public class ConfigurationRule<TEntity, TProperty> : IConfigurationRule<TEntity> where TEntity : class
    {
        private ConfigurationRule(Expression<Func<TEntity, TProperty>> propertySelector, string columnName, bool isRequired, bool isConcurrencyToken, int? maxLength)
        {
            if(propertySelector == null)
                throw new ArgumentNullException(nameof(propertySelector));
            var propInfo = propertySelector.GetPropertyInfo();
            ColumnName = columnName;
            HasColumnName = !string.IsNullOrWhiteSpace(columnName);
            IsConcurrencyToken = isConcurrencyToken;
            MaxLength = maxLength;
            PropertyType = propInfo.PropertyType;
            PropertyName = propInfo.Name;
            IsRequired = isRequired;
        }

        public int? MaxLength { get; }

        public bool IsConcurrencyToken { get; }

        public bool IsRequired { get; }

        public Type PropertyType { get; }

        public string PropertyName { get; }

        public string ColumnName { get; }

        public bool HasColumnName { get; }

        public static ConfigurationRule<TEntity, TProperty> Create(Expression<Func<TEntity, TProperty>> propertySelector, string columnName = null, bool isRequired = false, bool isConcurrencyToken = false, int? maxLength = null)
        {
            return new ConfigurationRule<TEntity, TProperty>(propertySelector, columnName, isRequired, isConcurrencyToken, maxLength);
        }
    }
}