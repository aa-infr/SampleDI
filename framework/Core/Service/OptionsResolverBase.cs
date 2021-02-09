using System;
using System.Collections.Concurrent;
using System.Threading;
using Infrabel.ICT.Framework.Extension;

namespace Infrabel.ICT.Framework.Service
{
    public abstract class OptionsResolverBase : IOptionsResolver
    {
        private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        protected abstract TOptions BuildOptions<TOptions>() where TOptions : class, new();

        protected void InvalidateCache(IOptionsResolver state)
        {
            try
            {
                _locker.EnterWriteLock();
                _cache.Clear();
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public TOptions Resolve<TOptions>() where TOptions : class, new()
        {
            var key = typeof(TOptions);
            var result = _locker.ReadExecute(() => _cache.ContainsKey(key) ? _cache[key] as TOptions : null);

            if(result != null)
                return result;

            return _locker.UpgradableReadExecute(() =>
                                                 {
                                                     if(_cache.ContainsKey(key))
                                                         return _cache[key] as TOptions;

                                                     return _locker.WriteExecute(() =>
                                                                                 {
                                                                                     if(_cache.ContainsKey(key))
                                                                                         return _cache[key] as TOptions;
                                                                                     var options = BuildOptions<TOptions>();
                                                                                     _cache.GetOrAdd(key, options);
                                                                                     return options;
                                                                                 });
                                                 });
        }
    }
}