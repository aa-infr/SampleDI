using SimpleInjector;
using Infrabel.ICT.Framework.Ioc;

namespace Infrabel.ICT.Framework.Extended.SimpleInjectorIoc
{
  public static class RegistrationLifeTimeExtensions
  {
    public static Lifestyle ToContainerLifeTime(this RegistrationLifeTime lifeTime)
    {
      switch (lifeTime)
      {
        case RegistrationLifeTime.Singleton:
          return Lifestyle.Singleton;

        case RegistrationLifeTime.Transient:
          return Lifestyle.Transient;

        case RegistrationLifeTime.ScopedOrSingleton:
          return Lifestyle.Scoped;

        default:
          return Lifestyle.Scoped;
      }
    }
  }
}