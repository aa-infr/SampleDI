using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Service
{
    public interface IEnvironmentInfoService
    {
        IReadOnlyDictionary<string, string> Dump();

        string HostName { get; }
        string ApplicationVersion { get; }
        string EntryAssemblyName { get; }
        string EnvironmentName { get; }
    }
}