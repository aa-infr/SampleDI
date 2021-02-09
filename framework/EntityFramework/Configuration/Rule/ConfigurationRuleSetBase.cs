using System;
using System.Collections.Generic;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Ioc;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule
{
    [IoCRegistration(RegistrationLifeTime.Singleton)]
    public abstract class ConfigurationRuleSetBase<TInterface> : IConfigurationRuleSet where TInterface : class
    {
        public abstract bool IsGeneric { get; }

        private readonly Type _interfaceType = typeof(TInterface);

        private readonly Lazy<IEnumerable<IConfigurationRule>> _lazyConfigurations;

        protected ConfigurationRuleSetBase()
        { _lazyConfigurations = new Lazy<IEnumerable<IConfigurationRule>>(BuildRules); }

        protected bool CanHandle(Type entityType) { return _interfaceType.IsAssignableFrom(entityType); }

        public IEnumerable<IConfigurationRule> GetRules<TEntity>() where TEntity : class, IEntityBase, new()
        {
            if(!CanHandle(typeof(TEntity)))
                return new IConfigurationRule[0];

            return _lazyConfigurations.Value;
        }

        protected abstract IEnumerable<IConfigurationRule<TInterface>> BuildRules();
    }
}