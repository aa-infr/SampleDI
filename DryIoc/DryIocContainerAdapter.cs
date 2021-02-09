using DryIoc;
using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Ioc;
using System;
using System.Linq;

namespace Infrabel.ICT.Framework.Extended.DryIoc
{
    public class DryIocContainerAdapter : IRegistrationContainer
    {
        private readonly IContainer _container;

        private DryIocContainerAdapter(IContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        private static IfAlreadyRegistered Map(bool shouldReplace)
        {
            return shouldReplace ? IfAlreadyRegistered.Replace : IfAlreadyRegistered.AppendNotKeyed;
        }

        public IRegistrationContainer Register<TConcrete>(RegistrationTarget target, RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false)
        {
            return Register(typeof(TConcrete), target, registrationLifeTime, key, shouldReplace);
        }

        public IRegistrationContainer BulkRegisterByMatchingType<TMatchingType>(ILookup<RegistrationInfo, Type> lookup, RegistrationTarget target)
        {
            return BulkRegisterByMatchingType(typeof(TMatchingType), lookup, target);
        }

        public IRegistrationContainer BulkRegisterByMatchingType(Type matchingType, ILookup<RegistrationInfo, Type> lookup, RegistrationTarget target)
        {
            IterateAndRegister(lookup, t => t.IsBasedOnGenericType(matchingType) || matchingType.IsAssignableFrom(t), target);
            return this;
        }

        public IRegistrationContainer BulkRegisterByMatchingNamespace(ILookup<RegistrationInfo, Type> lookup, string matchingNamespace,
            RegistrationTarget target)
        {
            IterateAndRegister(lookup, t => string.Equals(t.Namespace, matchingNamespace, StringComparison.OrdinalIgnoreCase), target);
            return this;
        }

        public IRegistrationContainer BulkRegisterByMatchingNamespaceWithChildren(ILookup<RegistrationInfo, Type> lookup, string matchingNamespace,
            RegistrationTarget target)
        {
            IterateAndRegister(lookup, t => t.Namespace?.StartsWith(matchingNamespace, StringComparison.OrdinalIgnoreCase) ?? false, target);
            return this;
        }

        public IRegistrationContainer BulkRegisterByMatchingEndName(ILookup<RegistrationInfo, Type> lookup, string matchingEndName,
            RegistrationTarget target)
        {
            if (string.IsNullOrWhiteSpace(matchingEndName))
                return this;

            var genericEndName = $"{matchingEndName}`1";

            IterateAndRegister(lookup, t => t.Name.EndsWith(matchingEndName, StringComparison.OrdinalIgnoreCase) || t.Name.EndsWith(genericEndName, StringComparison.OrdinalIgnoreCase), target);
            return this;
        }

        public IRegistrationContainer BulkRegisterByPredicate(ILookup<RegistrationInfo, Type> lookup, Func<Type, bool> predicate, RegistrationTarget target)
        {
            IterateAndRegister(lookup, predicate, target);
            return this;
        }

        public IRegistrationContainer RegisterDecorator<TAbstract, TConcrete>(TConcrete instance, string key = null, bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract
        {
            var serviceKey = ResolveKey(key);

            if (string.IsNullOrEmpty(serviceKey))
                _container.RegisterInstance<TAbstract>(instance, setup: Setup.Decorator, ifAlreadyRegistered: Map(shouldReplace));
            else
                _container.RegisterInstance<TAbstract>(instance, setup: Setup.DecoratorWith(r => string.Equals(key, r.ServiceKey as string, StringComparison.OrdinalIgnoreCase)));
            return this;
        }

        public IRegistrationContainer RegisterDecorator<TAbstract, TConcrete>(RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract
        {
            var serviceKey = ResolveKey(key);

            if (string.IsNullOrEmpty(serviceKey))
                _container.Register<TAbstract, TConcrete>(registrationLifeTime.ToContainerLifeTime(),
                    setup: Setup.Decorator, ifAlreadyRegistered: Map(shouldReplace));
            else
                _container.Register<TAbstract, TConcrete>(registrationLifeTime.ToContainerLifeTime(),
                    setup: Setup.DecoratorWith(r =>
                        string.Equals(key, r.ServiceKey as string, StringComparison.OrdinalIgnoreCase)));
            return this;
        }

        public IRegistrationContainer RegisterDecorator(Type abstractType, Type decorator,
            RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false)
        {
            var serviceKey = ResolveKey(key);

            if (string.IsNullOrEmpty(serviceKey))
                _container.Register(abstractType, decorator, registrationLifeTime.ToContainerLifeTime(), setup: Setup.Decorator, ifAlreadyRegistered: Map(shouldReplace));
            else
                _container.Register(abstractType, decorator, registrationLifeTime.ToContainerLifeTime(), setup: Setup.DecoratorWith(r =>
                    string.Equals(key, r.ServiceKey as string, StringComparison.OrdinalIgnoreCase)));
            return this;
        }

        public IRegistrationContainer Register<TAbstract, TConcrete>(TConcrete instance, Action<TAbstract> cleanupAction = null,
            string key = null, bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract
        {
            _container.RegisterInstance<TAbstract>(instance, serviceKey: ResolveKey(key), ifAlreadyRegistered: Map(shouldReplace));

            if (cleanupAction != null)
                _container.RegisterDisposer(cleanupAction);

            return this;
        }

        public IRegistrationContainer Register<TAbstract, TConcrete>(
            RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient,
            Action<TAbstract> cleanupAction = null,
            string key = null, bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract
        {
            _container.Register<TAbstract, TConcrete>(registrationLifeTime.ToContainerLifeTime(), serviceKey: ResolveKey(key), ifAlreadyRegistered: Map(shouldReplace));

            if (cleanupAction != null)
                _container.RegisterDisposer(cleanupAction);

            return this;
        }

        public IRegistrationContainer RegisterFactory<TAbstract>(Func<IResolutionContainer, TAbstract> factoryFunc,
            RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false) where TAbstract : class

        {
            _container.RegisterDelegate(_ => factoryFunc(DryIocResolverAdapter.Adapt(_container)), registrationLifeTime.ToContainerLifeTime(), serviceKey: ResolveKey(key), ifAlreadyRegistered: Map(shouldReplace));
            return this;
        }

        public IRegistrationContainer Register(Type abstractType, Type concreteType,
            RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false)
        {
            _container.Register(abstractType, concreteType, registrationLifeTime.ToContainerLifeTime(), serviceKey: ResolveKey(key), ifAlreadyRegistered: Map(shouldReplace));

            return this;
        }

        public IRegistrationContainer Register(Type concreteType, RegistrationTarget target,
            RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false)
        {
            var serviceTypeCondition = TargetResolver.Resolve(target);
            _container.RegisterMany(new[] { concreteType }, registrationLifeTime.ToContainerLifeTime(), serviceTypeCondition: serviceTypeCondition, serviceKey: ResolveKey(key), ifAlreadyRegistered: Map(shouldReplace));
            return this;
        }

        private void IterateAndRegister(ILookup<RegistrationInfo, Type> lookup, Func<Type, bool> typePredicate,
            RegistrationTarget target)
        {
            if (lookup == null || typePredicate == null)
                return;

            var serviceTypeCondition = TargetResolver.Resolve(target);

            foreach (var group in lookup)
            {
                if (group.Key.Matches(RegistrationLifeTime.None))
                    continue;

                var matchingTypes = group.Where(t => t != null)
                    .Where(typePredicate);

                var lifetime = group.Key.LifeTime.ToContainerLifeTime();
                var key = group.Key.Key;

                _container.RegisterMany(matchingTypes, lifetime, serviceTypeCondition: serviceTypeCondition, serviceKey: ResolveKey(key));
            }
        }

        public static IRegistrationContainer Adapt(IContainer container)
        {
            return new DryIocContainerAdapter(container);
        }

        private string ResolveKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                return key;

            return null;
        }
    }
}