using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Ioc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    [IoCRegistration(RegistrationLifeTime.Singleton)]
    internal class ConfigurationRulesService<T> : IConfigurationRulesService<T> where T : class, IEntityBase, new()
    {
        private readonly Lazy<IEnumerable<IConfigurationRuleSet>> _rulesFactory;

        public ConfigurationRulesService(Lazy<IEnumerable<IConfigurationRuleSet>> rulesFactory)
        {
            _rulesFactory = rulesFactory;
        }

        public void Process(EntityTypeBuilder<T> builder, DataBaseProvider provider, Func<IConfigurationRuleSet, bool> filter = null)
        {
            var mappingRuleSets = filter == null ? _rulesFactory.Value : _rulesFactory.Value.Where(filter);
            foreach (var mappingRuleSet in mappingRuleSets)
            {
                foreach (var rule in mappingRuleSet.GetRules<T>())
                {
                    var propertyBuilder = builder.Property(rule.PropertyType, rule.PropertyName);
                    if (rule.IsConcurrencyToken)
                        propertyBuilder = propertyBuilder.IsConcurrencyToken();
                    if (rule.IsRequired)
                        propertyBuilder = propertyBuilder.IsRequired();
                    if (rule.MaxLength.HasValue)
                        propertyBuilder = propertyBuilder.HasMaxLength(rule.MaxLength.Value);

                    var columnName = rule.HasColumnName
                        ? DbColumnBuilder.CreateWithPristineValue(rule.ColumnName, provider).Build()
                        : DbColumnBuilder.CreateWithNormalizedValue(rule.PropertyName, provider).Build();

                    propertyBuilder.HasColumnName(columnName);
                }
            }
        }
    }
}