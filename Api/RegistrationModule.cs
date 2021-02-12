using ICT.Template.Api.Resources;
using Infrabel.ICT.Framework.Extended.AspNetCore.Authorization;
using Infrabel.ICT.Framework.Ioc;
using Infrabel.ICT.Framework.Service;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;

namespace ICT.Template.Api
{
    public class RegistrationModule : IoCModuleBase
    {
        public override void Load(IOptionsResolver optionsResolver, IAssemblyTypesResolver assemblyTypesResolver,
            IRegistrationContainer container)
        {
            var types = assemblyTypesResolver.Resolve(Assembly.GetExecutingAssembly());
            container.BulkRegisterByMatchingEndName(types, "Service", RegistrationTarget.Interfaces);
            container.BulkRegisterByMatchingType<IIdentityClaimRefiner>(types, RegistrationTarget.Interfaces);
            //container.Register<IClaimsTransformation, ClaimsRefinementTransformation>(RegistrationLifeTime.Transient);




    }
    }
}