using DryIoc;
using Infrabel.ICT.Framework.Ioc;
using System;

namespace Infrabel.ICT.Framework.Extended.DryIoc
{
    public class DryIocBootstrapper : IoCBootstrapperBase<IContainer>
    {
        private static readonly Lazy<IContainerInitializer<IContainer>> LazyInitializer =
            new Lazy<IContainerInitializer<IContainer>>(() => new DryIocBootstrapper());

        public static IContainerInitializer<IContainer> GetInstance() => LazyInitializer.Value;

        private DryIocBootstrapper()
        {
        }

        protected override IRegistrationContainer AdaptContainer(IContainer container)
        {
            return DryIocContainerAdapter.Adapt(container);
        }

        protected override IResolutionContainer AdaptResolver(IContainer container)
        {
            return DryIocResolverAdapter.Adapt(container);
        }
    }
}