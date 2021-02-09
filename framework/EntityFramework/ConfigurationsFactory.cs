using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using Infrabel.ICT.Framework.Extension;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    internal class ConfigurationsFactory : IConfigurationsFactory
    {
        private readonly Func<IEnumerable<EntityBaseConfiguration>> _configurationsFactory;
        private ILookup<string, EntityBaseConfiguration> _lookup;
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        public ConfigurationsFactory(Func<IEnumerable<EntityBaseConfiguration>> configurationsFactory)
        {
            _configurationsFactory = configurationsFactory;
        }

        public IEnumerable<dynamic> GetConfigurationTypes(string contextName, bool cacheConfigurations = false)
        {
            var configurations = GetConfigurationDictionary();

            if(configurations.Contains(contextName))
                foreach(var configuration in configurations[contextName])
                    yield return configuration;
            else if(configurations.Contains(string.Empty))
                foreach(var configuration in configurations[string.Empty])
                    yield return configuration;
        }

        public ILookup<string, EntityBaseConfiguration> GetConfigurationDictionary(bool cacheConfigurations = false)
        {
            if(!cacheConfigurations)
                return _configurationsFactory()
                       .ToLookup(c => c.GetContextName(), c => c);

            var result = _locker.ReadExecute(() => _lookup);

            if(result != null)
                return result;

            return _locker.UpgradableReadExecute(() =>
                                                 {
                                                     if(_lookup != null)
                                                         return _lookup;

                                                     return _locker.UpgradableWriteExecute(() =>
                                                                                           {
                                                                                               _lookup = _lookup ?? _configurationsFactory()
                                                                                                                    .ToLookup(c => c.GetContextName(), c => c);
                                                                                               return _lookup;
                                                                                           });
                                                 });
        }
    }
}