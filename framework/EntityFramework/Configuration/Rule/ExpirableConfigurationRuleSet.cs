using System;
using System.Collections.Generic;
using Infrabel.ICT.Framework.Entity;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    internal class ExpirableConfigurationRuleSet : ConfigurationRuleSetBase<IExpirable>
    {
        public override bool IsGeneric => true;

        protected override IEnumerable<IConfigurationRule<IExpirable>> BuildRules()
        {
            return new IConfigurationRule<IExpirable>[]
            {
                ConfigurationRule<IExpirable, DateTime>.Create(x => x.ValidFrom),
                ConfigurationRule<IExpirable, DateTime?>.Create(x => x.ValidTo)
            };
        }
    }
}