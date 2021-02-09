using Infrabel.ICT.Framework.Service;
using System;

namespace Infrabel.ICT.Framework.Ioc
{
    public abstract class IoCModuleBase
    {
        protected IoCModuleBase()
        {
        }

        public void Initialize(IOptionsResolver optionsResolver, IAssemblyTypesResolver assemblyTypesResolver, IRegistrationContainer container)
        {
            Load(optionsResolver ?? throw new ArgumentNullException(nameof(optionsResolver)), assemblyTypesResolver ?? throw new ArgumentNullException(nameof(assemblyTypesResolver)), container ?? throw new ArgumentNullException(nameof(container)));
        }

        public abstract void Load(IOptionsResolver optionsResolver, IAssemblyTypesResolver assemblyTypesResolver, IRegistrationContainer container);
    }
}