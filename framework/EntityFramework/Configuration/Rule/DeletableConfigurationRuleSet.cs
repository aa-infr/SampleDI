using Infrabel.ICT.Framework.Entity;
using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    public class DeletableConfigurationRuleSet : ConfigurationRuleSetBase<IDeletable>
    {
        public override bool IsGeneric => true;

        protected override IEnumerable<IConfigurationRule<IDeletable>> BuildRules()
        {
            return new IConfigurationRule<IDeletable>[]
            {
                ConfigurationRule<IDeletable, DateTime?>.Create(x => x.Deletion)
            };
        }
    }
}