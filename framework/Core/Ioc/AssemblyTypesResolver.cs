using Infrabel.ICT.Framework.Extension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrabel.ICT.Framework.Ioc
{
    public class AssemblyTypesResolver : IAssemblyTypesResolver
    {
        private readonly ConcurrentDictionary<int, ILookup<RegistrationInfo, Type>> _dictionary =
            new ConcurrentDictionary<int, ILookup<RegistrationInfo, Type>>();

        public ILookup<RegistrationInfo, Type> Resolve()
        {
            return Resolve(Assembly.GetCallingAssembly());
        }

        public ILookup<RegistrationInfo, Type> Resolve<TAnchor>() where TAnchor : class
        {
            var targetAssembly = typeof(TAnchor).Assembly;
            return Resolve(targetAssembly);
        }

        public ILookup<RegistrationInfo, Type> Resolve(Type anchorType)
        {
            var targetAssembly = anchorType.Assembly;

            return Resolve(targetAssembly);
        }

        public ILookup<RegistrationInfo, Type> Resolve(Assembly targetAssembly)
        {
            var typeDictionary = _dictionary.GetOrAdd(targetAssembly.GetHashCode(),
                _ => Feed(targetAssembly));

            return typeDictionary;
        }

        public void ClearCache()
        {
            _dictionary.Clear();
        }

        private ILookup<RegistrationInfo, Type> Feed(Assembly targetAssembly)
        {
            IEnumerable<Type> assemblyTypes = null;
            try
            {
                assemblyTypes = targetAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                assemblyTypes = e.Types.Where(t => t != null);
            }

            var types = assemblyTypes.Where(t => !t.IsAbstract && t.IsClass && !t.IsNestedPrivate && t.Namespace != null)
                .Select(t => new { Type = t, LifeTimeRegistration = t.ResolveRegistrationInfo() })
                .Where(t => t.LifeTimeRegistration.LifeTime != RegistrationLifeTime.None)
                .ToLookup(t => t.LifeTimeRegistration, t => t.Type);

            return types;
        }
    }
}