using Infrabel.ICT.Framework.Entity;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    public interface IConfigurationRuleSet
    {
        bool IsGeneric { get; }

        IEnumerable<IConfigurationRule> GetRules<TEntity>() where TEntity : class, IEntityBase, new();
    }
}