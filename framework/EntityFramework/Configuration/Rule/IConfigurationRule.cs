using System;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    public interface IConfigurationRule
    {
        int? MaxLength { get; }
        bool IsConcurrencyToken { get; }
        bool IsRequired { get; }
        Type PropertyType { get; }
        string PropertyName { get; }
        string ColumnName { get; }
        bool HasColumnName { get; }
    }

    public interface IConfigurationRule<TEntity> : IConfigurationRule where TEntity : class
    {
    }
}