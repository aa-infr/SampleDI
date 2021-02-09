using ICT.Template.Infrastructure.Data;
using Infrabel.ICT.Framework.Entity;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration;
using Infrabel.ICT.Framework.Extended.EntityFramework.Configuration.Rule;
using Infrabel.ICT.Framework.Extended.EntityFramework.Extension;
using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ICT.Template.Infrastructure
{
    public sealed class RegistrationModule : IoCModuleBase
    {
        public override void Load(IOptionsResolver optionsResolver, IAssemblyTypesResolver assemblyTypesResolver,
            IRegistrationContainer container)
        {
            var types = assemblyTypesResolver.Resolve(Assembly.GetExecutingAssembly());

            container.BulkRegisterByMatchingType(typeof(IRepository<>), types, RegistrationTarget.Interfaces);
            container.BulkRegisterByMatchingEndName(types, "Service", RegistrationTarget.Interfaces);
            container.BulkRegisterByMatchingType(typeof(IConfigurationRuleSet), types, RegistrationTarget.Interfaces);
            container.BulkRegisterByMatchingType<EntityBaseConfiguration>(types, RegistrationTarget.AbstractBases);
            container.RegisterDbContext(connectionString => new DbContextOptionsBuilder<SampleDbContext>().UseInMemoryDatabase(connectionString)
                .Options);
        }
    }
}