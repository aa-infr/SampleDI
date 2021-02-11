using Infrabel.ICT.Framework.Service;
using System;
using System.Linq;
using System.Reflection;

namespace Infrabel.ICT.Framework.Ioc
{
    public abstract class IoCBootstrapperBase<T> : IContainerInitializer<T>, IInitializedModuleLoader where T : class
    {
        private readonly object _locker = new object();
        private IOptionsResolver _optionsResolver;
        private volatile bool _initialized;
        private IRegistrationContainer _genericContainer;

        private readonly AssemblyTypesResolver _typeResolver;

        protected IoCBootstrapperBase()
        {
            _typeResolver = new AssemblyTypesResolver();
        }

        protected abstract IRegistrationContainer AdaptContainer(T container);

        protected abstract IResolutionContainer AdaptResolver(T container);

        public IModuleLoader Initialize(T scope, IOptionsResolver optionsResolver, IConnectionStringResolver connectionStringResolver)
        {
            if (_initialized)
                return this;

            lock (_locker)
            {
                if (_initialized)
                    return this;

                _genericContainer = AdaptContainer(scope);

                _genericContainer.Register<IResolutionContainer, IResolutionContainer>(AdaptResolver(scope));
                _genericContainer.Register<IConnectionStringResolver, IConnectionStringResolver>(connectionStringResolver);
                _genericContainer.Register<IOptionsResolver, IOptionsResolver>(optionsResolver);
                _genericContainer.Register(typeof(IOptionsResolver<>), typeof(OptionsResolver<>), RegistrationLifeTime.Singleton);
                //_genericContainer.Register(typeof(OptionsResolver<>), RegistrationTarget.Interfaces, RegistrationLifeTime.Singleton);
                _optionsResolver = optionsResolver;
                _initialized = true;
            }

            return this;
        }

        public IInitializedModuleLoader LoadModules(Assembly targetAssembly)
        {
            lock (_locker)
            {
                var modules = targetAssembly.DefinedTypes.Where(t => !t.IsAbstract)
                                                          .Where(t => t.IsSubclassOf(typeof(IoCModuleBase)));

                foreach (var module in modules)
                {
                    var constructor = module.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                        continue;
                    var moduleInstance = (IoCModuleBase)constructor.Invoke(new object[0]);
                    moduleInstance.Initialize(_optionsResolver, _typeResolver, _genericContainer);
                }
            }

            return this;
        }

        public IInitializedModuleLoader LoadModules<TAnchor>() where TAnchor : class
        {
            return LoadModules(typeof(TAnchor).Assembly);
        }

        public IRegistrationContainer BuildContainer()
        {
            _typeResolver.ClearCache();
            return _genericContainer;
        }
    }
}