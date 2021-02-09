using System;
using System.Collections.Generic;
using Infrabel.ICT.Framework.Entity;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    internal class AuditableConfigurationRuleSet : ConfigurationRuleSetBase<IAuditableEntity>
    {
        public override bool IsGeneric => true;

        protected override IEnumerable<IConfigurationRule<IAuditableEntity>> BuildRules()
        {
            return new IConfigurationRule<IAuditableEntity>[]
            {
                ConfigurationRule<IAuditableEntity, string>.Create(x => x.CreatedBy),
                ConfigurationRule<IAuditableEntity, DateTime>.Create(x => x.CreationDate),
                ConfigurationRule<IAuditableEntity, DateTime>.Create(x => x.UpdateDate, isConcurrencyToken: true),
                ConfigurationRule<IAuditableEntity, string>.Create(x => x.UpdatedBy)
            };
        }
    }
}