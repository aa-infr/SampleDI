using Infrabel.ICT.Framework.Entity;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    public class EnablableConfigurationRuleSet : ConfigurationRuleSetBase<IEnablable>
    {
        public override bool IsGeneric => true;

        protected override IEnumerable<IConfigurationRule<IEnablable>> BuildRules()
        {
            return new IConfigurationRule<IEnablable>[]
            {
                ConfigurationRule<IEnablable, bool>.Create(x => x.IsEnabled)
            };
        }
    }
}