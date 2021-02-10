using SimpleInjector;
using Infrabel.ICT.Framework.Ioc;
using System;

namespace Infrabel.ICT.Framework.Extended.SimpleInjectorIoc
{
  public class SimpleInjectorIocBootstrapper : IoCBootstrapperBase<Container>
  {
    private static readonly Lazy<IContainerInitializer<Container>> LazyInitializer =
        new Lazy<IContainerInitializer<Container>>(() => new SimpleInjectorIocBootstrapper());

    public static IContainerInitializer<Container> GetInstance() => LazyInitializer.Value;

    private SimpleInjectorIocBootstrapper()
    {
    }

    protected override IRegistrationContainer AdaptContainer(Container container)
    {
      return SimpleInjectorIocContainerAdapter.Adapt(container);
    }

    protected override IResolutionContainer AdaptResolver(Container container)
    {
      return SimpleInjectorIocResolverAdapter.Adapt(container);
    }
  }
}