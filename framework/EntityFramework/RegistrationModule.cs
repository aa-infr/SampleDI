using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule;
using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;

namespace Infrabel.ICT.Framework.Extended.EntityFramework
{
    public class RegistrationModule : IoCModuleBase
    {
        public override void Load(IOptionsResolver optionsResolver, IAssemblyTypesResolver assemblyTypesResolver,
            IRegistrationContainer container)
        {
            var types = assemblyTypesResolver.Resolve();
            container.BulkRegisterByMatchingEndName(types, "Service", RegistrationTarget.Interfaces);
            container.BulkRegisterByMatchingType(typeof(IConfigurationRuleSet), types, RegistrationTarget.Interfaces);
            container.Register<IConfigurationsFactory, ConfigurationsFactory>(RegistrationLifeTime.Singleton);
            container.Register<IDbContextResolver, DbContextResolver>(RegistrationLifeTime.Scoped);
            container.Register<IDbContextFactory, DbContextFactory>(RegistrationLifeTime.Scoped);
            container.Register(typeof(IRepository<>), typeof(DbRepository<>), RegistrationLifeTime.Scoped);
        }
    }
}