using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;
using System.Reflection;

namespace Infrabel.ICT.Framework
{
    public sealed class RegistrationModule : IoCModuleBase
    {
        public override void Load(IOptionsResolver optionsResolver, IAssemblyTypesResolver assemblyTypesResolver, IRegistrationContainer container)
        {
            var types = assemblyTypesResolver.Resolve(Assembly.GetExecutingAssembly());
            container.BulkRegisterByMatchingEndName(types, "Service", RegistrationTarget.Interfaces);
        }
    }
}