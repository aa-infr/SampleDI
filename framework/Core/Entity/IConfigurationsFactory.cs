using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Entity
{
    public interface IConfigurationsFactory
    {
        IEnumerable<dynamic> GetConfigurationTypes(string contextName, bool cacheConfigurations = false);
    }
}