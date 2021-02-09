using System;
using System.Linq;
using System.Reflection;

namespace Infrabel.ICT.Framework.Ioc
{
    public interface IAssemblyTypesResolver
    {
        ILookup<RegistrationInfo, Type> Resolve();

        ILookup<RegistrationInfo, Type> Resolve<TAnchor>() where TAnchor : class;

        ILookup<RegistrationInfo, Type> Resolve(Type anchorType);

        ILookup<RegistrationInfo, Type> Resolve(Assembly targetAssembly);

        void ClearCache();
    }
}