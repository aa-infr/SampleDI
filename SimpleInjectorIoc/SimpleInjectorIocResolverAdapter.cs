using SimpleInjector;
using Infrabel.ICT.Framework.Ioc;
using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.SimpleInjectorIoc
{
  public class SimpleInjectorIocResolverAdapter : IResolutionContainer
  {
    private readonly Container _resolver;

    private SimpleInjectorIocResolverAdapter(Container resolver)
    {
      _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    public TType Resolve<TType>(string key = null) where TType : class
    {
      return _resolver.GetInstance<TType>();
    }

    public IEnumerable<TType> ResolveAll<TType>(string key = null) where TType : class => _resolver.GetAllInstances<TType>();

    public object Resolve(Type typeToResolve, string key = null) =>
        _resolver.GetInstance(typeToResolve);

    public IEnumerable<object> ResolveAll(Type typeToResolve, string key = null)
        => _resolver.GetAllInstances(typeToResolve);

    private string ResolveKey(string key)
    {
      if (!string.IsNullOrWhiteSpace(key))
        return key;

      return null;
    }

    public static SimpleInjectorIocResolverAdapter Adapt(Container resolver)
    {
      return new SimpleInjectorIocResolverAdapter(resolver);
    }
  }
}