using Infrabel.ICT.Framework.Entity;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    public class OrderableConfigurationRuleSet : ConfigurationRuleSetBase<IOrderable>
    {
        public override bool IsGeneric => true;

        protected override IEnumerable<IConfigurationRule<IOrderable>> BuildRules()
        {
            return new IConfigurationRule<IOrderable>[]
            {
                ConfigurationRule<IOrderable, bool>.Create(x => x.IsDefault),
                ConfigurationRule<IOrderable, int>.Create(x => x.OrderNumber)
            };
        }
    }
}