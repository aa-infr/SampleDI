using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrabel.ICT.Framework.Extended.EntityFramework.Extension
{
    public static class RegistrationContainerExtensions
    {
        public static IRegistrationContainer RegisterDbContext<TDbContext>(this IRegistrationContainer container, Func<string, DbContextOptions<TDbContext>> optionsBuilder) where TDbContext : DbContext
        {
            container.RegisterFactory(resolver =>
            {
                var connectionStringResolver = resolver.Resolve<IConnectionStringResolver>();

                var connectionString = connectionStringResolver.Resolve<TDbContext>();

                return optionsBuilder(connectionString);
            }, RegistrationLifeTime.Scoped);

            container.Register<TDbContext>(RegistrationTarget.Self | RegistrationTarget.AbstractBases, RegistrationLifeTime.Scoped);

            return container;
        }

        public static IRegistrationContainer RegisterDbContextByName<TDbContext>(this IRegistrationContainer container, Func<string, DbContextOptions<TDbContext>> optionsBuilder) where TDbContext : DbContext
        {
            container.RegisterFactory(resolver =>
            {
                var connectionStringResolver = resolver.Resolve<IConnectionStringResolver>();

                var connectionString = connectionStringResolver.Resolve<TDbContext>();

                return optionsBuilder(connectionString);
            }, RegistrationLifeTime.Scoped);

            container.Register<TDbContext>(RegistrationTarget.Self | RegistrationTarget.AbstractBases, RegistrationLifeTime.Scoped, key: typeof(TDbContext).Name);

            return container;
        }
    }
}