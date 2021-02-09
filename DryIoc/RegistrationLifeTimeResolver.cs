using DryIoc;
using Infrabel.ICT.Framework.Ioc;

namespace Infrabel.ICT.Framework.Extended.DryIoc
{
    public static class RegistrationLifeTimeExtensions
    {
        public static IReuse ToContainerLifeTime(this RegistrationLifeTime lifeTime)
        {
            switch (lifeTime)
            {
                case RegistrationLifeTime.Singleton:
                    return Reuse.Singleton;

                case RegistrationLifeTime.Transient:
                    return Reuse.Transient;

                case RegistrationLifeTime.ScopedOrSingleton:
                    return Reuse.ScopedOrSingleton;

                default:
                    return Reuse.Scoped;
            }
        }
    }
}