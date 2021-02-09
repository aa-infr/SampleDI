using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using Infrabel.ICT.Framework.Ioc;
using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    [IoCRegistration(RegistrationLifeTime.Singleton)]
    internal sealed class DbContextResolver : IDbContextResolver
    {
        private readonly IDictionary<Type, string> _map = new Dictionary<Type, string>();

        public DbContextResolver(IEnumerable<EntityBaseConfiguration> configurations)
        {
            InitializeMap(configurations);
        }

        public void InitializeMap(IEnumerable<EntityBaseConfiguration> configurations)
        {
            var matchingType = typeof(EntityBaseConfiguration<>);

            foreach (var configuration in configurations)
            {
                var baseType = configuration.GetType();

                while (baseType != null && (!baseType.IsConstructedGenericType || baseType.GetGenericTypeDefinition() != matchingType)) baseType = baseType.BaseType;

                if (baseType == null) continue;

                var entityType = baseType.GenericTypeArguments[0];
                var contextName = configuration.GetContextName();

                if (_map.ContainsKey(entityType)) continue;

                _map.Add(entityType, string.IsNullOrWhiteSpace(contextName) ? string.Empty : contextName);
            }
        }

        public string Resolve<T>() where T : class, IEntityBase, new()
        {
            var type = typeof(T);

            if (_map.ContainsKey(type))
                return _map[type];
            else
                throw new ArgumentOutOfRangeException(type.Name);
        }
    }
}