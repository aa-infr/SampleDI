using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Ioc;
using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.DryIoc
{
    public static class TargetResolver
    {
        private static readonly Dictionary<RegistrationTarget, Func<Type, bool>> TargetMap = new Dictionary<RegistrationTarget, Func<Type, bool>>()
        {
            { RegistrationTarget.AbstractBases | RegistrationTarget.Interfaces | RegistrationTarget.Self, t => TargetMap[RegistrationTarget.Interfaces](t) || TargetMap[RegistrationTarget.AbstractBases](t) || TargetMap[RegistrationTarget.Self](t) },
            { RegistrationTarget.AbstractBases | RegistrationTarget.Interfaces, t => TargetMap[RegistrationTarget.Interfaces](t) || TargetMap[RegistrationTarget.AbstractBases](t) },
            { RegistrationTarget.AbstractBases | RegistrationTarget.Self, t => TargetMap[RegistrationTarget.Self](t) || TargetMap[RegistrationTarget.AbstractBases](t) },
            { RegistrationTarget.Interfaces | RegistrationTarget.Self, t => TargetMap[RegistrationTarget.Self](t) || TargetMap[RegistrationTarget.Interfaces](t) },
            { RegistrationTarget.Interfaces, t => CanHandle(t) && t.IsInterface && t.IsPublic },
            { RegistrationTarget.AbstractBases, t => CanHandle(t) && t.IsAbstract && !t.IsInterface && t.IsPublic},
            { RegistrationTarget.Self, t => CanHandle(t) && !t.IsAbstract && t.IsPublic }
        };

        public static Func<Type, bool> Resolve(RegistrationTarget target)
        {
            return TargetMap.ContainsKey(target) ? TargetMap[target] : TargetMap[RegistrationTarget.Interfaces];
        }

        public static bool CanHandle(Type type)
        {
            if (type == null)
                return false;

            return !type.HasAttribute<IoCIgnoreTargetAttribute>();
        }
    }
}