using SimpleInjector;
using Infrabel.ICT.Framework.Extension;
using Infrabel.ICT.Framework.Ioc;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.SimpleInjectorIoc
{
  public class SimpleInjectorIocContainerAdapter : IRegistrationContainer
  {
    private readonly Container _container;

    private SimpleInjectorIocContainerAdapter(Container container)
    {
      _container = container ?? throw new ArgumentNullException(nameof(container));
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
      throw new NotImplementedException();
    }

    public IRegistrationContainer RegisterDecorator<TAbstract, TConcrete>(RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract
    {
      throw new NotImplementedException();
    }

    public IRegistrationContainer RegisterDecorator(Type abstractType, Type decorator,
        RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false)
    {
      throw new NotImplementedException();
    }

    public IRegistrationContainer Register<TAbstract, TConcrete>(TConcrete instance, Action<TAbstract> cleanupAction = null,
        string key = null, bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract
    {
      _container.RegisterInstance<TAbstract>(instance);

      return this;
    }

    public IRegistrationContainer Register<TAbstract, TConcrete>(
        RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient,
        Action<TAbstract> cleanupAction = null,
        string key = null, bool shouldReplace = false) where TAbstract : class where TConcrete : class, TAbstract
    {
      _container.Register<TAbstract, TConcrete>(registrationLifeTime.ToContainerLifeTime());

      return this;
    }

    public IRegistrationContainer RegisterFactory<TAbstract>(Func<IResolutionContainer, TAbstract> factoryFunc,
        RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false) where TAbstract : class

    {
      SimpleInjectorIocResolverAdapter c =  SimpleInjectorIocResolverAdapter.Adapt(_container);
      _container.Register<TAbstract>(() => factoryFunc(c), registrationLifeTime.ToContainerLifeTime());
      return this;
    }

    public IRegistrationContainer Register(Type abstractType, Type concreteType,
        RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false)
    {
      _container.Register(abstractType, concreteType, registrationLifeTime.ToContainerLifeTime());

      return this;
    }

    public IRegistrationContainer Register(Type concreteType, RegistrationTarget target,
        RegistrationLifeTime registrationLifeTime = RegistrationLifeTime.Transient, string key = null, bool shouldReplace = false)
    {
      var serviceTypeCondition = TargetResolver.Resolve(target);

      //_container.Register(concreteType);
      //_container.Register( concreteType.UnderlyingSystemType, () => concreteType, registrationLifeTime.ToContainerLifeTime());
      _container.Register(concreteType.GetType() , () => concreteType, registrationLifeTime.ToContainerLifeTime());
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

        foreach(Type t in matchingTypes)
        {
          var serviceType= t.GetInterfaces().First().IsGenericType ? t.GetInterfaces().First().GetGenericTypeDefinition() : t.GetInterfaces().First();
          _container.Register(serviceType, t,lifestyle: lifetime );
        }
        
      }
    }

    public static SimpleInjectorIocContainerAdapter Adapt(Container container)
    {
      return new SimpleInjectorIocContainerAdapter(container);
    }

    private string ResolveKey(string key)
    {
      if (!string.IsNullOrWhiteSpace(key))
        return key;

      return null;
    }


  }

  public static class ReflectionTools
  {
    /// <summary>Returns the sensible services automatically discovered for RegisterMany implementation type.
    /// Excludes the collection wrapper interfaces. The <paramref name="type"/> may be concrete, abstract or
    /// generic definition.</summary>
    public static Type[] GetRegisterManyImplementedServiceTypes(this Type type, bool nonPublicServiceTypes = false) =>
      GetImplementedServiceTypes(type, nonPublicServiceTypes)
          .Match(t => !t.IsGenericDefinition() || WrappersSupport.SupportedCollectionTypes.IndexOfReference(t) == -1);

    /// <summary>Returns true if type is generic.</summary>
    public static bool IsGeneric(this Type type) =>
        type.GetTypeInfo().IsGenericType;

    /// <summary>Returns true if type is generic type definition (open type).</summary>
    public static bool IsGenericDefinition(this Type type) =>
        type.GetTypeInfo().IsGenericTypeDefinition;


    /// <summary>Returns only those types that could be used as service types of <paramref name="type"/>.
    /// It means that for open-generic <paramref name="type"/> its service type should supply all type arguments.</summary>
    public static Type[] GetImplementedServiceTypes(this Type type, bool nonPublicServiceTypes = false)
    {
      var implementedTypes = type.GetImplementedTypes(ReflectionTools.AsImplementedType.SourceType);

      var serviceTypes = nonPublicServiceTypes
          ? implementedTypes.Match(t => t.IsServiceType())
          : implementedTypes.Match(t => t.IsPublicOrNestedPublic() && t.IsServiceType());

      if (type.IsGenericDefinition())
        serviceTypes = serviceTypes.Match(type.GetGenericParamsAndArgs(),
            (paramsAndArgs, x) => x.ContainsAllGenericTypeParameters(paramsAndArgs),
            (_, x) => x.GetGenericDefinitionOrNull());

      return serviceTypes;
    }

    /// <summary>Returns all interfaces and all base types (in that order) implemented by <paramref name="sourceType"/>.
    /// Specify <paramref name="asImplementedType"/> to include <paramref name="sourceType"/> itself as first item and
    /// <see cref="object"/> type as the last item.</summary>
    public static Type[] GetImplementedTypes(this Type sourceType, AsImplementedType asImplementedType = AsImplementedType.None)
    {
      Type[] results;

      var interfaces = sourceType.GetImplementedInterfaces();
      var interfaceStartIndex = (asImplementedType & AsImplementedType.SourceType) == 0 ? 0 : 1;
      var includingObjectType = (asImplementedType & AsImplementedType.ObjectType) == 0 ? 0 : 1;
      var sourcePlusInterfaceCount = interfaceStartIndex + interfaces.Length;

      var baseType = sourceType.GetTypeInfo().BaseType;
      if (baseType == null || baseType == typeof(object))
        results = new Type[sourcePlusInterfaceCount + includingObjectType];
      else
      {
        List<Type> baseBaseTypes = null;
        for (var bb = baseType.GetTypeInfo().BaseType; bb != null && bb != typeof(object); bb = bb.GetTypeInfo().BaseType)
          (baseBaseTypes ?? (baseBaseTypes = new List<Type>(2))).Add(bb);

        if (baseBaseTypes == null)
          results = new Type[sourcePlusInterfaceCount + includingObjectType + 1];
        else
        {
          results = new Type[sourcePlusInterfaceCount + baseBaseTypes.Count + includingObjectType + 1];
          baseBaseTypes.CopyTo(results, sourcePlusInterfaceCount + 1);
        }

        results[sourcePlusInterfaceCount] = baseType;
      }

      if (interfaces.Length == 1)
        results[interfaceStartIndex] = interfaces[0];
      else if (interfaces.Length > 1)
        Array.Copy(interfaces, 0, results, interfaceStartIndex, interfaces.Length);

      if (interfaceStartIndex == 1)
        results[0] = sourceType;
      if (includingObjectType == 1)
        results[results.Length - 1] = typeof(object);

      return results;
    }


    /// <summary>Flags for <see cref="GetImplementedTypes"/> method.</summary>
    [Flags]
    public enum AsImplementedType
    {
      /// <summary>Include nor object not source type.</summary>
      None = 0,
      /// <summary>Include source type to list of implemented types.</summary>
      SourceType = 1,
      /// <summary>Include <see cref="System.Object"/> type to list of implemented types.</summary>
      ObjectType = 2
    }

    /// <summary>Gets a collection of the interfaces implemented by the current type and its base types.</summary>
    public static Type[] GetImplementedInterfaces(this Type type) =>
        type.GetTypeInfo().ImplementedInterfaces.ToArrayOrSelf();


    /// <summary>Checks that type can be used a service type.</summary>
    public static bool IsServiceType(this Type type) =>
        !type.IsPrimitive() && !type.IsCompilerGenerated() && !type.IsExcludedGeneralPurposeServiceType();


    /// <summary>Checks that type is not in the list of <see cref="ExcludedGeneralPurposeServiceTypes"/>.</summary>
    public static bool IsExcludedGeneralPurposeServiceType(this Type type) =>
        ExcludedGeneralPurposeServiceTypes.IndexOf((type.Namespace + "." + type.Name).Split('`')[0]) != -1;

    /// <summary>List of types excluded by default from RegisterMany convention.</summary>
    public static readonly string[] ExcludedGeneralPurposeServiceTypes =
    {
            "System.Object",
            "System.IDisposable",
            "System.ValueType",
            "System.ICloneable",
            "System.IEquatable",
            "System.IComparable",
            "System.Runtime.Serialization.ISerializable",
            "System.Collections.IStructuralEquatable",
            "System.Collections.IEnumerable",
            "System.Collections.IList",
            "System.Collections.ICollection",
        };

    /// <summary>Checks if type is public or nested public in public type.</summary>
    public static bool IsPublicOrNestedPublic(this Type type)
    {
      var ti = type.GetTypeInfo();
      return ti.IsPublic || ti.IsNestedPublic && ti.DeclaringType.IsPublicOrNestedPublic();
    }

    /// <summary>Returns generic type parameters and arguments in order they specified. If type is not generic, returns empty array.</summary>
    public static Type[] GetGenericParamsAndArgs(this Type type)
    {
      var ti = type.GetTypeInfo();
      return ti.IsGenericTypeDefinition ? ti.GenericTypeParameters : ti.GenericTypeArguments;
    }

    /// <summary>Returns true if the <paramref name="openGenericType"/> contains all generic parameters
    /// from <paramref name="genericParameters"/>.</summary>
    public static bool ContainsAllGenericTypeParameters(this Type openGenericType, Type[] genericParameters)
    {
      if (!openGenericType.IsOpenGeneric())
        return false;

      var matchedParams = new Type[genericParameters.Length];
      Array.Copy(genericParameters, 0, matchedParams, 0, genericParameters.Length);

      ClearGenericParametersReferencedInConstraints(matchedParams);
      ClearMatchesFoundInGenericParameters(matchedParams, openGenericType.GetGenericParamsAndArgs());

      for (var i = 0; i < matchedParams.Length; i++)
        if (matchedParams[i] != null)
          return false;
      return true;
    }


    private static void ClearMatchesFoundInGenericParameters(Type[] matchedParams, Type[] genericParams)
    {
      for (var i = 0; i < genericParams.Length; i++)
      {
        var genericParam = genericParams[i];
        if (genericParam.IsGenericParameter)
        {
          for (var j = 0; j < matchedParams.Length; ++j)
            if (matchedParams[j] == genericParam)
            {
              matchedParams[j] = null; // match
              break;
            }
        }
        else if (genericParam.IsOpenGeneric())
          ClearMatchesFoundInGenericParameters(matchedParams, genericParam.GetGenericParamsAndArgs());
      }
    }

    /// <summary>Returns array of interface and base class constraints for provider generic parameter type.</summary>
    public static Type[] GetGenericParamConstraints(this Type type) =>
        type.GetTypeInfo().GetGenericParameterConstraints();

    private static void ClearGenericParametersReferencedInConstraints(Type[] genericParams)
    {
      for (var i = 0; i < genericParams.Length; i++)
      {
        var genericParam = genericParams[i];
        if (genericParam == null)
          continue;

        var genericConstraints = genericParam.GetGenericParamConstraints();
        for (var j = 0; j < genericConstraints.Length; j++)
        {
          var genericConstraint = genericConstraints[j];
          if (genericConstraint.IsOpenGeneric())
          {
            var constraintGenericParams = genericConstraint.GetGenericParamsAndArgs();
            for (var k = 0; k < constraintGenericParams.Length; k++)
            {
              var constraintGenericParam = constraintGenericParams[k];
              if (constraintGenericParam != genericParam)
              {
                for (var g = 0; g < genericParams.Length; ++g)
                  if (genericParams[g] == constraintGenericParam)
                  {
                    genericParams[g] = null; // match
                    break;
                  }
              }
            }
          }
        }
      }
    }

    /// <summary>Returns generic type definition if type is generic and null otherwise.</summary>
    public static Type GetGenericDefinitionOrNull(this Type type) =>
        type != null && type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : null;


    /// <summary>Returns true if provided type IsPrimitive in .Net terms, or enum, or string,
    /// or array of primitives if <paramref name="orArrayOfPrimitives"/> is true.</summary>
    public static bool IsPrimitive(this Type type, bool orArrayOfPrimitives = false)
    {
      var typeInfo = type.GetTypeInfo();
      return typeInfo.IsPrimitive || typeInfo.IsEnum || type == typeof(string)
          || orArrayOfPrimitives && typeInfo.IsArray && typeInfo.GetElementType().IsPrimitive(true);
    }

    /// <summary>Returns true if class is compiler generated. Checking for CompilerGeneratedAttribute
    /// is not enough, because this attribute is not applied for classes generated from "async/await".</summary>
    public static bool IsCompilerGenerated(this Type type) =>
        type.FullName != null && type.FullName.Contains("<>c__DisplayClass"); // todo: @perf simplify the check


    /// <summary>Returns true if type if open generic: contains at list one open generic parameter. Could be
    /// generic type definition as well.</summary>
    public static bool IsOpenGeneric(this Type type)
    {
      var typeInfo = type.GetTypeInfo();
      return typeInfo.IsGenericType && typeInfo.ContainsGenericParameters;
    }


  }


}