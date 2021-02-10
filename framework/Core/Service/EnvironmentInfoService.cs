using Infrabel.ICT.Framework.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Infrabel.ICT.Framework.Service
{
    [IoCRegistration(RegistrationLifeTime.Singleton)]
    public class EnvironmentInfoService : IEnvironmentInfoService
    {
        private static readonly string HostNameKey = "HostName";
        private static readonly string EntryAssemblyNameKey = "EntryAssembly";
        private static readonly string EnvironmentNameKey = "EnvironmentName";
        private static readonly string ApplicationVersionKey = "ApplicationVersion";
        private static readonly Dictionary<string, string> EnvironmentData;

        static EnvironmentInfoService()
        {
            var assemblyName = Assembly.GetEntryAssembly()?.GetName();

            EnvironmentData = new Dictionary<string, string>()
            {
                {HostNameKey, Environment.MachineName},
                {EntryAssemblyNameKey,assemblyName?.Name ?? string.Empty},
                {ApplicationVersionKey, assemblyName?.Version.ToString()},
                {EnvironmentNameKey,  Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty},
            };
        }

        public IReadOnlyDictionary<string, string> Dump()
        {
            return new ReadOnlyDictionary<string, string>(EnvironmentData);
        }

        public string HostName => EnvironmentData[HostNameKey];
        public string ApplicationVersion => EnvironmentData[ApplicationVersionKey];
        public string EntryAssemblyName => EnvironmentData[EntryAssemblyNameKey];
        public string EnvironmentName => EnvironmentData[EnvironmentNameKey];
    }
}