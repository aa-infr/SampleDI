using System;
using System.Collections.Generic;
using System.Linq;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Ioc;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    [IoCRegistration(RegistrationLifeTime.Scoped)]
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IList<KeyValuePair<string, DbContextBase>> _namedResolvers;
        private readonly DbContextBase _singleResolver;
        private readonly IDbContextResolver _resolver;

        public DbContextFactory(IList<KeyValuePair<string, DbContextBase>> namedResolvers,DbContextBase singleResolver, IDbContextResolver resolver)
        {
            _namedResolvers = namedResolvers;
            _singleResolver = singleResolver;
            _resolver = resolver;
        }

        public DbContextBase Create<T>() where T : class, IEntityBase, new()
        {
            var contextName = _resolver.Resolve<T>();

            if(string.IsNullOrWhiteSpace(contextName))
                return _singleResolver;

            var resolver = _namedResolvers.FirstOrDefault(c => string.Equals(c.Key, contextName, StringComparison.OrdinalIgnoreCase));

            if(resolver.Equals(default(KeyValuePair<string, Func<DbContextBase>>)))
                throw new InvalidOperationException($"Unable to resolve the context name {contextName}");

            return resolver.Value;
        }
    }
}