using System.Collections.Concurrent;
using System.Threading;
using Infrabel.ICT.Framework.Extension;

namespace Infrabel.ICT.Framework.Service
{
    public abstract class ConnectionStringResolverBase : IConnectionStringResolver
    {
        private readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        protected abstract string BuildConnectionString(string name);

        protected void InvalidateCache(IConnectionStringResolver state)
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

        public string Resolve<TDbContext>() where TDbContext : class
        {
            return Resolve(typeof(TDbContext).Name);
        }

        public string Resolve(string name)
        {
            var key = name;
            var result = _locker.ReadExecute(() => _cache.ContainsKey(key) ? _cache[key] : null);

            if(result != null)
                return result;

            return _locker.UpgradableReadExecute(() =>
                                                 {
                                                     if(_cache.ContainsKey(key))
                                                         return _cache[key];

                                                     return _locker.UpgradableWriteExecute(() =>
                                                                                           {
                                                                                               var options = BuildConnectionString(name);
                                                                                               _cache.GetOrAdd(key, options);
                                                                                               return options;
                                                                                           });
                                                 });
        }
    }
}