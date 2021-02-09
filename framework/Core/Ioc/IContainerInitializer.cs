using Infrabel.ICT.Framework.Service;

namespace Infrabel.ICT.Framework.Ioc
{
    public interface IContainerInitializer<in T> where T : class
    {
        IModuleLoader Initialize(T scope, IOptionsResolver optionsResolver, IConnectionStringResolver connectionStringResolver);
    }
}