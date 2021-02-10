using Infrabel.ICT.Framework.Extended.AspNetCore.Context;
using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Infrabel.ICT.Framework.Extended.AspNetCore
{
    public sealed class RegistrationModule : IoCModuleBase
    {
        public override void Load(IOptionsResolver optionsResolver, IAssemblyTypesResolver assemblyTypesResolver,
            IRegistrationContainer container)
        {
            var types = assemblyTypesResolver.Resolve(Assembly.GetExecutingAssembly());

            container.BulkRegisterByMatchingEndName(types, "Service", RegistrationTarget.Interfaces);
            container.Register<IHttpContextAccessor, HttpContextAccessor>(RegistrationLifeTime.Singleton);
            container.RegisterFactory(c => (IUserContext)new UserContext(() => c.Resolve<IHttpContextAccessor>().HttpContext, c.Resolve<IRolePoliciesService>(), c.Resolve<IDateService>())
                                    , RegistrationLifeTime.Singleton);
        }
    }
}