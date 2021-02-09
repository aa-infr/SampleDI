using System;
using System.Linq;

namespace Infrabel.ICT.Framework.Ioc
{
    public interface IRegistrationContainer
    {
        IRegistrationContainer RegisterDecorator<TAbstract, TConcrete>(TConcrete instance, string key = "", bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract;

        IRegistrationContainer RegisterDecorator<TAbstract, TConcrete>(RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = "", bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract;

        IRegistrationContainer RegisterDecorator(Type abstractType, Type decorator,
            RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = "", bool shouldReplace = false);

        IRegistrationContainer Register<TAbstract, TConcrete>(TConcrete instance, Action<TAbstract> cleanupAction = null, string key = null, bool shouldReplace = false) where TAbstract : class
            where TConcrete : class, TAbstract;

        IRegistrationContainer Register<TAbstract, TConcrete>(RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, Action<TAbstract> cleanupAction = null, string key = null, bool shouldReplace = false) where TAbstract : class
            where TConcrete : class, TAbstract;

        IRegistrationContainer RegisterFactory<TAbstract>(Func<IResolutionContainer, TAbstract> factoryFunc, RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false) where TAbstract : class;

        IRegistrationContainer Register(Type abstractType, Type concreteType, RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false);

        IRegistrationContainer Register(Type concreteType, RegistrationTarget target, RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false);

        IRegistrationContainer Register<TConcrete>(RegistrationTarget target, RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false);

        IRegistrationContainer BulkRegisterByMatchingType<TMatchingType>(ILookup<RegistrationInfo, Type> lookup, RegistrationTarget target);

        IRegistrationContainer BulkRegisterByMatchingType(Type matchingType, ILookup<RegistrationInfo, Type> lookup, RegistrationTarget target);

        IRegistrationContainer BulkRegisterByMatchingNamespace(ILookup<RegistrationInfo, Type> lookup, string matchingNamespace, RegistrationTarget target);

        IRegistrationContainer BulkRegisterByMatchingNamespaceWithChildren(ILookup<RegistrationInfo, Type> lookup, string matchingNamespace, RegistrationTarget target);

        IRegistrationContainer BulkRegisterByMatchingEndName(ILookup<RegistrationInfo, Type> lookup, string matchingEndName, RegistrationTarget target);

        IRegistrationContainer BulkRegisterByPredicate(ILookup<RegistrationInfo, Type> lookup, Func<Type, bool> predicate, RegistrationTarget target);
    }
}