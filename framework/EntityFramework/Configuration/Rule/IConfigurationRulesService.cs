using Infrabel.ICT.Framework.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    public interface IConfigurationRulesService<T> where T : class, IEntityBase, new()
    {
        void Process(EntityTypeBuilder<T> builder, DataBaseProvider provider, Func<IConfigurationRuleSet, bool> filter = null);
    }
}