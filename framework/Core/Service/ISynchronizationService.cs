using System;
using System.Threading.Tasks;

namespace Infrabel.ICT.Framework.Service
{
    public interface ISynchronizationService : IDisposable
    {
        void ExecuteInCriticalAccess(string key, Action action);

        Task ExecuteInCriticalAccessAsync(string key, Func<Task> func);
    }
}