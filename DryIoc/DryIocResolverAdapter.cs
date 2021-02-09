using DryIoc;
using Infrabel.ICT.Framework.Ioc;
using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extended.DryIoc
{
    public class DryIocResolverAdapter : IResolutionContainer
    {
        private readonly IResolverContext _resolver;

        private DryIocResolverAdapter(IResolverContext resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public TType Resolve<TType>(string key = null) where TType : class
        {
            return _resolver.Resolve<TType>(serviceKey: ResolveKey(key));
        }

        public IEnumerable<TType> ResolveAll<TType>(string key = null) where TType : class => _resolver.ResolveMany<TType>(serviceKey: ResolveKey(key));

        public object Resolve(Type typeToResolve, string key = null) =>
            _resolver.Resolve(typeToResolve, ResolveKey(key));

        public IEnumerable<object> ResolveAll(Type typeToResolve, string key = null)
            => _resolver.ResolveMany(typeToResolve, serviceKey: ResolveKey(key));

        private string ResolveKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                return key;

            return null;
        }

        public static DryIocResolverAdapter Adapt(IResolverContext resolver)
        {
            return new DryIocResolverAdapter(resolver);
        }
    }
}